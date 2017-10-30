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

    public class TakeObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly int count;
        readonly TimeSpan duration;
        public readonly IScheduler scheduler; // public for optimization check

        public TakeObservable(IObservable<T> source, int count)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.count = count;
        }

        public TakeObservable(IObservable<T> source, TimeSpan duration, IScheduler scheduler)
            : base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.duration = duration;
            this.scheduler = scheduler;
        }

        // optimize combiner

        public IObservable<T> Combine(int count)
        {
            // xs = 6
            // xs.Take(5) = 5         | xs.Take(3) = 3
            // xs.Take(5).Take(3) = 3 | xs.Take(3).Take(5) = 3

            // use minimum one
            return (this.count <= count)
                ? this
                : new TakeObservable<T>(source, count);
        }

        public IObservable<T> Combine(TimeSpan duration)
        {
            // xs = 6s
            // xs.Take(5s) = 5s          | xs.Take(3s) = 3s
            // xs.Take(5s).Take(3s) = 3s | xs.Take(3s).Take(5s) = 3s

            // use minimum one
            return (this.duration <= duration)
                ? this
                : new TakeObservable<T>(source, duration, scheduler);
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            if (scheduler == null)
            {
                return source.Subscribe(new Take(this, observer, cancel));
            }
            else
            {
                return new Take_(this, observer, cancel).Run();
            }
        }

        class Take : OperatorObserverBase<T, T>
        {
            int rest;

            public Take(TakeObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.rest = parent.count;
            }

            public override void OnNext(T value)
            {
                if (rest > 0)
                {
                    rest -= 1;
                    base.observer.OnNext(value);
                    if (rest == 0)
                    {
                        try { observer.OnCompleted(); } finally { Dispose(); };
                    }
                }
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); } finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                try { observer.OnCompleted(); } finally { Dispose(); }
            }
        }

        class Take_ : OperatorObserverBase<T, T>
        {
            readonly TakeObservable<T> parent;
            readonly object gate = new object();

            public Take_(TakeObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                var d1 = parent.scheduler.Schedule(parent.duration, Tick);
                var d2 = parent.source.Subscribe(this);

                return StableCompositeDisposable.Create(d1, d2);
            }

            void Tick()
            {
                lock (gate)
                {
                    try { observer.OnCompleted(); } finally { Dispose(); };
                }
            }

            public override void OnNext(T value)
            {
                lock (gate)
                {
                    base.observer.OnNext(value);
                }
            }

            public override void OnError(Exception error)
            {
                lock (gate)
                {
                    try { observer.OnError(error); } finally { Dispose(); };
                }
            }

            public override void OnCompleted()
            {
                lock (gate)
                {
                    try { observer.OnCompleted(); } finally { Dispose(); };
                }
            }
        }
    }
}