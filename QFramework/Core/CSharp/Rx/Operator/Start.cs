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

    public class StartObservable<T> : OperatorObservableBase<T>
    {
        readonly Action action;
        readonly Func<T> function;
        readonly IScheduler scheduler;
        readonly TimeSpan? startAfter;

        public StartObservable(Func<T> function, TimeSpan? startAfter, IScheduler scheduler)
            : base(scheduler == Scheduler.CurrentThread)
        {
            this.function = function;
            this.startAfter = startAfter;
            this.scheduler = scheduler;
        }

        public StartObservable(Action action, TimeSpan? startAfter, IScheduler scheduler)
            : base(scheduler == Scheduler.CurrentThread)
        {
            this.action = action;
            this.startAfter = startAfter;
            this.scheduler = scheduler;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            if (startAfter != null)
            {
                return scheduler.Schedule(startAfter.Value, new StartObserver(this, observer, cancel).Run);
            }
            else
            {
                return scheduler.Schedule(new StartObserver(this, observer, cancel).Run);
            }
        }

        class StartObserver : OperatorObserverBase<T, T>
        {
            readonly StartObservable<T> parent;

            public StartObserver(StartObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public void Run()
            {
                var result = default(T);
                try
                {
                    if (parent.function != null)
                    {
                        result = parent.function();
                    }
                    else
                    {
                        parent.action();
                    }
                }
                catch (Exception exception)
                {
                    try { observer.OnError(exception); }
                    finally { Dispose(); }
                    return;
                }

                OnNext(result);
                try { observer.OnCompleted(); }
                finally { Dispose(); }
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
}