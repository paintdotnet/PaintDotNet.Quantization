/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

#nullable enable

using PaintDotNet.Collections;
using PaintDotNet.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PaintDotNet.Imaging.Quantization
{
    // NOTE: _NOT_ thread safe due to ref usage w/ DictionarySlim, and because of dCenterToColorCache2

    /// <summary>
    /// This divides up the color space into 16x16x16 cubes (dependent on the precisionBits constant).
    /// For the center of each cube, the palette is copied and sorted with respect to distance from
    /// that center point.
    /// To query for any target point (color), the closest cube's proximity list is obtained and is
    /// searched for the closet point to the target. The math here gets a little tricky; we can detect
    /// when to stop searching based on when a point's ring is further away than the closest point that
    /// we've seen.
    /// A point's "ring" is the circle that contains a point and the cube's center point. The distance
    /// from the target point to that ring is
    ///     abs((distance from center to target) - (distance from center to point))
    /// The math is the same whether we're in 2D or 3D space, it only depends on the delta of the radii.
    /// https://www.varsitytutors.com/hotmath/hotmath_help/topics/shortest-distance-between-a-point-and-a-circle
    /// </summary>
    internal sealed class ProximityPaletteMap
        : PaletteMap
    {
        private const int precisionBits = 4;
        private const int precisionShift = 8 - precisionBits;
        private const int cubeLength = (1 << precisionBits);
        private const int coordOffset = (1 << precisionShift) >> 1;

        private readonly byte[][] paletteIndicesSortedByProximityMap = new byte[cubeLength * cubeLength * cubeLength][];
        private readonly byte[] paletteIndicesTemplate;
        private readonly int[] dCenterToColor2Cache;

        public ProximityPaletteMap(IEnumerable<ColorBgra32> palette)
            : base(palette)
        {
            this.paletteIndicesTemplate = new byte[this.OpaqueColorsArray.Length];
            for (int i = 0; i < this.paletteIndicesTemplate.Length; ++i)
            {
                this.paletteIndicesTemplate[i] = checked((byte)i);
            }

            this.dCenterToColor2Cache = new int[this.OpaqueColorsArray.Length];
        }

        protected override byte OnFindClosestPaletteIndex(ColorBgr24 target)
        {
            ref ColorBgr24 opaqueColors0 = ref this.OpaqueColorsArray[0];
            byte[] paletteIndices = GetPaletteIndicesFromProximityMap(target, out ColorBgr24 centerPt);
            int paletteIndicesLength = paletteIndices.Length;
            ref byte paletteIndices0 = ref paletteIndices[0];

            int dCenterToTarget2 = ColorBgr24Util.GetDistanceSquared(centerPt, target);
            float dCenterToTarget = MathF.Sqrt(dCenterToTarget2);

            byte closestIndex = paletteIndices0;
            ColorBgr24 closest = Unsafe.Add(ref opaqueColors0, closestIndex);
            int dTargetToClosest2 = ColorBgr24Util.GetDistanceSquared(target, closest);
            float? dTargetToClosest = null;

            for (int i = 1; i < paletteIndicesLength; ++i)
            {
                byte colorIndex = Unsafe.Add(ref paletteIndices0, i);
                ColorBgr24 color = Unsafe.Add(ref opaqueColors0, colorIndex);

                int dTargetToColor2 = ColorBgr24Util.GetDistanceSquared(target, color);
                if (dTargetToColor2 <= dTargetToClosest2)
                {
                    // If this color is closer, take it
                    closestIndex = colorIndex;
                    dTargetToClosest2 = dTargetToColor2;
                    dTargetToClosest = null;
                }
                else
                {
                    // This color wasn't closer, so let's check its "ring." The color's ring is the circle whose center
                    // is at centerPt, and who's radius is equal to the distance from centerPt to the color (color is
                    // always on the ring, of course). This then tells us the closest that any color on that ring could
                    // be (imagine the color on the ring that is closest to the target: it sits on the line that is
                    // formed from centerPt to target). The distance from centerPt to the rings will never decrease,
                    // and the distance from target to the rings will decrease and then increase as we enumerate colors
                    // via paletteIndices. Once a ring is farther away than the closest color we've seen, we're done.
                    int dCenterToColor2 = ColorBgr24Util.GetDistanceSquared(centerPt, color);
                    float dCenterToColor = MathF.Sqrt(dCenterToColor2);

                    float dTargetToColorRing = MathF.Abs(dCenterToTarget - dCenterToColor);
                    dTargetToClosest ??= MathF.Sqrt(dTargetToClosest2);

                    if (dTargetToColorRing > dTargetToClosest)
                    {
                        break;
                    }
                }
            }

            return closestIndex;
        }

        private byte[] GetPaletteIndicesFromProximityMap(ColorBgr24 color, out ColorBgr24 centerPt)
        {
            Point3Int32 coordinate = new Point3Int32(
                color.B >> precisionShift,
                color.G >> precisionShift,
                color.R >> precisionShift);

            ColorBgr24 centerPt2 = ColorBgr24.FromBgr(
                checked((byte)((coordinate.X << precisionShift) + coordOffset)),
                checked((byte)((coordinate.Y << precisionShift) + coordOffset)),
                checked((byte)((coordinate.Z << precisionShift) + coordOffset)));

            int coordinateIndex = coordinate.X | (coordinate.Y << precisionBits) | (coordinate.Z << (precisionBits * 2));
            ref byte[] paletteIndices = ref this.paletteIndicesSortedByProximityMap[coordinateIndex];
            if (paletteIndices == null)
            {
                paletteIndices = this.paletteIndicesTemplate.ToArray();
                ArrayStruct<byte> fastPaletteIndices = new ArrayStruct<byte>(paletteIndices);

                for (int i = 0; i < this.dCenterToColor2Cache.Length; ++i)
                {
                    ColorBgr24 opaqueColor = this.OpaqueColorsArray[i];
                    int dCenterToColor2 = ColorBgr24Util.GetDistanceSquared(opaqueColor, centerPt2);
                    this.dCenterToColor2Cache[i] = dCenterToColor2;
                }

                IndexByDistanceComparer comparer = new IndexByDistanceComparer(this.dCenterToColor2Cache);
                ListUtil.Sort<byte, ArrayStruct<byte>, IndexByDistanceComparer>(fastPaletteIndices, comparer);
            }

            centerPt = centerPt2;
            return paletteIndices;
        }

        private struct IndexByDistanceComparer
            : IComparer<byte>
        {
            private readonly int[] dCenterToColor2;
            
            public IndexByDistanceComparer(int[] dCenterToColor2)
            {
                this.dCenterToColor2 = dCenterToColor2;
            }

            public int Compare(byte index1, byte index2)
            {
                int distance1 = this.dCenterToColor2[index1];
                int distance2 = this.dCenterToColor2[index2];

                // TODO: tie-break by angle?
                int dCompare = distance1.CompareTo(distance2);
                return dCompare;
            }
        }
    }
}
