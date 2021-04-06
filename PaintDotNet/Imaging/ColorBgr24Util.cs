/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.CompilerServices;

namespace PaintDotNet.Imaging
{
    internal static class ColorBgr24Util
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetDistanceSquared(ColorBgr24 a, ColorBgr24 b)
        {
            int deltaB = a.B - b.B;
            int deltaG = a.G - b.G;
            int deltaR = a.R - b.R;
            return (deltaB * deltaB) + (deltaG * deltaG) + (deltaR * deltaR);
        }
    }
}
