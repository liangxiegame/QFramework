using System;

namespace QFramework
{
    public interface IDisposeEventRegister
    {
        void OnDisposed(System.Action onDisposedEvent);

        IDisposeEventRegister OnFinished(Action onFinishedEvent);
    }
}