/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using System;

namespace PaintDotNet
{
    public static class FloatUtil
    {
        public static float Clamp(float value, float min, float max)
        {
            if (min > max)
            {
                throw new ArgumentException(string.Format(
                    "min must be less than or equal to max. value={0}, min={1}, max={2}",
                    value,
                    min,
                    max));
            }

            if (value < min)
            {
                return min;
            }
            else if (value > max)
            {
                return max;
            }
            else
            {
                return value;
            }
        }
    }
}
