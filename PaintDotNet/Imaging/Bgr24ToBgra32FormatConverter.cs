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
    internal sealed class Bgr24ToBgra32FormatConverter
        : Disposable,
          IBitmapSource<ColorBgra32>
    {
        private IBitmapSource<ColorBgr24> source;

        public Bgr24ToBgra32FormatConverter(IBitmapSource<ColorBgr24> source)
        {
            this.source = source;
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
            ColorBgra32* pRowBgra32 = pBuffer;
            for (int dstY = 0; dstY < srcRect.height; ++dstY)
            {
                ColorBgr24* pRowBgr24 = (ColorBgr24*)pRowBgra32;

                this.source.CopyPixels(new RectInt32(srcRect.x, srcRect.Top + dstY, srcRect.width, 1), pRowBgr24, bufferStride);
                for (int dstX = srcRect.width - 1; dstX >= 0; --dstX)
                {
                    pRowBgra32[dstX] = pRowBgr24[dstX];
                }

                pRowBgra32 = (ColorBgra32*)((byte*)pRowBgra32 + bufferStride);
            }
        }

        public IReadOnlyList<ColorBgra32>? GetPalette()
        {
            return null;
        }
    }
}
