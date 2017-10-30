/****************************************************************************
 * Copyright (c) 2017 liangxie
 * 
 * http://liangxiegame.com
 * https://github.com/liangxiegame/QFramework
 * https://github.com/liangxiegame/QSingleton
 * https://github.com/liangxiegame/QChain
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

namespace QFramework
{
    using System;

    public class ReturnObservable<T> : OperatorObservableBase<T>
    {
        readonly T value;
        readonly IScheduler scheduler;

        public ReturnObservable(T value, IScheduler scheduler)
            : base(scheduler == Scheduler.CurrentThread)
        {
            this.value = value;
            this.scheduler = scheduler;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            observer = new Return(observer, cancel);

            if (scheduler == Scheduler.Immediate)
            {
                observer.OnNext(value);
                observer.OnCompleted();
                return Disposable.Empty;
            }
            else
            {
                return scheduler.Schedule(() =>
                {
                    observer.OnNext(value);
                    observer.OnCompleted();
                });
            }
        }

        class Return : OperatorObserverBase<T, T>
        {
            public Return(IObserver<T> observer, IDisposable cancel)
                : base(observer, cancel)
            {
            }

            public override void OnNext(T value)
            {
                try
                {
                    base.observer.OnNext(value);
                }
                catch
                {
                    Dispose();
                    throw;
                }
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); }
                finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                try { observer.OnCompleted(); }
                finally { Dispose(); }
            }
        }
    }

    public class ImmediateReturnObservable<T> : IObservable<T>, IOptimizedObservable<T>
    {
        readonly T value;

        public ImmediateReturnObservable(T value)
        {
            this.value = value;
        }

        public bool IsRequiredSubscribeOnCurrentThread()
        {
            return false;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            observer.OnNext(value);
            observer.OnCompleted();
            return Disposable.Empty;
        }
    }

    public class ImmutableReturnUnitObservable : IObservable<Unit>, IOptimizedObservable<Unit>
    {
        public static ImmutableReturnUnitObservable Instance = new ImmutableReturnUnitObservable();

        ImmutableReturnUnitObservable()
        {

        }

        public bool IsRequiredSubscribeOnCurrentThread()
        {
            return false;
        }

        public IDisposable Subscribe(IObserver<Unit> observer)
        {
            observer.OnNext(Unit.Default);
            observer.OnCompleted();
            return Disposable.Empty;
        }
    }

    public class ImmutableReturnTrueObservable : IObservable<bool>, IOptimizedObservable<bool>
    {
        public static ImmutableReturnTrueObservable Instance = new ImmutableReturnTrueObservable();

        ImmutableReturnTrueObservable()
        {

        }

        public bool IsRequiredSubscribeOnCurrentThread()
        {
            return false;
        }

        public IDisposable Subscribe(IObserver<bool> observer)
        {
            observer.OnNext(true);
            observer.OnCompleted();
            return Disposable.Empty;
        }
    }

    public class ImmutableReturnFalseObservable : IObservable<bool>, IOptimizedObservable<bool>
    {
        public static ImmutableReturnFalseObservable Instance = new ImmutableReturnFalseObservable();

        ImmutableReturnFalseObservable()
        {

        }

        public bool IsRequiredSubscribeOnCurrentThread()
        {
            return false;
        }

        public IDisposable Subscribe(IObserver<bool> observer)
        {
            observer.OnNext(false);
            observer.OnCompleted();
            return Disposable.Empty;
        }
    }
}
