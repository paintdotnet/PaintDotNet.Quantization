using PaintDotNet.Rendering;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;

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
            public IBitmap<TPixel> CreateBitmap<TPixel>(int width, int height) 
                where TPixel : unmanaged, INaturalPixelInfo<TPixel>
            {
                GdipPixelFormat gdipPixelFormat = GetGdipPixelFormat(default(TPixel).PixelFormat);
                return new GdipBitmap<TPixel>(width, height, gdipPixelFormat);
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
                        throw new InvalidEnumArgumentException();
                }
            }
        }

        private sealed class GdipBitmap<TPixel>
            : Disposable,
              IBitmap<TPixel>
              where TPixel : unmanaged, INaturalPixelInfo<TPixel>
        {
            private Bitmap gdipBitmap;
            private BitmapData gdipBitmapLock;

            public GdipBitmap(int width, int height, GdipPixelFormat gdipPixelFormat)
            {
                this.gdipBitmap = new Bitmap(width, height, gdipPixelFormat);
                this.gdipBitmapLock = this.gdipBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, gdipPixelFormat);
            }

            public PixelFormat PixelFormat => GdipImagingFactory.GetPixelFormat(this.gdipBitmap.PixelFormat);

            public SizeInt32 Size => new SizeInt32(this.gdipBitmap.Width, this.gdipBitmap.Height);

            public unsafe void CopyPixels(RectInt32? srcRect, TPixel* pBuffer, int bufferStride)
            {
                CopyPixelsImpl(srcRect ?? new RectInt32(Point2Int32.Zero, this.Size), pBuffer, bufferStride);
            }

            private unsafe void CopyPixelsImpl(RectInt32 srcRect, TPixel* pBuffer, int bufferStride)
            {
                int bytesPerRow = srcRect.width * sizeof(TPixel);
                TPixel* pSrcRow = (TPixel*)((byte*)this.gdipBitmapLock.Scan0 + ((long)srcRect.y * this.gdipBitmapLock.Stride));
                TPixel* pBufferRow = pBuffer;
                for (int dstY = 0; dstY < srcRect.height; ++dstY)
                {
                    BufferUtil.Copy(pBufferRow, pSrcRow + srcRect.x, bytesPerRow);
                    pSrcRow = (TPixel*)((byte*)pSrcRow + this.gdipBitmapLock.Stride);
                    pBufferRow = (TPixel*)((byte*)pBufferRow + bufferStride);
                }
            }

            public IBitmapLock<TPixel> Lock(RectInt32 rect, BitmapLockOptions options)
            {
                return new GdipBitmapDataAsLock<TPixel>(this.gdipBitmapLock, rect);
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    this.gdipBitmap.UnlockBits(this.gdipBitmapLock);
                    this.gdipBitmapLock = null;
                }

                DisposableUtil.Free(ref this.gdipBitmap, disposing);

                base.Dispose(disposing);
            }
        }

        private unsafe sealed class GdipBitmapDataAsLock<TPixel>
            : Disposable,
              IBitmapLock<TPixel>
              where TPixel : unmanaged, INaturalPixelInfo<TPixel>
        {
            private BitmapData gdipBitmapLock;
            private RectInt32 rect;

            public GdipBitmapDataAsLock(BitmapData gdipBitmapLock, RectInt32 rect)
            {
                this.gdipBitmapLock = gdipBitmapLock;
                this.rect = rect;
            }

            public SizeInt32 Size => this.rect.Size;

            public int Stride => this.gdipBitmapLock.Stride;

            public TPixel* Buffer => (TPixel*)((byte*)this.gdipBitmapLock.Scan0 + ((long)this.rect.Y * this.Stride));

            protected override void Dispose(bool disposing)
            {
                this.gdipBitmapLock = null;
                base.Dispose(disposing);
            }
        }
    }
}
