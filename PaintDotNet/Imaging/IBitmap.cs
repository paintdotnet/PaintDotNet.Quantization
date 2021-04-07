/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

#nullable enable

using PaintDotNet.Rendering;
using System;

namespace PaintDotNet.Imaging
{
    /// <summary>
    /// Provides a managed projection of the IWICBitmap interface: https://docs.microsoft.com/en-us/windows/win32/api/wincodec/nn-wincodec-iwicbitmap
    /// </summary>
    /// <remarks>
    /// In the real Paint.NET codebase, this is properly routed to WIC via a complicated interop system.
    /// For this example repo, we just adapt it over the top of GDI+.
    /// </remarks>
    public interface IBitmap
        : IBitmapSource
    {
        IBitmapLock Lock(RectInt32 rect, BitmapLockOptions options);
    }
}
