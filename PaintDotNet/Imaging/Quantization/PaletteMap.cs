/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace PaintDotNet.Imaging.Quantization
{
    public abstract class PaletteMap
    {
        public static PaletteMap Create(IEnumerable<ColorBgra32> colors)
        {
            return new ProximityPaletteMap(colors);
        }

        private readonly ColorBgra32[] colors;
        private readonly ColorBgr24[] opaqueColors;
        private readonly byte? transparentIndex;

        protected PaletteMap(IEnumerable<ColorBgra32> colors)
        {
            this.colors = colors.ToArray();
            if (this.colors.Length == 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            int opaqueColorsCount = this.colors.Length;
            for (int i = 0; i < this.colors.Length; ++i)
            {
                ColorBgra32 color = this.colors[i];
                if (color.A == 255)
                {
                    // okay
                }
                else if (color.Bgra == 0 && i == (this.colors.Length - 1))
                {
                    // okay
                    this.transparentIndex = checked((byte)i);
                    opaqueColorsCount = i;
                }
                else
                {
                    throw new ArgumentException("palette colors must be opaque (A=255), although 1 transparent color as the last entry is permitted");
                }
            }

            this.opaqueColors = new ColorBgr24[opaqueColorsCount];
            for (int i = 0; i < this.opaqueColors.Length; ++i)
            {
                this.opaqueColors[i] = (ColorBgr24)(ColorBgr32)this.colors[i];
            }
        }

        public IReadOnlyList<ColorBgra32> Colors => this.colors;

        protected ColorBgra32[] ColorsArray => this.colors;

        public IReadOnlyList<ColorBgr24> OpaqueColors => this.opaqueColors;

        protected ColorBgr24[] OpaqueColorsArray
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.opaqueColors;
        }

        public byte? TransparentIndex => this.transparentIndex;

        public byte FindClosestPaletteIndex(ColorBgra32 target)
        {
            if (target.A < 255 && this.transparentIndex.HasValue)
            {
                return this.transparentIndex.Value;
            }

            return OnFindClosestPaletteIndex((ColorBgr24)(ColorBgr32)target);
        }

        protected abstract byte OnFindClosestPaletteIndex(ColorBgr24 target);
    }
}
