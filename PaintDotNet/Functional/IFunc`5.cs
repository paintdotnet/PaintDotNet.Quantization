/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using System;

namespace PaintDotNet.Functional
{
    public interface IFunc<in TArg1, in TArg2, in TArg3, in TArg4, out TResult>
    {
        TResult Invoke(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4);
    }
}
