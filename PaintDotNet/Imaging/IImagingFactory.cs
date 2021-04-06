using System;

namespace PaintDotNet.Imaging
{
    public interface IImagingFactory
    {
        IBitmap<TPixel> CreateBitmap<TPixel>(int width, int height)
            where TPixel : unmanaged, INaturalPixelInfo<TPixel>;
    }
}
