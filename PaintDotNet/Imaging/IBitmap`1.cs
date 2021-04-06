/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using PaintDotNet.Rendering;

namespace PaintDotNet.Imaging
{
    public interface IBitmap<TPixel>
        : IBitmapSource<TPixel>
          where TPixel : unmanaged, INaturalPixelInfo<TPixel>
    {
        IBitmapLock<TPixel> Lock(RectInt32 rect, BitmapLockOptions options);
    }
}
