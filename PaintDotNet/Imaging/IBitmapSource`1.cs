/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using PaintDotNet.Rendering;
using System;

namespace PaintDotNet.Imaging
{
    public interface IBitmapSource<TPixel>
        : IDisposable
          where TPixel : unmanaged, INaturalPixelInfo<TPixel>
    {
        PixelFormat PixelFormat
        {
            get;
        }

        SizeInt32 Size
        {
            get;
        }

        unsafe void CopyPixels(RectInt32? srcRect, TPixel* pBuffer, int bufferStride);
    }
}
