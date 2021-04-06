/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using System;

namespace PaintDotNet.Functional
{
    public interface IFunc<out TResult>
    {
        TResult Invoke();
    }
}
