/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using PaintDotNet;
using System;
using System.Runtime.InteropServices;

namespace PaintDotNet.Imaging
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct ColorRgb96Float
         : INaturalPixelInfo<ColorRgb96Float>,
           IEquatable<ColorRgb96Float>
    {
        internal float r;
        internal float g;
        internal float b;

        public float R
        {
            get => this.r;
            set => this.r = value;
        }

        public float G
        {
            get => this.g;
            set => this.g = value;
        }

        public float B
        {
            get => this.b;
            set => this.b = value;
        }

        public ColorRgb96Float(float r, float g, float b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public static implicit operator ColorRgb96Float(ColorBgr24 color)
        {
            return new ColorRgb96Float(
                ByteUtil.ToScalingFloat(color.r),
                ByteUtil.ToScalingFloat(color.g),
                ByteUtil.ToScalingFloat(color.b));
        }

        public bool Equals(ColorRgb96Float other)
        {
            return this.r.Equals(other.r)
                && this.g.Equals(other.g)
                && this.b.Equals(other.b);
        }

        public override bool Equals(object obj)
        {
            return EquatableUtil.Equals(this, obj);
        }

        public static bool operator ==(ColorRgb96Float lhs, ColorRgb96Float rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(ColorRgb96Float lhs, ColorRgb96Float rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            return HashCodeUtil.CombineHashCodes(
                this.r.GetHashCode(),
                this.g.GetHashCode(),
                this.b.GetHashCode());
        }

        int INaturalPixelInfo.BytesPerPixel => 12;

        int IPixelInfo.BitsPerPixel => 96;

        PixelFormat IPixelInfo.PixelFormat => PixelFormat.Rgb96Float;
    }
}
