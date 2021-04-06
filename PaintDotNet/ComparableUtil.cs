/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.CompilerServices;

namespace PaintDotNet
{
    public static class ComparableUtil
    {
        // Ensures that a comparison results in only the values -1, 0, +1
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CompareTrinary<T>(T value1, T value2)
            where T : IComparable<T>
        {
            int compare = value1.CompareTo(value2);
            return (compare < 0) ? -1 : (compare > 0) ? +1 : 0;
        }
    }
}
