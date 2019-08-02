using System;
using UniRx;

namespace uFrame.Kernel
{
    public interface IDisposableContainer
    {
        CompositeDisposable Disposer { get; set; }
    }

    public static class DisposableContainerExtensions
    {
        public static IDisposable DisposeWith(this IDisposable disposable, IDisposableContainer container)
        {
            if (container.Disposer.IsDisposed)
            {
                throw new Exception(string.Format("DisposeWith on {0} object is already disposed", container.GetType().Name));
            }
            container.Disposer.Add(disposable);
            return disposable;
        }
    }
}