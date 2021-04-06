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
    public struct ColorBgr32
        : IEquatable<ColorBgr32>,
          INaturalPixelInfo<ColorBgr32>
    {
        [FieldOffset(0)]
        internal byte b;

        [FieldOffset(1)]
        internal byte g;

        [FieldOffset(2)]
        internal byte r;

        [FieldOffset(3)]
        internal byte x;

        [NonSerialized]
        [FieldOffset(0)]
        internal uint bgrx;

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

        public byte X
        {
            get => this.x;
            set => this.x = value;
        }

        public uint Bgrx
        {
            get => this.bgrx;
            set => this.bgrx = value;
        }

        public static implicit operator ColorBgr32(ColorBgr24 color)
        {
            return FromBgr(color.B, color.G, color.R);
        }

        public static explicit operator ColorBgr32(ColorBgra32 color)
        {
            return Unsafe.As<ColorBgra32, ColorBgr32>(ref color);
        }

        public static ColorBgr32 FromBgrx(byte b, byte g, byte r, byte x)
        {
            ColorBgr32 color = new ColorBgr32();
            color.b = b;
            color.g = g;
            color.r = r;
            color.x = x;
            return color;
        }

        public static ColorBgr32 FromBgr(byte b, byte g, byte r)
        {
            ColorBgr32 color = new ColorBgr32();
            color.b = b;
            color.g = g;
            color.r = r;
            color.x = 255;
            return color;
        }

        public static ColorBgr32 FromUInt32(uint bgrx)
        {
            return Unsafe.As<uint, ColorBgr32>(ref bgrx);
        }

        public static ColorBgr32 Ceiling(ColorRgb96Float color)
        {
            return FromBgr(
                (byte)FloatUtil.Clamp((float)Math.Ceiling(color.B * 255.0f), 0, 255.0f),
                (byte)FloatUtil.Clamp((float)Math.Ceiling(color.G * 255.0f), 0, 255.0f),
                (byte)FloatUtil.Clamp((float)Math.Ceiling(color.R * 255.0f), 0, 255.0f));
        }

        public static ColorBgr32 Floor(ColorRgb96Float color)
        {
            return FromBgr(
                (byte)FloatUtil.Clamp((float)Math.Floor(color.B * 255.0f), 0, 255.0f),
                (byte)FloatUtil.Clamp((float)Math.Floor(color.G * 255.0f), 0, 255.0f),
                (byte)FloatUtil.Clamp((float)Math.Floor(color.R * 255.0f), 0, 255.0f));
        }

        public static ColorBgr32 Round(ColorRgb96Float color, MidpointRounding mode = MidpointRounding.AwayFromZero)
        {
            return FromBgr(
                (byte)FloatUtil.Clamp((float)Math.Round(color.B * 255.0f, mode), 0, 255.0f),
                (byte)FloatUtil.Clamp((float)Math.Round(color.G * 255.0f, mode), 0, 255.0f),
                (byte)FloatUtil.Clamp((float)Math.Round(color.R * 255.0f, mode), 0, 255.0f));
        }

        public static ColorBgr32 Truncate(ColorRgb96Float color)
        {
            return FromBgr(
                (byte)FloatUtil.Clamp((float)Math.Truncate(color.B * 255.0f), 0, 255.0f),
                (byte)FloatUtil.Clamp((float)Math.Truncate(color.G * 255.0f), 0, 255.0f),
                (byte)FloatUtil.Clamp((float)Math.Truncate(color.R * 255.0f), 0, 255.0f));
        }

        public bool Equals(ColorBgr32 other)
        {
            return this.bgrx == other.bgrx;
        }

        public override bool Equals(object obj)
        {
            return EquatableUtil.Equals(this, obj);
        }

        public static bool operator ==(ColorBgr32 a, ColorBgr32 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(ColorBgr32 a, ColorBgr32 b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return unchecked((int)this.bgrx);
        }

        PixelFormat INaturalPixelInfo.PixelFormat => PixelFormat.Bgr32;

        int INaturalPixelInfo.BytesPerPixel => 4;

        int IPixelInfo.BitsPerPixel => 32;
    }
}
