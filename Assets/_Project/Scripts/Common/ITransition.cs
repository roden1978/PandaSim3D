using System;

namespace Common
{
    public interface ITransition
    {
        Func<bool> Condition { get; }
        IState To { get; }
    }
}
