using System;
using System.Collections.Generic;
using UniRx;

namespace uFrame.Kernel
{
    public class SimpleSubject<T> : ISubject<T>
    {
        private List<IObserver<T>> _observers;

        public List<IObserver<T>> Observers
        {
            get { return _observers ?? (_observers = new List<IObserver<T>>()); }
            set { _observers = value; }
        }

        public void OnCompleted()
        {
            foreach (var observer in Observers.ToArray())
            {
                if (observer == null) continue;
                observer.OnCompleted();
            }
            Observers.Clear();
        }

        public void OnError(Exception error)
        {
            foreach (var observer in Observers.ToArray())
            {
                if (observer == null) continue;
                observer.OnError(error);
            }
        }

        public void OnNext(T value)
        {
            foreach (var observer in Observers)
            {
                if (observer == null) continue;
                observer.OnNext(value);
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            Observers.Add(observer);
            return Disposable.Create(() => Observers.Remove(observer));
        }
    }
}