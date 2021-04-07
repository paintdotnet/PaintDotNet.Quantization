/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

#nullable enable

using PaintDotNet.Rendering;
using System;
using System.Collections.Generic;

namespace PaintDotNet.Imaging
{
    internal sealed class Indexed8ToBgra32FormatConverter
        : Disposable,
          IBitmapSource<ColorBgra32>
    {
        private IBitmapSource<ColorIndexed8> source;
        private IReadOnlyList<ColorBgra32> palette;

        public Indexed8ToBgra32FormatConverter(IBitmapSource<ColorIndexed8> source)
        {
            this.source = source;
            this.palette = source.GetPalette()!;
        }

        protected override void Dispose(bool disposing)
        {
            this.source = null!;
            base.Dispose(disposing);
        }

        public PixelFormat PixelFormat => PixelFormat.Bgra32;

        public SizeInt32 Size => this.source.Size;

        public unsafe void CopyPixels(RectInt32? srcRect, void* pBuffer, int bufferStride)
        {
            CopyPixelsImpl(srcRect ?? new RectInt32(Point2Int32.Zero, this.Size), (ColorBgra32*)pBuffer, bufferStride);
        }

        private unsafe void CopyPixelsImpl(RectInt32 srcRect, ColorBgra32* pBuffer, int bufferStride)
        {
            ColorIndexed8[] srcBuffer = new ColorIndexed8[srcRect.Width];
            fixed (ColorIndexed8* pSrcBuffer = srcBuffer)
            {
                ColorBgra32* pBufferRow = pBuffer;
                for (int dstY = 0; dstY < srcRect.height; ++dstY)
                {
                    this.source.CopyPixels(new RectInt32(srcRect.x, srcRect.Top + dstY, srcRect.width, 1), pSrcBuffer, srcRect.Width);

                    for (int x = 0; x < srcRect.width; ++x)
                    {
                        pBufferRow[x] = this.palette[(byte)pSrcBuffer[x]];
                    }

                    pBufferRow = (ColorBgra32*)((byte*)pBufferRow + bufferStride);
                }
            }
        }

        public IReadOnlyList<ColorBgra32>? GetPalette()
        {
            return null;
        }
    }
}
