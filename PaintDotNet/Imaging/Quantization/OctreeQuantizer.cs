/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

// Based on: http://msdn.microsoft.com/en-us/library/aa479306.aspx

using PaintDotNet.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace PaintDotNet.Imaging.Quantization
{
    internal sealed class OctreeQuantizer
        : IQuantizer
    {
        public OctreeQuantizer()
        {
        }

        public ColorBgra32[] GeneratePalette(IBitmapSource<ColorBgra32> source, int maxColorCount, bool addTransparentColor, CancellationToken cancelToken)
        {
            if (maxColorCount < 2 || maxColorCount > 256)
            {
                throw new ArgumentOutOfRangeException(nameof(maxColorCount));
            }

            cancelToken.ThrowIfCancellationRequested();

            ColorHistogram<ColorBgr24> histogram = ColorHistogram.CreateOpaque(source, cancelToken);

            cancelToken.ThrowIfCancellationRequested();

            return GeneratePalette(histogram, maxColorCount, addTransparentColor, cancelToken);
        }

        public ColorBgra32[] GeneratePalette(ColorHistogram<ColorBgr24> histogram, int maxColorCount, bool addTransparentColor, CancellationToken cancelToken)
        {
            if (maxColorCount < 1 || maxColorCount > 256)
            {
                throw new ArgumentOutOfRangeException(nameof(maxColorCount));
            }

            if (histogram.Count == 0)
            {
                return addTransparentColor
                    ? new ColorBgra32[] { Colors.Black, Colors.Zero }
                    : new ColorBgra32[] { Colors.Black };
            }

            cancelToken.ThrowIfCancellationRequested();

            int colorCount = addTransparentColor ? (maxColorCount - 1) : maxColorCount;
            ColorBgr24[] paletteBgr24 = GeneratePaletteImpl(histogram, colorCount, cancelToken);

            cancelToken.ThrowIfCancellationRequested();

            if (paletteBgr24.Length > colorCount)
            {
                throw new InternalErrorException();
            }

            IEnumerable<ColorBgra32> paletteBgra32 = paletteBgr24.Select(c => (ColorBgra32)c);
            if (addTransparentColor)
            {
                paletteBgra32 = paletteBgra32.Concat(Colors.TransparentBlack);
            }

            ColorBgra32[] palette = paletteBgra32.ToArray();
            return palette;
        }

        public ColorBgr24[] GeneratePaletteImpl(ColorHistogram<ColorBgr24> histogram, int maxColorCount, CancellationToken cancelToken)
        {
            Octree octree = new Octree();

            int count = 0;
            foreach (ColorHistogram<ColorBgr24>.Entry entry in histogram)
            {
                octree.AddColor(entry.Color, entry.Count);

                ++count;
                if ((count & 1023) == 0)
                {
                    cancelToken.ThrowIfCancellationRequested();
                }
            }

            cancelToken.ThrowIfCancellationRequested();

            octree.Reduce(maxColorCount, cancelToken);

            cancelToken.ThrowIfCancellationRequested();

            List<ColorBgr24> palette = new List<ColorBgr24>();
            octree.GatherColors(palette, cancelToken);

            cancelToken.ThrowIfCancellationRequested();

            return palette.ToArray();
        }

        private sealed class Octree
        {
            private OctreeNode rootNode;
            private int colorCount;

            public Octree()
            {
                this.rootNode = new OctreeNodeFull();
            }

            public void AddColor(ColorBgr24 color, long count)
            {
                AddColor(this.rootNode, 0, color, count);
            }

            private void AddColor(OctreeNode node, int nodeLevel, ColorBgr24 color, long count)
            {
                int offset = GetColorOffset(color, nodeLevel);

                if (nodeLevel < 7)
                {
                    OctreeNode childNode = node.TryGetChild(offset);
                    if (childNode == null)
                    {
                        childNode = new OctreeNodeFull();
                        node.SetChild(offset, childNode);
                    }

                    AddColor(childNode, nodeLevel + 1, color, count);
                }
                else
                {
                    OctreeNode childNode = node.TryGetChild(offset);
                    if (childNode == null)
                    {
                        childNode = OctreeNodeSimple.Create(color, count);
                        node.SetChild(offset, childNode);
                        ++this.colorCount;
                    }
                    else
                    {
                        OctreeNode childNode2 = new OctreeNodeFull();
                        childNode2.AddColorInfo(childNode);
                        childNode2.AddColorInfo(color, count);
                        node.SetChild(offset, childNode2);
                    }
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static int GetColorOffset(ColorBgr24 color, int level)
            {
                int shift = 7 - level;
                byte mask = (byte)(1 << shift);

                int index = (color.R & mask) >> shift |
                            (((color.G & mask) >> shift) << 1) |
                            (((color.B & mask) >> shift) << 2);

                return index;
            }

            public void Reduce(int targetColorCount, CancellationToken cancelToken)
            {
                cancelToken.ThrowIfCancellationRequested();

                int[] levelNodeCounts = GetLevelNodeCounts(cancelToken);

                cancelToken.ThrowIfCancellationRequested();

                // Fully reduce levels until we reach a point where level[n].count > targetColorCount and level[n-1].count <= targetColorCount
                int level;
                for (level = 8; level >= 1 && levelNodeCounts[level - 1] > targetColorCount; --level)
                {
                    MergeLevelIntoParent(level, cancelToken);
                    levelNodeCounts[level] = 0; // not strictly necessary, but helpful for debugging
                }

                cancelToken.ThrowIfCancellationRequested();

                // Gather level[n]'s nodes, which we need to do a partial merge of. Because we've already reduced
                // most of the way, the memory usage should not be too much -- on the order of 1000 nodes.
                List<(OctreeNode node, OctreeNode parentNode, int childOffset)> finalLevelNodes =
                    new List<(OctreeNode node, OctreeNode parentNode, int childOffset)>(levelNodeCounts[level]);
                GatherLevelNodes(level, finalLevelNodes, cancelToken);

                cancelToken.ThrowIfCancellationRequested();

                // Sort level[n]'s node by count -- we want to merge nodes with the smallest counts first.
                // For a comparison tie-breaker, use the color's hash code
                ListUtil.SortParallel(
                    finalLevelNodes,
                    (nodeInfo1, nodeInfo2) =>
                    {
                        OctreeNode node1 = nodeInfo1.node;
                        OctreeNode node2 = nodeInfo2.node;

                        int countCompare = node1.ColorCount.CompareTo(node2.ColorCount);
                        if (countCompare != 0)
                        {
                            return countCompare;
                        }

                        ColorRgb96Float color1 = node1.Color;
                        ColorRgb96Float color2 = node2.Color;
                        if (color1 == color2)
                        {
                            return 0;
                        }

                        // Hash of color -- we just need some kind of tie-breaker that isn't lopsided,
                        // that's well-behaved, and that isn't just a random number generator or something
                        // weird or difficult to port to other languages/platforms/etc.
                        int hash1 = color1.GetHashCode();
                        int hash2 = color2.GetHashCode();
                        int hashCompare = hash1.CompareTo(hash2);
                        if (hashCompare != 0)
                        {
                            return hashCompare;
                        }

                        // Final tie breaker, just arbitrarily compare the color components
                        return CompareColorsParallel(color1, color2);
                    });

                cancelToken.ThrowIfCancellationRequested();

                // Merge until our color count reaches the target. Note that the first merge into a particular parent
                // node will not reduce the color count because the parent node now has color info and is included in
                // the color count.
                for (int i = 0, count = finalLevelNodes.Count; i < count && this.colorCount > targetColorCount; ++i)
                {
                    var nodeInfo = finalLevelNodes[i];
                    MergeChildIntoParent(nodeInfo.parentNode, nodeInfo.childOffset, nodeInfo.node);
                    --levelNodeCounts[level]; // not strictly necessary, but helpful for debugging
                }

                cancelToken.ThrowIfCancellationRequested();
            }

            private int[] GetLevelNodeCounts(CancellationToken cancelToken)
            {
                int[] levelNodeCounts = new int[9];
                GetLevelNodeCounts(levelNodeCounts, this.rootNode, 0, cancelToken);
                return levelNodeCounts;
            }

            private void GetLevelNodeCounts(int[] levelNodeCounts, OctreeNode node, int nodeLevel, CancellationToken cancelToken)
            {
                cancelToken.ThrowIfCancellationRequested();

                ++levelNodeCounts[nodeLevel];

                if (node.HasChildren)
                {
                    for (int i = 0; i < 8; ++i)
                    {
                        OctreeNode childNode = node.TryGetChild(i);
                        if (childNode != null)
                        {
                            GetLevelNodeCounts(levelNodeCounts, childNode, nodeLevel + 1, cancelToken);
                        }
                    }
                }
            }

            private void MergeLevelIntoParent(int level, CancellationToken cancelToken)
            {
                MergeLevelIntoParent(level, this.rootNode, 0, cancelToken);
            }

            private void MergeLevelIntoParent(int level, OctreeNode node, int nodeLevel, CancellationToken cancelToken)
            {
                cancelToken.ThrowIfCancellationRequested();

                if (!node.HasChildren)
                {
                    return;
                }

                bool mergeChildren = (level == nodeLevel + 1);
                for (int i = 0; i < 8; ++i)
                {
                    OctreeNode childNode = node.TryGetChild(i);
                    if (childNode == null)
                    {
                        continue;
                    }

                    if (mergeChildren)
                    {
                        Debug.Assert(!childNode.HasChildren);
                        MergeChildIntoParent(node, i, childNode);
                    }
                    else
                    {
                        MergeLevelIntoParent(level, childNode, nodeLevel + 1, cancelToken);
                    }
                }
            }

            private void MergeChildIntoParent(OctreeNode parentNode, int childOffset, OctreeNode childNode)
            {
                Debug.Assert(!childNode.HasChildren);

                if (!parentNode.HasColorInfo)
                {
                    ++this.colorCount;
                }

                parentNode.AddColorInfo(childNode);

                parentNode.RemoveChild(childOffset);
                --this.colorCount;
            }

            private void GatherLevelNodes(int level, ICollection<(OctreeNode node, OctreeNode parentNode, int childOffset)> output, CancellationToken cancelToken)
            {
                Debug.Assert(level >= 1);
                GatherLevelNodes(level, this.rootNode, 0, output, cancelToken);
            }

            private void GatherLevelNodes(int level, OctreeNode node, int nodeLevel, ICollection<(OctreeNode node, OctreeNode parentNode, int childOffset)> output, CancellationToken cancelToken)
            {
                cancelToken.ThrowIfCancellationRequested();

                if (!node.HasChildren)
                {
                    return;
                }

                bool gatherChildren = (level == nodeLevel + 1);
                for (int i = 0; i < 8; ++i)
                {
                    OctreeNode childNode = node.TryGetChild(i);
                    if (childNode == null)
                    {
                        continue;
                    }

                    if (gatherChildren)
                    {
                        Debug.Assert(!childNode.HasChildren);
                        output.Add((childNode, node, i));
                    }
                    else
                    {
                        GatherLevelNodes(level, childNode, nodeLevel + 1, output, cancelToken);
                    }
                }
            }

            public void GatherColors(ICollection<ColorBgr24> output, CancellationToken cancelToken)
            {
                GatherColors(output, this.rootNode, cancelToken);
            }

            private void GatherColors(ICollection<ColorBgr24> output, OctreeNode node, CancellationToken cancelToken)
            {
                if (node.HasColorInfo)
                {
                    ColorRgb96Float colorF = node.Color;
                    ColorBgr24 color = ColorBgr24.Round(colorF);
                    output.Add(color);
                }

                if (node.HasChildren)
                {
                    for (int i = 0; i < 8; ++i)
                    {
                        OctreeNode childNode = node.TryGetChild(i);
                        if (childNode != null)
                        {
                            GatherColors(output, childNode, cancelToken);
                            cancelToken.ThrowIfCancellationRequested();
                        }
                    }
                }
            }

            // Compare the colors bit-by-bit, giving equal weighting to R, G, and B.
            // The color is first rounded to 16-bits integer per component
            private int CompareColorsParallel(ColorRgb96Float color1, ColorRgb96Float color2)
            {
                ColorBgr48 color1I = ColorBgr48.Round(color1);
                ColorBgr48 color2I = ColorBgr48.Round(color2);
                if (color1I == color2I)
                {
                    return 0;
                }

                for (int bit = 15; bit >= 0; --bit)
                {
                    ushort mask = (ushort)(1 << bit);

                    int r1 = color1I.R & mask;
                    int r2 = color2I.R & mask;
                    int rCompare = ComparableUtil.CompareTrinary(r1, r2);

                    int g1 = color1I.G & mask;
                    int g2 = color2I.G & mask;
                    int gCompare = ComparableUtil.CompareTrinary(g1, g2);

                    int b1 = color1I.B & mask;
                    int b2 = color2I.B & mask;
                    int bCompare = ComparableUtil.CompareTrinary(b1, b2);

                    int rgbCompare = rCompare + gCompare + bCompare;
                    if (rgbCompare != 0)
                    {
                        return rgbCompare;
                    }
                }

                return 0;
            }
        }

        private abstract class OctreeNode
        {
            protected OctreeNode()
            {
            }

            public abstract bool HasChildren
            {
                get;
            }

            public bool HasColorInfo => this.ColorCount > 0;

            public abstract long ColorCount
            {
                get;
            }

            public abstract (long r, long g, long b) ColorComponentSums
            {
                get;
            }

            public ColorRgb96Float Color
            {
                get
                {
                    long colorCount = this.ColorCount;
                    (long r, long g, long b) componentSums = this.ColorComponentSums;
                    return new ColorRgb96Float(
                        (float)(((double)componentSums.r / colorCount) / 255.0),
                        (float)(((double)componentSums.g / colorCount) / 255.0),
                        (float)(((double)componentSums.b / colorCount) / 255.0));
                }
            }

            public abstract bool CanAddColorInfo
            {
                get;
            }

            public abstract void AddColorInfo(OctreeNode node);

            public abstract void AddColorInfo(ColorBgr24 color, long count);

            public OctreeNode GetChild(int offset)
            {
                OctreeNode node = TryGetChild(offset);
                if (node == null)
                {
                    throw new KeyNotFoundException();
                }

                return node;
            }

            public abstract OctreeNode TryGetChild(int offset);

            public abstract void SetChild(int offset, OctreeNode childNode);

            public abstract void RemoveChild(int offset);
        }

        // A simple node must be a leaf (no children), and has limited storage to store color information.
        // This is done to save memory because there can be a LOT of leaf nodes, and consuming 32 bytes for
        // each one can add up really fast.
        private abstract class OctreeNodeSimple
            : OctreeNode
        {
            public static OctreeNodeSimple Create(ColorBgr24 color, long count)
            {
                if (count < 1)
                {
                    throw new ArgumentOutOfRangeException();
                }

                if (count == 1)
                {
                    return new OctreeNodeSimple1(color);
                }
                else if (count <= uint.MaxValue)
                {
                    return new OctreeNodeSimple32(color, (uint)count);
                }
                else
                {
                    return new OctreeNodeSimple64(color, count);
                }
            }

            private ColorBgr24 color;

            protected OctreeNodeSimple(ColorBgr24 color)
            {
                this.color = color;
            }

            public override bool HasChildren => false;

            public override (long r, long g, long b) ColorComponentSums
            {
                get
                {
                    long colorCount = this.ColorCount;

                    return (this.color.R * colorCount,
                            this.color.G * colorCount,
                            this.color.B * colorCount);
                }
            }

            public override bool CanAddColorInfo => false;

            public override void AddColorInfo(ColorBgr24 color, long count)
            {
                throw new NotSupportedException();
            }

            public override void AddColorInfo(OctreeNode node)
            {
                throw new NotSupportedException();
            }

            public override OctreeNode TryGetChild(int offset)
            {
                return null;
            }

            public override void SetChild(int offset, OctreeNode childNode)
            {
                throw new NotSupportedException();
            }

            public override void RemoveChild(int offset)
            {
                throw new NotSupportedException();
            }
        }

        private sealed class OctreeNodeSimple1
            : OctreeNodeSimple
        {
            public OctreeNodeSimple1(ColorBgr24 color)
                : base(color)
            {
            }

            public override long ColorCount => 1;
        }

        private sealed class OctreeNodeSimple32
            : OctreeNodeSimple
        {
            private uint count;

            public OctreeNodeSimple32(ColorBgr24 color, uint count)
                : base(color)
            {
                this.count = count;
            }

            public override long ColorCount => this.count;
        }

        private sealed class OctreeNodeSimple64
            : OctreeNodeSimple
        {
            private long count;

            public OctreeNodeSimple64(ColorBgr24 color, long count)
                : base(color)
            {
                this.count = count;
            }

            public override long ColorCount => this.count;
        }

        private sealed class OctreeNodeFull
            : OctreeNode
        {
            private long colorCount;
            private long redSum;
            private long greenSum;
            private long blueSum;

            private OctreeNode[] children;
            private int childrenCount;

            public OctreeNodeFull()
            {
            }

            public override bool HasChildren => this.children != null;

            public override long ColorCount => this.colorCount;

            public override (long r, long g, long b) ColorComponentSums => (this.redSum, this.greenSum, this.blueSum);

            public override bool CanAddColorInfo => true;

            public override void AddColorInfo(OctreeNode node)
            {
                this.colorCount += node.ColorCount;
                (long r, long g, long b) componentSums = node.ColorComponentSums;
                this.redSum += componentSums.r;
                this.greenSum += componentSums.g;
                this.blueSum += componentSums.b;
            }

            public override void AddColorInfo(ColorBgr24 color, long count)
            {
                this.redSum += color.R * count;
                this.greenSum += color.G * count;
                this.blueSum += color.B * count;
                this.colorCount += count;
            }

            public override OctreeNode TryGetChild(int offset)
            {
                if (this.children == null)
                {
                    return null;
                }

                return this.children[offset];
            }

            public override void SetChild(int offset, OctreeNode childNode)
            {
                if (this.children == null)
                {
                    this.children = new OctreeNode[8];
                }

                if (this.children[offset] != null)
                {
                    --this.childrenCount;
                }

                this.children[offset] = childNode;
                ++this.childrenCount;
            }

            public override void RemoveChild(int offset)
            {
                if (this.children == null)
                {
                    throw new InvalidOperationException();
                }

                if (this.children[offset] == null)
                {
                    throw new ArgumentOutOfRangeException();
                }

                this.children[offset] = null;
                --this.childrenCount;
                if (this.childrenCount == 0)
                {
                    this.children = null;
                }
            }
        }
    }
}
