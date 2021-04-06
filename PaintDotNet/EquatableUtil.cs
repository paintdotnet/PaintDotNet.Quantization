/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PaintDotNet
{
    public static class EquatableUtil
    {
        public static new bool Equals(object a, object b)
        {
            if (object.ReferenceEquals(a, b))
            {
                return true;
            }

            bool aIsNull = a is null;
            bool bIsNull = b is null;

            if (aIsNull && bIsNull)
            {
                return true;
            }

            if (aIsNull || bIsNull)
            {
                return false;
            }

            if (a.GetType() != b.GetType())
            {
                return false;
            }

            if (a.GetHashCode() != b.GetHashCode())
            {
                return false;
            }

            bool aEqualsB = a.Equals(b);
            Debug.Assert(aEqualsB == b.Equals(a));
            return aEqualsB;
        }

        /// <summary>
        /// This is a utility method you can call from your override of System.Object.Equals(object) when you
        /// have already implemented IEquatable&lt;TThis&gt;.Equals(TThis other).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals<TThis, TOther>(TThis @this, TOther obj)
            where TThis : TOther, IEquatable<TThis>
        {
            return obj is TThis other
                && @this.GetHashCode() == other.GetHashCode()
                && @this.Equals(other);
        }

        public static bool OperatorEquals<T>(T a, T b)
            where T : class, IEquatable<T>
        {
            if (object.ReferenceEquals(a, b))
            {
                return true;
            }

            if (a is null || b is null)
            {
                return false;
            }

            if (a.GetHashCode() != b.GetHashCode())
            {
                return false;
            }

            Debug.Assert(a.Equals(b) == b.Equals(a));
            return a.Equals(b);
        }
    }
}
