/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using PaintDotNet.Rendering;
using System;

namespace PaintDotNet.Imaging
{
    public static class BitmapSourceExtensions
    {
        public static unsafe void CopyPixels<TPixel>(this IBitmapSource<TPixel> source, RectInt32? srcRect, IBitmapLock<TPixel> buffer)
            where TPixel : unmanaged, INaturalPixelInfo<TPixel>
        {
            source.CopyPixels(srcRect, buffer.Buffer, buffer.Stride);
        }
    }
}
