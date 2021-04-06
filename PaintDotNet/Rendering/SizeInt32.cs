/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PaintDotNet.Rendering
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    [DebuggerDisplay("{width},{height}")]
    public struct SizeInt32
        : IEquatable<SizeInt32>
    {
        public static SizeInt32 Empty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new SizeInt32()
                {
                    width = int.MinValue,
                    height = int.MaxValue
                };
            }
        }

        public static SizeInt32 Max
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new SizeInt32(int.MaxValue, int.MaxValue);
            }
        }

        public static SizeInt32 Zero
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new SizeInt32(0, 0);
            }
        }

        // SizeInt32 from Point
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator SizeInt32(Point2Int32 pt)
        {
            return new SizeInt32(pt.x, pt.y);
        }

        // Fields
        internal int width;
        internal int height;

        public int Width
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.width;
            set => this.width = value;
        }

        public int Height
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.height;
            set => this.height = value;
        }

        public long Area
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return (long)this.width * (long)this.height;
            }
        }

        public bool HasPositiveArea
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return this.width > 0 && this.height > 0;
            }
        }

        public bool HasZeroArea
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return this.width == 0 && this.height == 0;
            }
        }

        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return this == Empty;
            }
        }

        public bool IsZero
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return this == Zero;
            }
        }

        // Constructor
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SizeInt32(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        // Equality
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(SizeInt32 other)
        {
            return this.width == other.width &&
                   this.height == other.height;
        }

        public override bool Equals(object obj)
        {
            return EquatableUtil.Equals(this, obj);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(SizeInt32 lhs, SizeInt32 rhs)
        {
            return lhs.Equals(rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(SizeInt32 lhs, SizeInt32 rhs)
        {
            return !(lhs == rhs);
        }

        // GetHashCode
        public override int GetHashCode()
        {
            return HashCodeUtil.CombineHashCodes(
                this.width, 
                this.height);
        }
    }
}
