/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using System;

namespace PaintDotNet.Imaging
{
    public interface IImagingFactory
    {
        IBitmap<TPixel> CreateBitmap<TPixel>(int width, int height)
            where TPixel : unmanaged, INaturalPixelInfo<TPixel>;
    }
}
