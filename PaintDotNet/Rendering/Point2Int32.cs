using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PaintDotNet.Rendering
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    [DebuggerDisplay("{X},{Y}")]
    public struct Point2Int32
        : IEquatable<Point2Int32>
    {
        public static Point2Int32 Zero => default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Point2Int32(SizeInt32 size)
        {
            return new Point2Int32(size.width, size.height);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point2Int32 operator -(Point2Int32 pt)
        {
            return new Point2Int32(-pt.x, -pt.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point2Int32 Offset(Point2Int32 pt, int dx, int dy)
        {
            return new Point2Int32(pt.x + dx, pt.y + dy);
        }

        // Fields
        internal int x;
        internal int y;

        // Properties
        public int X
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return this.x;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                this.x = value;
            }
        }

        public int Y
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return this.y;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                this.y = value;
            }
        }

        public bool IsZero
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return this.x == 0 && this.y == 0;
            }
        }

        // Constructor
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Point2Int32(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        // Equality
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Point2Int32 other)
        {
            return this.x == other.x &&
                   this.y == other.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            return EquatableUtil.Equals(this, obj);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Point2Int32 lhs, Point2Int32 rhs)
        {
            return lhs.Equals(rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Point2Int32 lhs, Point2Int32 rhs)
        {
            return !(lhs == rhs);
        }

        // GetHashCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return HashCodeUtil.CombineHashCodes(this.x, this.y);
        }
    }
}
