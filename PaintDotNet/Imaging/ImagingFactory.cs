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
    using GdipImageFormat = System.Drawing.Imaging.ImageFormat;
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
                if (default(TPixel).PixelFormat == PixelFormat.Indexed8)
                {
                    // we need to add a way to set the palette
                    throw new ArgumentException("Indexed8 isn't yet supported");
                }

                return new GdipBitmap<TPixel>(width, height);
            }

            public IBitmap LoadBitmapFromStream(Stream stream)
            {
                Image image = Bitmap.FromStream(stream);
                return CreateBitmapFrom(image);
            }

            public unsafe void SaveBitmapToStream(Stream stream, IBitmap bitmap, ImageFormat format)
            {
                if (bitmap is GdipBitmap ourBitmap)
                {
                    GdipImageFormat gdipFormat;
                    switch (format)
                    {
                        case ImageFormat.Png:
                            gdipFormat = GdipImageFormat.Png;
                            break;

                        default:
                            throw new InvalidEnumArgumentException();
                    }

                    using (IBitmapLock bitmapLock = bitmap.Lock(BitmapLockOptions.Read))
                    {
                        using Bitmap alias = new Bitmap(
                            bitmap.Size.Width, 
                            bitmap.Size.Height, 
                            bitmapLock.Stride, 
                            GetGdipPixelFormat(bitmap.PixelFormat), 
                            (IntPtr)bitmapLock.Buffer);

                        alias.Save(stream, gdipFormat);
                    }
                }
                else
                {
                    throw new ArgumentException();
                }
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

            private static PixelFormat GetPixelFormat(GdipPixelFormat pixelFormat)
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

            private static GdipPixelFormat GetGdipPixelFormat(PixelFormat pixelFormat)
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

            private static IBitmap CreateBitmapFrom(Image image)
            {
                if (image is Bitmap asBitmap)
                {
                    return CreateBitmapFrom(asBitmap);
                }
                else
                {
                    Bitmap bitmap = new Bitmap(image);
                    return CreateBitmapFrom(bitmap);
                }
            }

            private static IBitmap CreateBitmapFrom(Bitmap bitmap)
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

            private abstract class GdipBitmap
                : Disposable,
                  IBitmap
            {
                private Bitmap gdipBitmap;
                private BitmapData gdipBitmapLock;
                private PixelFormat pixelFormat;
                private int sizeOfPixel;

                public GdipBitmap(int width, int height, PixelFormat pixelFormat, GdipPixelFormat gdipPixelFormat)
                    : this(new Bitmap(width, height, gdipPixelFormat), pixelFormat)
                {
                }

                public GdipBitmap(Bitmap gdipBitmap, PixelFormat pixelFormat)
                {
                    this.gdipBitmap = gdipBitmap;

                    this.gdipBitmapLock = this.gdipBitmap.LockBits(
                        new Rectangle(0, 0, gdipBitmap.Width, gdipBitmap.Height),
                        ImageLockMode.ReadWrite,
                        this.gdipBitmap.PixelFormat);

                    this.pixelFormat = pixelFormat;
                    this.sizeOfPixel = this.pixelFormat.GetBytesPerPixel();
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

                public Bitmap Bitmap => this.gdipBitmap;

                protected BitmapData BitmapLock => this.gdipBitmapLock;

                public SizeInt32 Size => new SizeInt32(this.Bitmap.Width, this.Bitmap.Height);

                public PixelFormat PixelFormat => this.pixelFormat;

                public unsafe void CopyPixels(RectInt32? srcRect, void* pBuffer, int bufferStride)
                {
                    CopyPixelsImpl(srcRect ?? new RectInt32(Point2Int32.Zero, this.Size), pBuffer, bufferStride);
                }

                private unsafe void CopyPixelsImpl(RectInt32 srcRect, void* pBuffer, int bufferStride)
                {
                    int bytesPerRow = srcRect.width * this.sizeOfPixel;
                    void* pSrcRow = (byte*)this.BitmapLock.Scan0 + ((long)srcRect.y * this.BitmapLock.Stride) + (srcRect.x * this.sizeOfPixel);
                    void* pBufferRow = pBuffer;

                    for (int dstY = 0; dstY < srcRect.height; ++dstY)
                    {
                        BufferUtil.Copy(pBufferRow, pSrcRow, bytesPerRow);

                        pSrcRow = (byte*)pSrcRow + this.BitmapLock.Stride;
                        pBufferRow = (byte*)pBufferRow + bufferStride;
                    }
                }

                public IReadOnlyList<ColorBgra32>? GetPalette()
                {
                    if (this.PixelFormat != PixelFormat.Indexed8)
                    {
                        return null;
                    }

                    Color[] entries = this.Bitmap.Palette.Entries;
                    ColorBgra32[] palette = new ColorBgra32[entries.Length];
                    for (int i = 0; i < palette.Length; ++i)
                    {
                        Color entry = entries[i];
                        palette[i] = ColorBgra32.FromBgra(entry.B, entry.G, entry.R, entry.A);
                    }

                    return palette;
                }

                public virtual IBitmapLock Lock(RectInt32 rect, BitmapLockOptions options)
                {
                    return new GdipBitmapDataAsLock(this, this.gdipBitmapLock, rect);
                }
            }

            private sealed class GdipBitmap<TPixel>
                : GdipBitmap,
                  IBitmap<TPixel>
                  where TPixel : unmanaged, INaturalPixelInfo<TPixel>
            {
                private static readonly GdipPixelFormat gdipPixelFormat = GetGdipPixelFormat(default(TPixel).PixelFormat);

                public GdipBitmap(int width, int height)
                    : base(width, height, default(TPixel).PixelFormat, gdipPixelFormat)
                {
                }

                public GdipBitmap(Bitmap bitmap)
                    : base(bitmap, default(TPixel).PixelFormat)
                {
                }

                public override IBitmapLock Lock(RectInt32 rect, BitmapLockOptions options)
                {
                    return new GdipBitmapDataAsLock<TPixel>(this, this.BitmapLock, rect);
                }

                IBitmapLock<TPixel> IBitmap<TPixel>.Lock(RectInt32 rect, BitmapLockOptions optionsS)
                {
                    return new GdipBitmapDataAsLock<TPixel>(this, this.BitmapLock, rect);
                }
            }

            private unsafe class GdipBitmapDataAsLock
                : Disposable,
                  IBitmapLock
            {
                private object keepAlive;
                private BitmapData gdipBitmapLock;
                private RectInt32 rect;
                private int sizeOfPixel;

                public GdipBitmapDataAsLock(object keepAlive, BitmapData gdipBitmapLock, RectInt32 rect)
                {
                    this.keepAlive = keepAlive;
                    this.gdipBitmapLock = gdipBitmapLock;
                    this.rect = rect;

                    PixelFormat pixelFormat = GetPixelFormat(gdipBitmapLock.PixelFormat);
                    this.sizeOfPixel = pixelFormat.GetBytesPerPixel();
                }

                protected override void Dispose(bool disposing)
                {
                    this.keepAlive = null!;
                    this.gdipBitmapLock = null!;
                    base.Dispose(disposing);
                }

                public SizeInt32 Size => this.rect.Size;

                public int Stride => this.gdipBitmapLock.Stride;

                public void* Buffer => ((byte*)this.gdipBitmapLock.Scan0 + ((long)this.rect.Top * this.Stride)) + (this.rect.Left * this.sizeOfPixel);
            }

            private unsafe sealed class GdipBitmapDataAsLock<TPixel>
                : GdipBitmapDataAsLock,
                  IBitmapLock<TPixel>
                  where TPixel : unmanaged, INaturalPixelInfo<TPixel>
            {
                public GdipBitmapDataAsLock(object keepAlive, BitmapData gdipBitmapLock, RectInt32 rect)
                    : base(keepAlive, gdipBitmapLock, rect)
                {
                }

                public new TPixel* Buffer => (TPixel*)base.Buffer;
            }
        }
    }
}
