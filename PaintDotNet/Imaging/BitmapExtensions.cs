using PaintDotNet.Rendering;
using System;

namespace PaintDotNet.Imaging
{
    public static class BitmapExtensions
    {
        public static IBitmapLock<TPixel> Lock<TPixel>(this IBitmap<TPixel> bitmap, BitmapLockOptions options)
            where TPixel : unmanaged, INaturalPixelInfo<TPixel>
        {
            return bitmap.Lock(new RectInt32(Point2Int32.Zero, bitmap.Size), options);
        }
    }
}
