/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using System;

namespace PaintDotNet.Imaging
{
    public unsafe interface IBitmapLock<TPixel>
        : IBitmapLock
          where TPixel : unmanaged, INaturalPixelInfo<TPixel>
    {
        new TPixel* Buffer
        {
            get;
        }
    }
}
