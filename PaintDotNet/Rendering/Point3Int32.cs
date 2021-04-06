using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PaintDotNet.Rendering
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    [DebuggerDisplay("{X},{Y},{Z}")]
    public struct Point3Int32
        : IEquatable<Point3Int32>
    {
        public static Point3Int32 Zero => default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point3Int32 operator -(Point3Int32 pt)
        {
            return new Point3Int32(-pt.x, -pt.y, -pt.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Point3Int32 Offset(Point3Int32 pt, int dx, int dy, int dz)
        {
            return new Point3Int32(pt.x + dx, pt.y + dy, pt.z + dz);
        }

        // Fields
        internal int x;
        internal int y;
        internal int z;

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

        public int Z
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return this.z;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                this.z = value;
            }
        }

        public bool IsZero
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return this.x == 0 && this.y == 0 && this.z == 0;
            }
        }

        // Constructor
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Point3Int32(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        // Equality
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Point3Int32 other)
        {
            return this.x == other.x &&
                   this.y == other.y &&
                   this.z == other.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            return EquatableUtil.Equals(this, obj);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Point3Int32 lhs, Point3Int32 rhs)
        {
            return lhs.Equals(rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Point3Int32 lhs, Point3Int32 rhs)
        {
            return !(lhs == rhs);
        }

        // GetHashCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return HashCodeUtil.CombineHashCodes(this.x, this.y, this.z);
        }
    }
}
