using System;

namespace PaintDotNet.Imaging
{
    public interface IPixelInfo
    {
        int BitsPerPixel
        {
            get;
        }

        /*
        PixelFormat PixelFormat
        {
            get;
        }*/
    }
}
