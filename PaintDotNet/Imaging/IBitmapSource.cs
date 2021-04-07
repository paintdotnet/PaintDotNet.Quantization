/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

#nullable enable

using PaintDotNet.Rendering;
using System;
using System.Collections.Generic;

namespace PaintDotNet.Imaging
{
    /// <summary>
    /// Provides a managed projection of the IWICBitmapSource interface: https://docs.microsoft.com/en-us/windows/win32/api/wincodec/nn-wincodec-iwicbitmapsource
    /// </summary>
    /// <remarks>
    /// In the real Paint.NET codebase, this is properly routed to WIC via a complicated interop system.
    /// For this example repo, we just adapt it over the top of GDI+.
    /// </remarks>
    public interface IBitmapSource
        : IDisposable
    {
        PixelFormat PixelFormat
        {
            get;
        }

        SizeInt32 Size
        {
            get;
        }

        unsafe void CopyPixels(RectInt32? srcRect, void* pBuffer, int bufferStride);

        // Only returns non-null for indexed pixel formats
        IReadOnlyList<ColorBgra32>? GetPalette();
    }
}
