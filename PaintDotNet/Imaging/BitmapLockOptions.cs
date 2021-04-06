using System;

namespace PaintDotNet.Imaging
{
    public enum BitmapLockOptions
    {
        Read = 1,
        Write = 2,
        ReadWrite = Read | Write
    }
}
