using System;

namespace PaintDotNet.Functional
{
    public interface IFunc<out TResult>
    {
        TResult Invoke();
    }
}
