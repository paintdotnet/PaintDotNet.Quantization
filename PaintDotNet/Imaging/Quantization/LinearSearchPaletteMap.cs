/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

#nullable enable

using System;
using System.Collections.Generic;

namespace PaintDotNet.Imaging.Quantization
{
    internal sealed class LinearSearchPaletteMap
        : PaletteMap
    {
        public LinearSearchPaletteMap(IEnumerable<ColorBgra32> colors)
            : base(colors)
        {
        }

        protected override byte OnFindClosestPaletteIndex(ColorBgr24 target)
        {
            byte bestIndex = 0;
            int bestDistance = ColorBgr24Util.GetDistanceSquared(target, this.OpaqueColorsArray[0]);

            for (int i = 1; i < this.OpaqueColorsArray.Length; ++i)
            {
                ColorBgr24 color = this.OpaqueColorsArray[i];
                int colorDistance = ColorBgr24Util.GetDistanceSquared(target, color);

                if (colorDistance < bestDistance)
                {
                    bestIndex = (byte)i;
                    bestDistance = colorDistance;
                }
            }

            return bestIndex;
        }
    }
}
