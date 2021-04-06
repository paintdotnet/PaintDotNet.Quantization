/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Threading;

namespace PaintDotNet.Imaging.Quantization
{
    internal interface IQuantizer
    {
        ColorBgra32[] GeneratePalette(
            IBitmapSource<ColorBgra32> source,
            int maxColorCount,
            bool addTransparentColor,
            CancellationToken cancelToken);

        ColorBgra32[] GeneratePalette(
            ColorHistogram<ColorBgr24> histogram,
            int maxColorCount,
            bool addtransparentColor,
            CancellationToken cancelToken);
    }
}
