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
    public struct ColorBgr24
        : IEquatable<ColorBgr24>,
          INaturalPixelInfo<ColorBgr24>
    {
        [FieldOffset(0)]
        internal byte b;

        [FieldOffset(1)]
        internal byte g;

        [FieldOffset(2)]
        internal byte r;

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

        public int Bgr
        {
            get => this.b | (this.g << 8) | (this.r << 16);
        }

        public static explicit operator ColorBgr24(ColorBgr32 color)
        {
            return ColorBgr24.FromBgr(color.B, color.G, color.R);
        }

        public static ColorBgr24 FromBgr(byte b, byte g, byte r)
        {
            ColorBgr24 color = new ColorBgr24();
            color.b = b;
            color.g = g;
            color.r = r;
            return color;
        }

        public static ColorBgr24 Ceiling(ColorRgb96Float color)
        {
            return FromBgr(
                (byte)FloatUtil.Clamp((float)Math.Ceiling(color.B * 255.0f), 0, 255.0f),
                (byte)FloatUtil.Clamp((float)Math.Ceiling(color.G * 255.0f), 0, 255.0f),
                (byte)FloatUtil.Clamp((float)Math.Ceiling(color.R * 255.0f), 0, 255.0f));
        }

        public static ColorBgr24 Floor(ColorRgb96Float color)
        {
            return FromBgr(
                (byte)FloatUtil.Clamp((float)Math.Floor(color.B * 255.0f), 0, 255.0f),
                (byte)FloatUtil.Clamp((float)Math.Floor(color.G * 255.0f), 0, 255.0f),
                (byte)FloatUtil.Clamp((float)Math.Floor(color.R * 255.0f), 0, 255.0f));
        }

        public static ColorBgr24 Round(ColorRgb96Float color, MidpointRounding mode = MidpointRounding.AwayFromZero)
        {
            return FromBgr(
                (byte)FloatUtil.Clamp((float)Math.Round(color.B * 255.0f, mode), 0, 255.0f),
                (byte)FloatUtil.Clamp((float)Math.Round(color.G * 255.0f, mode), 0, 255.0f),
                (byte)FloatUtil.Clamp((float)Math.Round(color.R * 255.0f, mode), 0, 255.0f));
        }

        public static ColorBgr24 Truncate(ColorRgb96Float color)
        {
            return FromBgr(
                (byte)FloatUtil.Clamp((float)Math.Truncate(color.B * 255.0f), 0, 255.0f),
                (byte)FloatUtil.Clamp((float)Math.Truncate(color.G * 255.0f), 0, 255.0f),
                (byte)FloatUtil.Clamp((float)Math.Truncate(color.R * 255.0f), 0, 255.0f));
        }

        public bool Equals(ColorBgr24 other)
        {
            return this.Bgr == other.Bgr;
        }

        public override bool Equals(object obj)
        {
            return EquatableUtil.Equals(this, obj);
        }

        public static bool operator ==(ColorBgr24 a, ColorBgr24 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(ColorBgr24 a, ColorBgr24 b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return this.Bgr;
        }

        public override string ToString()
        {
            return "#" + this.Bgr.ToString("X6");
        }

        PixelFormat IPixelInfo.PixelFormat => PixelFormat.Bgr24;

        int IPixelInfo.BitsPerPixel => 24;

        int INaturalPixelInfo.BytesPerPixel => 3;
    }
}
