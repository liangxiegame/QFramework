using System;

namespace QFramework
{
    public interface ICanClick<T>
    {
        T OnClick(Action action);
    }
}