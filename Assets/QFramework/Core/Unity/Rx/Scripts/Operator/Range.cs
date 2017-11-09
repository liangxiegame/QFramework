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

    public class RangeObservable : OperatorObservableBase<int>
    {
        readonly int start;
        readonly int count;
        readonly IScheduler scheduler;

        public RangeObservable(int start, int count, IScheduler scheduler)
            : base(scheduler == Scheduler.CurrentThread)
        {
            if (count < 0) throw new ArgumentOutOfRangeException("count < 0");

            this.start = start;
            this.count = count;
            this.scheduler = scheduler;
        }

        protected override IDisposable SubscribeCore(IObserver<int> observer, IDisposable cancel)
        {
            observer = new Range(observer, cancel);

            if (scheduler == Scheduler.Immediate)
            {
                for (int i = 0; i < count; i++)
                {
                    int v = start + i;
                    observer.OnNext(v);
                }
                observer.OnCompleted();

                return Disposable.Empty;
            }
            else
            {
                var i = 0;
                return scheduler.Schedule((Action self) =>
                {
                    if (i < count)
                    {
                        int v = start + i;
                        observer.OnNext(v);
                        i++;
                        self();
                    }
                    else
                    {
                        observer.OnCompleted();
                    }
                });
            }
        }

        class Range : OperatorObserverBase<int, int>
        {
            public Range(IObserver<int> observer, IDisposable cancel)
                : base(observer, cancel)
            {
            }

            public override void OnNext(int value)
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