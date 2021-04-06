/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

#nullable enable

using PaintDotNet.Collections;
using System;

namespace PaintDotNet.Imaging.Quantization
{
    // NOTE: _NOT_ thread safe due to ref usage w/ DictionarySlim

    internal sealed class CachingPaletteMap
        : PaletteMap
    {
        private readonly PaletteMap paletteMap;
        private readonly DictionarySlim<ColorBgr24, int> indexCache;

        public CachingPaletteMap(PaletteMap paletteMap)
            : base(paletteMap.Colors)
        {
            this.paletteMap = paletteMap;
            this.indexCache = new DictionarySlim<ColorBgr24, int>();
        }

        protected override byte OnFindClosestPaletteIndex(ColorBgr24 target)
        {
            ref int index = ref this.indexCache.GetOrAddValueRef(target, -1);
            if (index == -1)
            {
                index = this.paletteMap.FindClosestPaletteIndex(target);
            }

            return (byte)index;
        }
    }
}
