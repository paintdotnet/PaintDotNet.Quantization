/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.CompilerServices;

namespace PaintDotNet
{
    public static class BufferUtil
    {
        [Obsolete("Use Clear() instead. This method will be removed soon.", true)]
        public static unsafe void ZeroMemory(byte* pBuffer, long length)
        {
            Clear(pBuffer, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Clear(void* startAddress, int byteCount)
        {
            Unsafe.InitBlockUnaligned(startAddress, 0, checked((uint)byteCount));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Clear(void* startAddress, uint byteCount)
        {
            Unsafe.InitBlockUnaligned(startAddress, 0, byteCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Clear(void* startAddress, long byteCount)
        {
            UnsafeUtil.InitBlockUnaligned(startAddress, 0, byteCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Clear(void* startAddress, ulong byteCount)
        {
            UnsafeUtil.InitBlockUnaligned(startAddress, 0, byteCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Copy(void* dst, void* src, int byteCount)
        {
            Buffer.MemoryCopy(src, dst, byteCount, byteCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Copy(void* dst, void* src, uint byteCount)
        {
            Buffer.MemoryCopy(src, dst, byteCount, byteCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Copy(void* dst, void* src, long byteCount)
        {
            Buffer.MemoryCopy(src, dst, byteCount, byteCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Copy(void* dst, void* src, ulong byteCount)
        {
            Buffer.MemoryCopy(src, dst, byteCount, byteCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Fill(void* startAddress, byte value, int byteCount)
        {
            Unsafe.InitBlockUnaligned(startAddress, value, checked((uint)byteCount));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Fill(void* startAddress, byte value, uint byteCount)
        {
            Unsafe.InitBlockUnaligned(startAddress, value, byteCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Fill(void* startAddress, byte value, long byteCount)
        {
            UnsafeUtil.InitBlockUnaligned(startAddress, value, byteCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Fill(void* startAddress, byte value, ulong byteCount)
        {
            UnsafeUtil.InitBlockUnaligned(startAddress, value, byteCount);
        }
    }
}
