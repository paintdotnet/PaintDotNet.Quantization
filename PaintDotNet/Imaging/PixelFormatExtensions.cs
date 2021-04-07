/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel;

namespace PaintDotNet.Imaging
{
    public static class PixelFormatExtensions
    {
        public static int GetBytesPerPixel(this PixelFormat format)
        {
            switch (format)
            {
                case PixelFormat.Indexed8:
                    return 1;

                case PixelFormat.Bgr24:
                    return 3;

                case PixelFormat.Bgr32:
                    return 4;

                case PixelFormat.Bgr48:
                    return 6;

                case PixelFormat.Bgra32:
                    return 4;

                case PixelFormat.Rgb96Float:
                    return 12;

                default:
                    throw new InvalidEnumArgumentException();
            }
        }
    }
}
