/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using PaintDotNet.Rendering;
using System;

namespace PaintDotNet.Imaging
{
    public interface IBitmap<TPixel>
        : IBitmap,
          IBitmapSource<TPixel>
          where TPixel : unmanaged, INaturalPixelInfo<TPixel>
    {
        new IBitmapLock<TPixel> Lock(RectInt32 rect, BitmapLockOptions options);
    }
}
