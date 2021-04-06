/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PaintDotNet.Imaging
{
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct ColorBgra32
        : INaturalPixelInfo<ColorBgra32>,
          IEquatable<ColorBgra32>
    {
        [FieldOffset(0)]
        internal byte b;

        [FieldOffset(1)]
        internal byte g;

        [FieldOffset(2)]
        internal byte r;

        [FieldOffset(3)]
        internal byte a;

        [NonSerialized]
        [FieldOffset(0)]
        internal uint bgra;

        public byte B
        {
            get => this.b;
            set => this.b = value;
        }

        public byte G
        {
            get => this.g;
            set => this.g = value;
        }

        public byte R
        {
            get => this.r;
            set => this.r = value;
        }

        public byte A
        {
            get => this.a;
            set => this.a = value;
        }

        public uint Bgra
        {
            get => this.bgra;
            set => this.bgra = value;
        }

        public static implicit operator ColorBgra32(ColorBgr24 color)
        {
            return (ColorBgra32)(ColorBgr32)color;
        }

        public static implicit operator ColorBgra32(ColorBgr32 color)
        {
            return Unsafe.As<ColorBgr32, ColorBgra32>(ref color);
        }

        public static ColorBgra32 FromBgra(byte b, byte g, byte r, byte a)
        {
            ColorBgra32 color = new ColorBgra32();
            color.b = b;
            color.g = g;
            color.r = r;
            color.a = a;
            return color;
        }

        public static ColorBgra32 FromUInt32(uint bgra)
        {
            return Unsafe.As<uint, ColorBgra32>(ref bgra);
        }

        public bool Equals(ColorBgra32 other)
        {
            return this.bgra == other.bgra;
        }

        public override bool Equals(object obj)
        {
            return EquatableUtil.Equals(this, obj);
        }

        public static bool operator ==(ColorBgra32 a, ColorBgra32 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(ColorBgra32 a, ColorBgra32 b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return unchecked((int)this.bgra);
        }

        public override string ToString()
        {
            return "#" + this.bgra.ToString("X8");
        }

        PixelFormat INaturalPixelInfo.PixelFormat => PixelFormat.Bgra32;

        int INaturalPixelInfo.BytesPerPixel => 4;

        int IPixelInfo.BitsPerPixel => 32;
    }
}
