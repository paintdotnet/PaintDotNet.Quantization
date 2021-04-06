/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using PaintDotNet.Collections;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PaintDotNet.Imaging
{
    internal sealed class ColorHistogramBuilder<TPixel>
        : IEnumerable<ColorHistogram<TPixel>.Entry>
          where TPixel : unmanaged, INaturalPixelInfo, IEquatable<TPixel>
    {
        private HashSet<TPixel> entries1;
        private DictionarySlim<TPixel, uint> entries32;
        private DictionarySlim<TPixel, long> entries64;

        public ColorHistogramBuilder()
        {
            this.entries1 = new HashSet<TPixel>();
            this.entries32 = new DictionarySlim<TPixel, uint>();
            this.entries64 = new DictionarySlim<TPixel, long>();
        }

        public int Count => this.CountUnique + this.Count32Bit + this.Count64Bit;

        public int CountUnique => this.entries1.Count;

        public int Count32Bit => this.entries32.Count;

        public int Count64Bit => this.entries64.Count;

        public void AddColor(TPixel color)
        {
            if (this.entries64.TryGetValue(color, out long count64))
            {
                this.entries64.GetOrAddValueRef(color) = count64 + 1;
            }
            else if (this.entries32.TryGetValue(color, out uint count32))
            {
                if (count32 == uint.MaxValue)
                {
                    this.entries64.GetOrAddValueRef(color) = (long)uint.MaxValue + 1;
                    this.entries32.Remove(color);
                }
                else
                {
                    this.entries32.GetOrAddValueRef(color) = count32 + 1;
                }
            }
            else if (!this.entries1.Add(color))
            {
                this.entries1.Remove(color);
                this.entries32.GetOrAddValueRef(color) = 2;
            }
        }

        public void AddColor(TPixel color, long countIncrement)
        {
            if (countIncrement < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (countIncrement == 0)
            {
                // do nothing
            }
            else if (countIncrement == 1)
            {
                AddColor(color);
            }
            else if (this.entries64.TryGetValue(color, out long count64))
            {
                this.entries64.GetOrAddValueRef(color) = count64 + countIncrement;
            }
            else if (this.entries32.TryGetValue(color, out uint count32))
            {
                long newCount = (long)count32 + countIncrement;
                if (newCount <= uint.MaxValue)
                {
                    this.entries32.GetOrAddValueRef(color) = (uint)newCount;
                }
                else
                {
                    this.entries64.GetOrAddValueRef(color) = newCount;
                    this.entries32.Remove(color);
                }
            }
            else
            {
                long newCount = countIncrement;
                if (this.entries1.Remove(color))
                {
                    ++newCount;
                }

                if (newCount <= uint.MaxValue)
                {
                    this.entries32.GetOrAddValueRef(color) = (uint)newCount;
                }
                else
                {
                    this.entries64.GetOrAddValueRef(color) = newCount;
                }
            }
        }

        public void Clear()
        {
            this.entries1.Clear();
            this.entries32.Clear();
            this.entries64.Clear();
        }

        public void UnionWith(ColorHistogramBuilder<TPixel> other)
        {
            if (other.Count64Bit > 0)
            {
                using (DictionarySlim<TPixel, long>.Enumerator enum64 = other.GetEnumeratorFor64bitCounts())
                {
                    while (enum64.MoveNext())
                    {
                        KeyValuePair<TPixel, long> entry = enum64.Current;
                        AddColor(entry.Key, entry.Value);
                    }
                }
            }

            if (other.Count32Bit > 0)
            {
                using (DictionarySlim<TPixel, uint>.Enumerator enum32 = other.GetEnumeratorFor32bitCounts())
                {
                    while (enum32.MoveNext())
                    {
                        KeyValuePair<TPixel, uint> entry = enum32.Current;
                        AddColor(entry.Key, entry.Value);
                    }
                }
            }

            if (other.CountUnique > 0)
            {
                using (HashSet<TPixel>.Enumerator enum1 = other.GetEnumeratorForUniqueCounts())
                {
                    while (enum1.MoveNext())
                    {
                        AddColor(enum1.Current);
                    }
                }
            }
        }

        public ColorHistogram<TPixel> Build()
        {
            return ColorHistogram<TPixel>.From(this);
        }

        public IEnumerator<ColorHistogram<TPixel>.Entry> GetEnumerator()
        {
            foreach (TPixel color in this.entries1)
            {
                yield return new ColorHistogram<TPixel>.Entry(color, 1);
            }

            foreach (KeyValuePair<TPixel, uint> entry in this.entries32)
            {
                yield return new ColorHistogram<TPixel>.Entry(entry.Key, entry.Value);
            }

            foreach (KeyValuePair<TPixel, long> entry in this.entries64)
            {
                yield return new ColorHistogram<TPixel>.Entry(entry.Key, entry.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal HashSet<TPixel>.Enumerator GetEnumeratorForUniqueCounts()
        {
            return this.entries1.GetEnumerator();
        }

        internal DictionarySlim<TPixel, uint>.Enumerator GetEnumeratorFor32bitCounts()
        {
            return this.entries32.GetEnumerator();
        }

        internal DictionarySlim<TPixel, long>.Enumerator GetEnumeratorFor64bitCounts()
        {
            return this.entries64.GetEnumerator();
        }
    }
}
