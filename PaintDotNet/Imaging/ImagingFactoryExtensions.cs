/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using PaintDotNet.Rendering;
using System;

namespace PaintDotNet.Imaging
{
    public static class ImagingFactoryExtensions
    {
        public static IBitmap<TPixel> CreateBitmap<TPixel>(this IImagingFactory factory, SizeInt32 size)
            where TPixel : unmanaged, INaturalPixelInfo<TPixel>
        {
            return factory.CreateBitmap<TPixel>(size.width, size.height);
        }

        public static IBitmapSource<TPixel> CreateFormatConvertedBitmap<TPixel>(this IImagingFactory factory, IBitmapSource source)
            where TPixel : unmanaged, INaturalPixelInfo<TPixel>
        {
            return (IBitmapSource<TPixel>)factory.CreateFormatConvertedBitmap(source, default(TPixel).PixelFormat);
        }
    }
}
