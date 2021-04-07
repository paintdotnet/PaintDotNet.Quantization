/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;

namespace PaintDotNet.Imaging
{
    public interface IImagingFactory
    {
        bool CanConvertFormat(
            PixelFormat srcFormat,
            PixelFormat dstFormat);

        IBitmap<TPixel> CreateBitmap<TPixel>(int width, int height)
            where TPixel : unmanaged, INaturalPixelInfo<TPixel>;

        IBitmapSource CreateFormatConvertedBitmap(
            IBitmapSource source,
            PixelFormat dstFormat);

        IBitmap LoadBitmapFromStream(Stream stream);
    }
}
