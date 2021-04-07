/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Threading;

namespace PaintDotNet.Imaging.Quantization
{
    public static class QuantizerExtensions
    {
        public static ColorBgra32[] GeneratePalette(
            this IQuantizer quantizer,
            IBitmapSource<ColorBgra32> source,
            int maxColorCount,
            bool addTransparentColor)
        {
            return quantizer.GeneratePalette(source, maxColorCount, addTransparentColor, CancellationToken.None);
        }

        public static ColorBgra32[] GeneratePalette(
            this IQuantizer quantizer,
            ColorHistogram<ColorBgr24> histogram,
            int maxColorCount,
            bool addtransparentColor)
        {
            return quantizer.GeneratePalette(histogram, maxColorCount, addtransparentColor, CancellationToken.None);
        }
    }
}
