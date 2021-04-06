/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using System;

namespace PaintDotNet
{
    public class InternalErrorException
        : Exception
    {
        public InternalErrorException()
        {
        }

        public InternalErrorException(string message)
            : base(message)
        {
        }

        public InternalErrorException(Exception innerException)
            : this(null, innerException)
        {
        }

        public InternalErrorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
