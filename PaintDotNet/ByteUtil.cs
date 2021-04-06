using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PaintDotNet
{
    public static class ByteUtil
    {
        private static readonly float[] byteToScalingFloatLookup;
        private static readonly GCHandle byteToScalingFloatLookupPin;
        private static readonly unsafe float* pByteToScalingFloatLookup; // size = 256

        static unsafe ByteUtil()
        {
            byteToScalingFloatLookup = new float[256];
            byteToScalingFloatLookupPin = GCHandle.Alloc(byteToScalingFloatLookup, GCHandleType.Pinned);
            pByteToScalingFloatLookup = (float*)byteToScalingFloatLookupPin.AddrOfPinnedObject();

            for (int i = 0; i <= 255; ++i)
            {
                pByteToScalingFloatLookup[i] = i / 255.0f;
            }
        }

        /// <summary>
        /// Returns (x / 255.0f). A lookup table is used for improved performance.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToScalingFloat(byte x)
        {
            unsafe
            {
                return pByteToScalingFloatLookup[x];
            }
        }
    }
}
