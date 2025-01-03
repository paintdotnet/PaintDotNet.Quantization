﻿/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using System;

namespace PaintDotNet.Imaging
{
    /// <summary>
    /// Like IPixelInfo, but for pixels whose size is a multiple of 1 byte.
    /// So, not 4-bits or 10-bits. 8, 16, 24, 32, etc.
    /// </summary>
    public interface INaturalPixelInfo
        : IPixelInfo
    {
        int BytesPerPixel
        {
            get;
        }
    }
}
