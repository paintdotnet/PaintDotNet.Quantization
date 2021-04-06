/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using System;

namespace PaintDotNet.Imaging
{
    public static class Colors
    {
        public static ColorBgra32 Black => ColorBgra32.FromUInt32(0xFF000000);

        public static ColorBgra32 TransparentBlack => default;

        public static ColorBgra32 Zero => default;
    }
}
