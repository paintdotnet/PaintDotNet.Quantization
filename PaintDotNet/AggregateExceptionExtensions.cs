/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using System;

namespace PaintDotNet
{
    public static class AggregateExceptionExtensions
    {
        public static bool IsOnly<T>(this AggregateException aggEx)
            where T : Exception
        {
            foreach (Exception ex in aggEx.InnerExceptions)
            {
                if (ex is AggregateException aggEx2)
                {
                    if (!IsOnly<T>(aggEx2))
                    {
                        return false;
                    }
                }
                else if (!(ex is T))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
