/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PaintDotNet.Collections
{
    public struct ComparisonWrapper<T>
        : IComparer<T>
    {
        private Comparison<T> comparison;

        public ComparisonWrapper(Comparison<T> comparison)
        {
            this.comparison = comparison;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Compare(T x, T y)
        {
            return this.comparison(x, y);
        }
    }
}
