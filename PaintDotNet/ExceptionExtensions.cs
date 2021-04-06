/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using System;

namespace PaintDotNet
{
    public static class ExceptionExtensions
    {
        public static bool IsOperationCanceled(this Exception ex)
        {
            if (ex is OperationCanceledException)
            {
                return true;
            }
            else if (ex is AggregateException aggEx)
            {
                return aggEx.IsOnly<OperationCanceledException>();
            }
            else
            {
                return false;
            }
        }
    }
}
