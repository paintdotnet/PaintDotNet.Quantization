using System;
using System.Runtime.CompilerServices;

namespace PaintDotNet.Imaging
{
    internal static class ColorComponentUtil
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort Convert8UTo16U(byte c8U)
        {
            return unchecked((ushort)((ushort)c8U | ((ushort)c8U << 8)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Convert16UTo8U(ushort c16U)
        {
            return unchecked((byte)(c16U >> 8));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Convert8UTo32F(byte c8U)
        {
            //return ByteUtil.ToScalingFloat(c8U); // this uses a lookup table
            return c8U / 255.0f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Convert16UTo32F(ushort c16U)
        {
            return (float)c16U / 65535.0f;
        }
    }
}
