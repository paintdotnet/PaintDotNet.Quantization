/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

// Based on: http://msdn.microsoft.com/en-us/library/aa479306.aspx

#nullable enable

using PaintDotNet.Rendering;
using System;

namespace PaintDotNet.Imaging.Quantization
{
    using GdipBitmap = System.Drawing.Bitmap;
    using GdipBitmapData = System.Drawing.Imaging.BitmapData;
    using GdipPixelFormat = System.Drawing.Imaging.PixelFormat;
    using WicPixelFormat = PaintDotNet.Imaging.PixelFormat;

    public unsafe sealed class QuantizedBitmapSource
        : Disposable,
          IBitmapSource<ColorIndexed8>
    {
        private IBitmapSource<ColorBgra32>? source;
        private readonly PaletteMap paletteMap;
        private readonly int ditherLevel;
        private CopyPixelsImpl? copyPixelsImplCache;

        public QuantizedBitmapSource(
            IBitmapSource<ColorBgra32> source,
            PaletteMap paletteMap,
            int ditherLevel)
        {
            if (ditherLevel < 0 || ditherLevel > 8)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(ditherLevel),
                    ditherLevel,
                    "Out of bounds. Must be in the range [0, 8]");
            }

            this.source = source;
            this.paletteMap = paletteMap;
            this.ditherLevel = ditherLevel;
        }

        protected override void Dispose(bool disposing)
        {
            this.source = null;
            base.Dispose(disposing);
        }

        public SizeInt32 Size => this.source!.Size;

        public PixelFormat PixelFormat => PixelFormat.Indexed8;

        public int NextY => this.copyPixelsImplCache?.NextY ?? 0;

        public void CopyPixels(
            RectInt32? srcRect,
            ColorIndexed8* pBuffer,
            int bufferStride)
        {
            RectInt32 rect = srcRect ?? new RectInt32(Point2Int32.Zero, this.Size);

            if (this.copyPixelsImplCache == null)
            {
                this.copyPixelsImplCache = new CopyPixelsImpl(this);
            }
            else if (rect.Top < this.copyPixelsImplCache.NextY)
            {
                this.copyPixelsImplCache = new CopyPixelsImpl(this);
            }

            unsafe
            {
                WriteQuantizedRowCallback writeRowCallback =
                    delegate (int srcY, byte* pQuantizedRow)
                    {
                        if (srcY < rect.Top || srcY >= rect.Bottom)
                        {
                            return;
                        }

                        int dstY = srcY - rect.Top;
                        byte* pDst = (byte*)pBuffer + ((long)bufferStride * (long)dstY);
                        byte* pSrc = pQuantizedRow + rect.Left;
                        BufferUtil.Copy((void*)pDst, (void*)pSrc, rect.Width);
                    };

                this.copyPixelsImplCache.CopyRows(rect.Top, rect.Bottom, writeRowCallback);
            }
        }

        internal unsafe delegate void WriteQuantizedRowCallback(int srcY, byte* pQuantizedRow);

        private sealed class CopyPixelsImpl
        {
            private readonly QuantizedBitmapSource owner;
            private readonly ColorBgra32[] sourceRow;
            private readonly byte[] destinationRow;

            private int[] errorThisRowR;
            private int[] errorThisRowG;
            private int[] errorThisRowB;

            private int[] errorNextRowR;
            private int[] errorNextRowG;
            private int[] errorNextRowB;

            private int nextY;
            public int NextY => this.nextY;

            public CopyPixelsImpl(QuantizedBitmapSource owner)
            {
                this.owner = owner;

                SizeInt32 size = this.owner.source!.Size;
                int width = size.Width;
                this.sourceRow = new ColorBgra32[width];
                this.destinationRow = new byte[width];

                this.errorThisRowR = new int[width + 1];
                this.errorThisRowG = new int[width + 1];
                this.errorThisRowB = new int[width + 1];

                this.errorNextRowR = new int[width + 1];
                this.errorNextRowG = new int[width + 1];
                this.errorNextRowB = new int[width + 1];
            }

            public void CopyRows(
                int topRow,
                int bottomRow,
                WriteQuantizedRowCallback writeRowCallback)
            {
                int ditherLevel = this.owner.ditherLevel;
                if (ditherLevel == 0)
                {
                    CopyRowsNoDithering(topRow, bottomRow, writeRowCallback);
                }
                else
                {
                    CopyRowsFloydSteinberg(topRow, bottomRow, ditherLevel, writeRowCallback);
                }
            }

            public void CopyRowsNoDithering(
                int topRow,
                int bottomRow,
                WriteQuantizedRowCallback writeRowCallback)
            {
                PaletteMap paletteMap = this.owner.paletteMap;
                SizeInt32 size = this.owner.source!.Size;
                int width = size.Width;
                int height = size.Height;

                fixed (ColorBgra32* pSourceRow = this.sourceRow)
                fixed (byte* pDestinationRow = this.destinationRow)
                {
                    for (int y = topRow; y < bottomRow; ++y)
                    {
                        this.owner.source.CopyPixels(
                            new RectInt32(0, y, size.Width, 1),
                            pSourceRow,
                            size.Width * sizeof(ColorBgra32));

                        for (int x = 0; x < width; ++x)
                        {
                            ColorBgra32 targetColor = pSourceRow[x];
                            byte paletteIndex = paletteMap.FindClosestPaletteIndex(targetColor);
                            pDestinationRow[x] = paletteIndex;
                        }

                        writeRowCallback(y, pDestinationRow);
                    }
                }
            }

            public void CopyRowsFloydSteinberg(
                int topRow,
                int bottomRow,
                int ditherLevel,
                WriteQuantizedRowCallback writeRowCallback)
            {
                // we can go forward but never backward
                if (topRow < this.nextY)
                {
                    throw new InvalidOperationException();
                }

                PaletteMap paletteMap = this.owner.paletteMap;
                SizeInt32 size = this.owner.source!.Size;
                int width = size.Width;
                int height = size.Height;

                fixed (ColorBgra32* pSourceRow = this.sourceRow)
                fixed (byte* pDestinationRow = this.destinationRow)
                {
                    for (int y = this.nextY; y < bottomRow; ++y)
                    {
                        this.owner.source.CopyPixels(
                            new RectInt32(0, y, size.Width, 1),
                            pSourceRow,
                            size.Width * sizeof(ColorBgra32));

                        ColorBgra32* pSourcePixel;
                        byte* pDestinationPixel;
                        int ptrInc;
                        if ((y & 1) == 0)
                        {
                            pSourcePixel = pSourceRow;
                            pDestinationPixel = pDestinationRow;
                            ptrInc = +1;
                        }
                        else
                        {
                            pSourcePixel = pSourceRow + width - 1;
                            pDestinationPixel = pDestinationRow + width - 1;
                            ptrInc = -1;
                        }

                        for (int x = 0; x < width; ++x)
                        {
                            ColorBgra32 srcColor = *pSourcePixel;

                            ColorBgra32 targetColor = ColorBgra32.FromBgra(
                                Int32Util.ClampToByte(srcColor.B - ((this.errorThisRowB[x] * ditherLevel) >> 3)),
                                Int32Util.ClampToByte(srcColor.G - ((this.errorThisRowG[x] * ditherLevel) >> 3)),
                                Int32Util.ClampToByte(srcColor.R - ((this.errorThisRowR[x] * ditherLevel) >> 3)),
                                srcColor.A);

                            byte paletteIndex = paletteMap.FindClosestPaletteIndex(targetColor);
                            *pDestinationPixel = paletteIndex;

                            ColorBgra32 paletteColor = paletteMap.Colors[paletteIndex];

                            int errorR = paletteColor.R - targetColor.R;
                            int errorG = paletteColor.G - targetColor.G;
                            int errorB = paletteColor.B - targetColor.B;

                            // Floyd-Steinberg Error Diffusion:
                            // a) 7/16 error goes to x+1
                            // b) 5/16 error goes to y+1
                            // c) 3/16 error goes to x-1,y+1
                            // d) 1/16 error goes to x+1,y+1

                            const int a = 7;
                            const int b = 5;
                            const int c = 3;

                            int errorRa = (errorR * a) >> 4;
                            int errorRb = (errorR * b) >> 4;
                            int errorRc = (errorR * c) >> 4;
                            int errorRd = errorR - errorRa - errorRb - errorRc;

                            int errorGa = (errorG * a) >> 4;
                            int errorGb = (errorG * b) >> 4;
                            int errorGc = (errorG * c) >> 4;
                            int errorGd = errorG - errorGa - errorGb - errorGc;

                            int errorBa = (errorB * a) >> 4;
                            int errorBb = (errorB * b) >> 4;
                            int errorBc = (errorB * c) >> 4;
                            int errorBd = errorB - errorBa - errorBb - errorBc;

                            this.errorThisRowR[x + 1] += errorRa;
                            this.errorThisRowG[x + 1] += errorGa;
                            this.errorThisRowB[x + 1] += errorBa;

                            this.errorNextRowR[width - x] += errorRb;
                            this.errorNextRowG[width - x] += errorGb;
                            this.errorNextRowB[width - x] += errorBb;

                            if (x != 0)
                            {
                                this.errorNextRowR[width - (x - 1)] += errorRc;
                                this.errorNextRowG[width - (x - 1)] += errorGc;
                                this.errorNextRowB[width - (x - 1)] += errorBc;
                            }

                            this.errorNextRowR[width - (x + 1)] += errorRd;
                            this.errorNextRowG[width - (x + 1)] += errorGd;
                            this.errorNextRowB[width - (x + 1)] += errorBd;

                            // unchecked is necessary because otherwise it throws a fit if ptrInc is negative.
                            unchecked
                            {
                                pSourcePixel += ptrInc;
                                pDestinationPixel += ptrInc;
                            }
                        }

                        ObjectUtil.Swap(ref this.errorThisRowB, ref this.errorNextRowB);
                        ObjectUtil.Swap(ref this.errorThisRowG, ref this.errorNextRowG);
                        ObjectUtil.Swap(ref this.errorThisRowR, ref this.errorNextRowR);

                        Array.Clear(this.errorNextRowB, 0, this.errorNextRowB.Length);
                        Array.Clear(this.errorNextRowG, 0, this.errorNextRowG.Length);
                        Array.Clear(this.errorNextRowR, 0, this.errorNextRowR.Length);

                        this.nextY = y + 1;

                        if (y >= topRow && y < bottomRow)
                        {
                            writeRowCallback(y, pDestinationRow);
                        }
                    }
                }
            }
        }
    }
}
