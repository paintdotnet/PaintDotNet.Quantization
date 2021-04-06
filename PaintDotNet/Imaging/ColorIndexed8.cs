/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.InteropServices;

namespace PaintDotNet.Imaging
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ColorIndexed8
        : INaturalPixelInfo<ColorIndexed8>,
          IEquatable<ColorIndexed8>
    {
        public static ColorIndexed8 Zero => default;

        internal byte value;

        public byte Value
        {
            get => this.value;
            set => this.value = value;
        }

        public static explicit operator ColorIndexed8(byte value)
        {
            return new ColorIndexed8(value);
        }

        public static explicit operator byte(ColorIndexed8 color)
        {
            return color.value;
        }

        public ColorIndexed8(byte value)
        {
            this.value = value;
        }

        public bool Equals(ColorIndexed8 other)
        {
            return this.value == other.value;
        }

        public override bool Equals(object obj)
        {
            return EquatableUtil.Equals(this, obj);
        }

        public static bool operator ==(ColorIndexed8 a, ColorIndexed8 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(ColorIndexed8 a, ColorIndexed8 b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }

        PixelFormat IPixelInfo.PixelFormat => PixelFormat.Indexed8;

        int IPixelInfo.BitsPerPixel => 8;

        int INaturalPixelInfo.BytesPerPixel => 1;
    }
}
