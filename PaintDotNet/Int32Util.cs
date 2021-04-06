/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.CompilerServices;

namespace PaintDotNet
{
    public static class Int32Util
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int Log2Floor(int x)
        {
            int num = 0;

            while (x >= 1)
            {
                num++;
                x >>= 1;
            }

            return num;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte ClampToByte(int x)
        {
            if (x > 255)
            {
                return 255;
            }
            else if (x < 0)
            {
                return 0;
            }
            else
            {
                return (byte)x;
            }
        }
    }
}
