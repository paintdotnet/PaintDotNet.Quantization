/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using System;

namespace PaintDotNet.Functional
{
    public interface IFunc<in TArg1, out TResult>
    {
        TResult Invoke(TArg1 arg1);
    }
}
