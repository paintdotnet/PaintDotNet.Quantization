/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

#nullable enable

using PaintDotNet.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace PaintDotNet.Imaging
{
    using GdipPixelFormat = System.Drawing.Imaging.PixelFormat;

    public static class ImagingFactory
    {
        public static IImagingFactory Instance
        {
            get;
        } = new GdipImagingFactory();

        // Paint.NET actually uses WIC for bitmap allocation, but for this sample we will just adapt GDI+.

        private sealed class GdipImagingFactory
            : IImagingFactory
        {
            public bool CanConvertFormat(PixelFormat srcFormat, PixelFormat dstFormat)
            {
                if (srcFormat == dstFormat)
                {
                    // sure, we can do that
                    return true;
                }
                else if (srcFormat == PixelFormat.Bgr24 && dstFormat == PixelFormat.Bgra32)
                {
                    return true;
                }
                else if (srcFormat == PixelFormat.Indexed8 && dstFormat == PixelFormat.Bgra32)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public IBitmap<TPixel> CreateBitmap<TPixel>(int width, int height)
                where TPixel : unmanaged, INaturalPixelInfo<TPixel>
            {
                return new GdipBitmap<TPixel>(width, height);
            }

            public IBitmap LoadBitmapFromStream(Stream stream)
            {
                Image image = Bitmap.FromStream(stream);
                return GdipBitmap.From(image);
            }

            public IBitmapSource CreateFormatConvertedBitmap(IBitmapSource source, PixelFormat dstFormat)
            {
                PixelFormat srcFormat = source.PixelFormat;

                if (srcFormat == dstFormat)
                {
                    return source;
                }
                else if (srcFormat == PixelFormat.Bgr24 && dstFormat == PixelFormat.Bgra32)
                {
                    return new Bgr24ToBgra32FormatConverter((IBitmapSource<ColorBgr24>)source);
                }
                else if (srcFormat == PixelFormat.Indexed8 && dstFormat == PixelFormat.Bgra32)
                {
                    return new Indexed8ToBgra32FormatConverter((IBitmapSource<ColorIndexed8>)source);
                }

                throw new ArgumentException();
            }

            public static PixelFormat GetPixelFormat(GdipPixelFormat pixelFormat)
            {
                switch (pixelFormat)
                {
                    case GdipPixelFormat.Format8bppIndexed:
                        return PixelFormat.Indexed8;

                    case GdipPixelFormat.Format24bppRgb:
                        return PixelFormat.Bgr24;

                    case GdipPixelFormat.Format32bppRgb:
                        return PixelFormat.Bgr32;

                    case GdipPixelFormat.Format32bppArgb:
                        return PixelFormat.Bgra32;

                    default:
                        throw new InvalidEnumArgumentException();
                }
            }

            public static GdipPixelFormat GetGdipPixelFormat(PixelFormat pixelFormat)
            {
                switch (pixelFormat)
                {
                    case PixelFormat.Indexed8:
                        return GdipPixelFormat.Format8bppIndexed;

                    case PixelFormat.Bgr24:
                        return GdipPixelFormat.Format24bppRgb;

                    case PixelFormat.Bgr32:
                        return GdipPixelFormat.Format32bppRgb;

                    case PixelFormat.Bgra32:
                        return GdipPixelFormat.Format32bppArgb;

                    default:
                        throw new ArgumentException();
                }
            }

            private static class GdipBitmap
            {
                public static IBitmap From(Image image)
                {
                    if (image is Bitmap asBitmap)
                    {
                        return From(asBitmap);
                    }
                    else
                    {
                        Bitmap bitmap = new Bitmap(image);
                        return From(bitmap);
                    }
                }

                public static IBitmap From(Bitmap bitmap)
                {
                    switch (bitmap.PixelFormat)
                    {
                        case GdipPixelFormat.Format8bppIndexed:
                            return new GdipBitmap<ColorIndexed8>(bitmap);

                        case GdipPixelFormat.Format24bppRgb:
                            return new GdipBitmap<ColorBgr24>(bitmap);

                        case GdipPixelFormat.Format32bppRgb:
                            return new GdipBitmap<ColorBgr32>(bitmap);

                        case GdipPixelFormat.Format32bppArgb:
                            return new GdipBitmap<ColorBgra32>(bitmap);
                    }

                    throw new ArgumentException();
                }
            }

            private sealed class GdipBitmap<TPixel>
                : Disposable,
                  IBitmap<TPixel>
                  where TPixel : unmanaged, INaturalPixelInfo<TPixel>
            {
                private static readonly GdipPixelFormat gdipPixelFormat = GetGdipPixelFormat(default(TPixel).PixelFormat);

                private Bitmap gdipBitmap;
                private BitmapData gdipBitmapLock;

                public GdipBitmap(int width, int height)
                    : this(new Bitmap(width, height, gdipPixelFormat))
                {
                }

                public GdipBitmap(Bitmap gdipBitmap)
                {
                    this.gdipBitmap = gdipBitmap;

                    this.gdipBitmapLock = this.gdipBitmap.LockBits(
                        new Rectangle(0, 0, gdipBitmap.Width, gdipBitmap.Height), 
                        ImageLockMode.ReadWrite, 
                        gdipPixelFormat);
                }

                protected override void Dispose(bool disposing)
                {
                    if (disposing)
                    {
                        this.gdipBitmap.UnlockBits(this.gdipBitmapLock);
                        this.gdipBitmapLock = null!;
                    }

                    DisposableUtil.Free(ref this.gdipBitmap, disposing);

                    base.Dispose(disposing);
                }

                public PixelFormat PixelFormat => default(TPixel).PixelFormat;

                public SizeInt32 Size => new SizeInt32(this.gdipBitmap.Width, this.gdipBitmap.Height);

                public IReadOnlyList<ColorBgra32>? GetPalette()
                {
                    if (this.PixelFormat != PixelFormat.Indexed8)
                    {
                        return null;
                    }

                    Color[] entries = this.gdipBitmap.Palette.Entries;
                    ColorBgra32[] palette = new ColorBgra32[entries.Length];
                    for (int i = 0; i < palette.Length; ++i)
                    {
                        Color entry = entries[i];
                        palette[i] = ColorBgra32.FromBgra(entry.B, entry.G, entry.R, entry.A);
                    }

                    return palette;
                }

                public unsafe void CopyPixels(RectInt32? srcRect, void* pBuffer, int bufferStride)
                {
                    CopyPixelsImpl(srcRect ?? new RectInt32(Point2Int32.Zero, this.Size), pBuffer, bufferStride);
                }

                private unsafe void CopyPixelsImpl(RectInt32 srcRect, void* pBuffer, int bufferStride)
                {
                    int bytesPerRow = srcRect.width * sizeof(TPixel);
                    TPixel* pSrcRow = (TPixel*)((byte*)this.gdipBitmapLock.Scan0 + ((long)srcRect.y * this.gdipBitmapLock.Stride)) + srcRect.x;
                    TPixel* pBufferRow = (TPixel*)pBuffer;

                    for (int dstY = 0; dstY < srcRect.height; ++dstY)
                    {
                        BufferUtil.Copy(pBufferRow, pSrcRow, bytesPerRow);

                        pSrcRow = (TPixel*)((byte*)pSrcRow + this.gdipBitmapLock.Stride);
                        pBufferRow = (TPixel*)((byte*)pBufferRow + bufferStride);
                    }
                }

                public IBitmapLock<TPixel> Lock(RectInt32 rect, BitmapLockOptions options)
                {
                    return new GdipBitmapDataAsLock<TPixel>(this, this.gdipBitmapLock, rect);
                }

                IBitmapLock IBitmap.Lock(RectInt32 rect, BitmapLockOptions options)
                {
                    return Lock(rect, options);
                }
            }

            private unsafe sealed class GdipBitmapDataAsLock<TPixel>
                : Disposable,
                  IBitmapLock<TPixel>
                  where TPixel : unmanaged, INaturalPixelInfo<TPixel>
            {
                private object keepAlive;
                private BitmapData gdipBitmapLock;
                private RectInt32 rect;

                public GdipBitmapDataAsLock(object keepAlive, BitmapData gdipBitmapLock, RectInt32 rect)
                {
                    this.keepAlive = keepAlive;
                    this.gdipBitmapLock = gdipBitmapLock;
                    this.rect = rect;
                }

                protected override void Dispose(bool disposing)
                {
                    this.keepAlive = null!;
                    this.gdipBitmapLock = null!;
                    base.Dispose(disposing);
                }

                public SizeInt32 Size => this.rect.Size;

                public int Stride => this.gdipBitmapLock.Stride;

                public TPixel* Buffer => (TPixel*)((byte*)this.gdipBitmapLock.Scan0 + ((long)this.rect.Top * this.Stride)) + this.rect.Left;

                void* IBitmapLock.Buffer => this.Buffer;
            }
        }
    }
}
