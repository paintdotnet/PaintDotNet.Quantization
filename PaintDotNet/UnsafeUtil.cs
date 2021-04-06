using System;
using System.Runtime.CompilerServices;

namespace PaintDotNet
{
    public static class UnsafeUtil
    {
        public static unsafe void CopyBlockUnaligned(void* destination, void* source, long byteCount)
        {
            CopyBlockUnaligned(destination, source, checked((ulong)byteCount));
        }

        public static unsafe void CopyBlockUnaligned(void* destination, void* source, ulong byteCount)
        {
            /*Validate.Begin()
                    .IsNotNull(destination, nameof(destination))
                    .IsNotNull(source, nameof(source))
                    .Check();*/

            ulong bytesLeft = byteCount;
            byte* pDst = (byte*)destination;
            byte* pSrc = (byte*)source;
            while (bytesLeft > 0)
            {
                uint byteCount32 = (uint)Math.Min(bytesLeft, uint.MaxValue);
                Unsafe.CopyBlockUnaligned(pDst, pSrc, byteCount32);
                pDst += byteCount32;
                pSrc += byteCount32;
                bytesLeft -= byteCount32;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void InitBlockUnaligned(void* startAddress, byte value, long byteCount)
        {
            InitBlockUnaligned(startAddress, value, checked((ulong)byteCount));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void InitBlockUnaligned(void* startAddress, byte value, ulong byteCount)
        {
            //Validate.IsNotNull(startAddress, nameof(startAddress));

            ulong bytesLeft = byteCount;
            byte* p = (byte*)startAddress;
            while (bytesLeft > 0)
            {
                uint byteCount32 = (uint)Math.Min(bytesLeft, uint.MaxValue);
                Unsafe.InitBlockUnaligned(p, value, byteCount32);
                p += byteCount32;
                bytesLeft -= byteCount32;
            }
        }
    }
}
