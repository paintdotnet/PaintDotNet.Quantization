using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PaintDotNet.Rendering
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    [DebuggerDisplay("{x},{y},{width},{height}")]
    public struct RectInt32
        : IEquatable<RectInt32>
    {
        public static RectInt32 Empty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new RectInt32()
                {
                    x = int.MaxValue,
                    y = int.MaxValue,
                    width = int.MinValue,
                    height = int.MinValue
                };
            }
        }

        public static RectInt32 Zero => default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectInt32 FromEdges(int left, int top, int right, int bottom)
        {
            int minX = Math.Min(left, right);
            int minY = Math.Min(top, bottom);
            int maxX = Math.Max(left, right);
            int maxY = Math.Max(top, bottom);

            int width = checked(maxX - minX);
            int height = checked(maxY - minY);
            return new RectInt32(minX, minY, width, height);
        }

        // Static methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectInt32 Inflate(RectInt32 rect, int dx, int dy)
        {
            RectInt32 rectP = rect;
            rectP.Inflate(dx, dy);
            return rectP;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectInt32 Inflate(RectInt32 rect, int left, int top, int right, int bottom)
        {
            return RectInt32.FromEdges(
                rect.Left - left,
                rect.Top - top,
                rect.Right + right,
                rect.Bottom + bottom);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectInt32 Intersect(RectInt32 a, RectInt32 b)
        {
            RectInt32 c = a;
            c.Intersect(b);
            return c;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectInt32 Offset(RectInt32 rect, Point2Int32 offset)
        {
            return Offset(rect, offset.x, offset.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectInt32 Offset(RectInt32 rect, int dx, int dy)
        {
            return new RectInt32(rect.x + dx, rect.y + dy, rect.width, rect.height);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectInt32 Scale(RectInt32 rect, int scale)
        {
            return Scale(rect, scale, scale);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectInt32 Scale(RectInt32 rect, int scaleX, int scaleY)
        {
            rect.Scale(scaleX, scaleY);
            return rect;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectInt32 Union(RectInt32 a, RectInt32 b)
        {
            RectInt32 union = a;
            union.Union(b);
            return union;
        }
        
        // Fields
        internal int x;
        internal int y;
        internal int width;
        internal int height;

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

        public int Width
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return this.width;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                this.width = value;
            }
        }

        public int Height
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return this.height;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                this.height = value;
            }
        }

        public int Left
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return this.x;
            }
        }

        public int Top
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return this.y;
            }
        }

        public int Right
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return this.x + this.width;
            }
        }

        public int Bottom
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return this.y + this.height;
            }
        }

        public Point2Int32 TopLeft
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new Point2Int32(this.x, this.y);
            }
        }

        public Point2Int32 TopRight
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new Point2Int32(this.x + this.width, this.y);
            }
        }

        public Point2Int32 BottomLeft
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new Point2Int32(this.x, this.y + this.height);
            }
        }

        public Point2Int32 BottomRight
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new Point2Int32(this.x + this.width, this.y + this.height);
            }
        }

        public Point2Int32 Location
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new Point2Int32(this.x, this.y);
            }
        }

        public SizeInt32 Size
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new SizeInt32()
                {
                    width = this.width,
                    height = this.height
                };
            }
        }

        public bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return this == RectInt32.Empty;
            }
        }

        public bool IsCenteredMax
        {
            get
            {
                return this.x == -1073741824
                    && this.y == -1073741824
                    && this.width == int.MaxValue
                    && this.height == int.MaxValue;
            }
        }

        public bool IsZero
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return this == RectInt32.Zero;
            }
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
                return this.width == 0 || this.height == 0;
            }
        }

        // Methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(int x, int y)
        {
            return (this.x <= x) && (x < (this.x + this.width)) && (this.y <= y) && (y < (this.y + this.height));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(Point2Int32 pt)
        {
            return Contains(pt.X, pt.Y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(RectInt32 rect)
        {
            return (this.x <= rect.x) && ((rect.X + rect.width) <= (this.x + this.width)) &&
                   (this.y <= rect.y) && ((rect.Y + rect.height) <= (this.y + this.height));                  
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Inflate(int dx, int dy)
        {
            this.x -= dx;
            this.y -= dy;
            this.width += (2 * dx);
            this.height += (2 * dy);
        }
        
        public void Intersect(RectInt32 rect)
        {
            int maxLeft = Math.Max(this.x, rect.x);
            int maxTop = Math.Max(this.y, rect.y);
            int width = Math.Max(Math.Min(this.Right, rect.Right) - maxLeft, 0);
            int height = Math.Max(Math.Min(this.Bottom, rect.Bottom) - maxTop, 0);

            this.x = maxLeft;
            this.y = maxTop;
            this.width = width;
            this.height = height;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IntersectsWith(RectInt32 rect)
        {
            return ((this.x < (rect.x + rect.width)) && (rect.x < (this.x + this.width))) &&
                   ((this.y < (rect.y + rect.height)) && (rect.y < (this.y + this.height)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Offset(Point2Int32 delta)
        {
            this.x += delta.X;
            this.y += delta.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Offset(int dx, int dy)
        {
            this.x += dx;
            this.y += dy;
        }

        public void Scale(int scaleX, int scaleY)
        {
            this.x *= scaleX;
            this.y *= scaleY;
            this.width *= scaleX;
            this.height *= scaleY;

            if (scaleX < 0)
            {
                this.x += this.width;
                this.width *= -1;
            }

            if (scaleY < 0)
            {
                this.y += this.height;
                this.height *= -1;
            }
        }

        public void Union(RectInt32 rect)
        {
            int left = Math.Min(this.Left, rect.Left);
            int top = Math.Min(this.Top, rect.Top);
            int right = Math.Max(this.Right, rect.Right);
            int bottom = Math.Max(this.Bottom, rect.Bottom);

            this.x = left;
            this.y = top;
            this.width = right - left;
            this.height = bottom - top;
        }

        // Constructor
        public RectInt32(Point2Int32 location, SizeInt32 size)
        {
            this.x = location.x;
            this.y = location.y;
            this.width = size.width;
            this.height = size.height;
        }

        public RectInt32(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        // Equality
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(RectInt32 other)
        {
            return this.x == other.x &&
                   this.y == other.y &&
                   this.width == other.width &&
                   this.height == other.height;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            return EquatableUtil.Equals(this, obj);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(RectInt32 a, RectInt32 b)
        {
            return a.Equals(b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(RectInt32 a, RectInt32 b)
        {
            return !(a == b);
        }

        // GetHashCode
        public override int GetHashCode()
        {
            return HashCodeUtil.CombineHashCodes(
                this.x.GetHashCode(),
                this.y.GetHashCode(),
                this.width.GetHashCode(),
                this.height.GetHashCode());
        }
    }
}

