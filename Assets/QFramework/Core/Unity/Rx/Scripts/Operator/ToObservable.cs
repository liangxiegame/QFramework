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
    using System.Collections.Generic;

    public class ToObservableObservable<T> : OperatorObservableBase<T>
    {
        readonly IEnumerable<T> source;
        readonly IScheduler scheduler;

        public ToObservableObservable(IEnumerable<T> source, IScheduler scheduler)
            : base(scheduler == Scheduler.CurrentThread)
        {
            this.source = source;
            this.scheduler = scheduler;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new ToObservable(this, observer, cancel).Run();
        }

        class ToObservable : OperatorObserverBase<T, T>
        {
            readonly ToObservableObservable<T> parent;

            public ToObservable(ToObservableObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                var e = default(IEnumerator<T>);
                try
                {
                    e = parent.source.GetEnumerator();
                }
                catch (Exception exception)
                {
                    OnError(exception);
                    return Disposable.Empty;
                }

                if (parent.scheduler == Scheduler.Immediate)
                {
                    while (true)
                    {
                        bool hasNext;
                        var current = default(T);
                        try
                        {
                            hasNext = e.MoveNext();
                            if (hasNext) current = e.Current;
                        }
                        catch (Exception ex)
                        {
                            e.Dispose();
                            try { observer.OnError(ex); }
                            finally { Dispose(); }
                            break;
                        }

                        if (hasNext)
                        {
                            observer.OnNext(current);
                        }
                        else
                        {
                            e.Dispose();
                            try { observer.OnCompleted(); }
                            finally { Dispose(); }
                            break;
                        }
                    }

                    return Disposable.Empty;
                }

                var flag = new SingleAssignmentDisposable();
                flag.Disposable = parent.scheduler.Schedule(self =>
                {
                    if (flag.IsDisposed)
                    {
                        e.Dispose();
                        return;
                    }

                    bool hasNext;
                    var current = default(T);
                    try
                    {
                        hasNext = e.MoveNext();
                        if (hasNext) current = e.Current;
                    }
                    catch (Exception ex)
                    {
                        e.Dispose();
                        try { observer.OnError(ex); }
                        finally { Dispose(); }
                        return;
                    }

                    if (hasNext)
                    {
                        observer.OnNext(current);
                        self();
                    }
                    else
                    {
                        e.Dispose();
                        try { observer.OnCompleted(); }
                        finally { Dispose(); }
                    }
                });

                return flag;
            }

            public override void OnNext(T value)
            {
                // do nothing
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