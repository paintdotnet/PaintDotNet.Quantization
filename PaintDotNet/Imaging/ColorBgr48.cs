/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

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
    [StructLayout(LayoutKind.Explicit)]
    public struct ColorBgr48
        : INaturalPixelInfo<ColorBgr48>,
          IEquatable<ColorBgr48>
    {
        [FieldOffset(0)]
        internal ushort b;

        [FieldOffset(1)]
        internal ushort g;

        [FieldOffset(2)]
        internal ushort r;

        public ushort B
        {
            get => this.b;
            set => this.b = value;
        }

        public ushort G
        {
            get => this.g;
            set => this.g = value;
        }

        public ushort R
        {
            get => this.r;
            set => this.r = value;
        }

        public long Bgr
        {
            get => this.b | (this.g << 16) | (this.r << 32);
        }

        public static ColorBgr48 FromBgr(ushort b, ushort g, ushort r)
        {
            ColorBgr48 color = new ColorBgr48();
            color.b = b;
            color.g = g;
            color.r = r;
            return color;
        }

        public static ColorBgr48 Ceiling(ColorRgb96Float color)
        {
            return FromBgr(
                (byte)FloatUtil.Clamp((float)Math.Ceiling(color.B * 65535.0f), 0, 65535.0f),
                (byte)FloatUtil.Clamp((float)Math.Ceiling(color.G * 65535.0f), 0, 65535.0f),
                (byte)FloatUtil.Clamp((float)Math.Ceiling(color.R * 65535.0f), 0, 65535.0f));
        }

        public static ColorBgr48 Floor(ColorRgb96Float color)
        {
            return FromBgr(
                (byte)FloatUtil.Clamp((float)Math.Floor(color.B * 65535.0f), 0, 65535.0f),
                (byte)FloatUtil.Clamp((float)Math.Floor(color.G * 65535.0f), 0, 65535.0f),
                (byte)FloatUtil.Clamp((float)Math.Floor(color.R * 65535.0f), 0, 65535.0f));
        }

        public static ColorBgr48 Round(ColorRgb96Float color, MidpointRounding mode = MidpointRounding.AwayFromZero)
        {
            return FromBgr(
                (byte)FloatUtil.Clamp((float)Math.Round(color.B * 65535.0f, mode), 0, 65535.0f),
                (byte)FloatUtil.Clamp((float)Math.Round(color.G * 65535.0f, mode), 0, 65535.0f),
                (byte)FloatUtil.Clamp((float)Math.Round(color.R * 65535.0f, mode), 0, 65535.0f));
        }

        public static ColorBgr48 Truncate(ColorRgb96Float color)
        {
            return FromBgr(
                (byte)FloatUtil.Clamp((float)Math.Truncate(color.B * 65535.0f), 0, 65535.0f),
                (byte)FloatUtil.Clamp((float)Math.Truncate(color.G * 65535.0f), 0, 65535.0f),
                (byte)FloatUtil.Clamp((float)Math.Truncate(color.R * 65535.0f), 0, 65535.0f));
        }

        public bool Equals(ColorBgr48 other)
        {
            return this.Bgr == other.Bgr;
        }

        public override bool Equals(object obj)
        {
            return EquatableUtil.Equals(this, obj);
        }

        public static bool operator ==(ColorBgr48 a, ColorBgr48 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(ColorBgr48 a, ColorBgr48 b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return this.Bgr.GetHashCode();
        }

        PixelFormat IPixelInfo.PixelFormat => PixelFormat.Bgr48;

        int IPixelInfo.BitsPerPixel => 48;

        int INaturalPixelInfo.BytesPerPixel => 6;
    }
}
