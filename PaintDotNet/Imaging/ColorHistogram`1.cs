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
    public sealed class ColorHistogram<TPixel>
        : IReadOnlyCollection<ColorHistogram<TPixel>.Entry>
          where TPixel : unmanaged, INaturalPixelInfo, IEquatable<TPixel>
    {
        internal static ColorHistogram<TPixel> From(ColorHistogramBuilder<TPixel> builder)
        {
            TPixel[] colors1;
            if (builder.CountUnique == 0)
            {
                colors1 = Array.Empty<TPixel>();
            }
            else
            {
                colors1 = new TPixel[builder.CountUnique];

                using (HashSet<TPixel>.Enumerator enum1 = builder.GetEnumeratorForUniqueCounts())
                {
                    int i = 0;
                    while (enum1.MoveNext())
                    {
                        colors1[i] = enum1.Current;
                        ++i;
                    }
                }
            }

            TPixel[] colors32;
            uint[] counts32;
            if (builder.Count32Bit == 0)
            {
                colors32 = Array.Empty<TPixel>();
                counts32 = Array.Empty<uint>();
            }
            else
            {
                colors32 = new TPixel[builder.Count32Bit];
                counts32 = new uint[colors32.Length];

                using (DictionarySlim<TPixel, uint>.Enumerator enum32 = builder.GetEnumeratorFor32bitCounts())
                {
                    int i = 0;
                    while (enum32.MoveNext())
                    {
                        KeyValuePair<TPixel, uint> entry = enum32.Current;
                        colors32[i] = entry.Key;
                        counts32[i] = entry.Value;
                        ++i;
                    }
                }
            }

            TPixel[] colors64;
            long[] counts64;
            if (builder.Count64Bit == 0)
            {
                colors64 = Array.Empty<TPixel>();
                counts64 = Array.Empty<long>();
            }
            else
            {
                colors64 = new TPixel[builder.Count64Bit];
                counts64 = new long[colors64.Length];

                using (DictionarySlim<TPixel, long>.Enumerator enum64 = builder.GetEnumeratorFor64bitCounts())
                {
                    int i = 0;
                    while (enum64.MoveNext())
                    {
                        KeyValuePair<TPixel, long> entry = enum64.Current;
                        colors64[i] = entry.Key;
                        counts64[i] = entry.Value;
                        ++i;
                    }
                }
            }

            return new ColorHistogram<TPixel>(ref colors1, ref colors32, ref counts32, ref colors64, ref counts64);
        }

        private readonly TPixel[] colors1;

        private readonly TPixel[] colors32;
        private readonly uint[] counts32;

        private readonly TPixel[] colors64;
        private readonly long[] counts64;

        private ColorHistogram(
            ref TPixel[] colors1,
            ref TPixel[] colors32,
            ref uint[] counts32,
            ref TPixel[] colors64,
            ref long[] counts64)
        {
            /*Validate.Begin()
                    .IsNotNull(colors1, nameof(colors1))
                    .IsNotNull(colors32, nameof(colors32))
                    .IsNotNull(counts32, nameof(counts32))
                    .AreLengthsEqual(colors32, nameof(colors32), counts32, nameof(counts32))
                    .IsNotNull(colors64, nameof(colors64))
                    .IsNotNull(counts64, nameof(counts64))
                    .AreLengthsEqual(colors64, nameof(colors64), counts64, nameof(counts64))
                    .Check();*/

            this.colors1 = colors1;
            this.colors32 = colors32;
            this.counts32 = counts32;
            this.colors64 = colors64;
            this.counts64 = counts64;

            colors1 = null;
            colors32 = null;
            counts32 = null;
            colors64 = null;
            counts64 = null;
        }

        public int Count => this.colors1.Length + this.colors32.Length + this.colors64.Length;

        public IEnumerator<Entry> GetEnumerator()
        {
            foreach (TPixel color in this.colors1)
            {
                yield return new Entry(color, 1);
            }

            for (int i = 0, count = this.colors32.Length; i < count; ++i)
            {
                yield return new Entry(this.colors32[i], this.counts32[i]);
            }

            for (int i = 0, count = this.colors64.Length; i < count; ++i)
            {
                yield return new Entry(this.colors64[i], this.counts64[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public readonly struct Entry
            : IEquatable<Entry>
        {
            public TPixel Color
            {
                get;
            }

            public long Count
            {
                get;
            }

            public Entry(TPixel color, long count)
            {
                this.Color = color;
                this.Count = count;
            }

            public bool Equals(Entry other)
            {
                return this.Color.Equals(other.Color)
                    && this.Count == other.Count;
            }

            public override bool Equals(object obj)
            {
                return EquatableUtil.Equals(this, obj);
            }

            public override int GetHashCode()
            {
                return HashCodeUtil.CombineHashCodes(
                    this.Color.GetHashCode(),
                    this.Count.GetHashCode());
            }
        }
    }
}
