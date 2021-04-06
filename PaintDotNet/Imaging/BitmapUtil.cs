/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using PaintDotNet.Rendering;
using System;
using System.Diagnostics;

namespace PaintDotNet.Imaging
{
    public static class BitmapUtil
    {
        public static uint GetBufferSizeChecked(int width, int height, int stride, int bitsPerPixel)
        {
            return checked((uint)((((long)height - 1) * (long)stride) + ((((long)width * (long)bitsPerPixel) + 7) / 8)));
        }

        /*
        public static uint GetBufferSizeChecked(int width, int height, int stride, PixelFormat pixelFormat)
        {
            return GetBufferSizeChecked(width, height, stride, pixelFormat.GetBitsPerPixel());
        }
        */

        public static uint GetBufferSizeChecked<TPixel>(int width, int height, int stride)
            where TPixel : struct, IPixelInfo
        {
            return GetBufferSizeChecked(width, height, stride, default(TPixel).BitsPerPixel);
        }
    }
}
