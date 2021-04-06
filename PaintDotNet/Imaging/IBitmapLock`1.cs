using PaintDotNet.Rendering;
using System;

namespace PaintDotNet.Imaging
{
    public unsafe interface IBitmapLock<TPixel>
        : IDisposable
          where TPixel : unmanaged, INaturalPixelInfo<TPixel>
    {
        SizeInt32 Size
        {
            get;
        }

        TPixel* Buffer
        {
            get;
        }

        int Stride
        {
            get;
        }
    }
}
