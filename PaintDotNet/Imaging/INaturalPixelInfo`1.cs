/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using System;

namespace PaintDotNet.Imaging
{
    public interface INaturalPixelInfo<TPixel>
        : INaturalPixelInfo
          where TPixel : unmanaged, INaturalPixelInfo<TPixel>
    {
    }
}
