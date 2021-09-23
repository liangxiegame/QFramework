using System;
using QFramework.ILRuntime;

namespace QFramework
{
    public interface ILCanRegisterEvent
    {
        ILCanDispose RegisterEvent<T>(Action<T> onEvent);

    }
}