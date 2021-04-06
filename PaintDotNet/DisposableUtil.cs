/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.CompilerServices;

namespace PaintDotNet
{
    public static class DisposableUtil
    {
        /// <summary>
        /// Disposes the given object reference, if it is non-null. If the reference is non-null, it will then be set to null.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Free<T>(ref T disposeMe)
            where T : class, IDisposable
        {
            if (disposeMe != null)
            {
                disposeMe.Dispose();
                disposeMe = null;
            }
        }

        /// <summary>
        /// Disposes the given object reference, if it is non-null. If the reference is non-null, it will then be set to null.
        /// </summary>
        /// <param name="callerIsNotFinalizing">
        /// Whether or not the caller is in their finalizer. Pass in the value of 'disposing' 
        /// from the Dispose() method. Otherwise, use the other overload of Free().
        /// If this value is false (which will be the case when Dispose(bool) is called from
        /// a finalizer), then Dispose() will not be called on disposeMe, but it will still
        /// be set to null.
        /// </param>
        /// <remarks>
        /// This overload of Free() should only be used from a Dispose(bool) method.
        /// </remarks>
        public static void Free<T>(ref T disposeMe, bool callerIsNotFinalizing)
            where T : class, IDisposable
        {
            if (disposeMe != null)
            {
                if (callerIsNotFinalizing)
                {
                    disposeMe.Dispose();
                }

                disposeMe = null;
            }
        }
    }
}
