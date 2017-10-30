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

    public class SkipObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly int count;
        readonly TimeSpan duration;
        public readonly IScheduler scheduler; // public for optimization check

        public SkipObservable(IObservable<T> source, int count)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.count = count;
        }

        public SkipObservable(IObservable<T> source, TimeSpan duration, IScheduler scheduler)
            : base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.duration = duration;
            this.scheduler = scheduler;
        }

        // optimize combiner

        public IObservable<T> Combine(int count)
        {
            // use sum
            // xs = 6
            // xs.Skip(2) = 4
            // xs.Skip(2).Skip(3) = 1

            return new SkipObservable<T>(source, this.count + count);
        }

        public IObservable<T> Combine(TimeSpan duration)
        {
            // use max
            // xs = 6s
            // xs.Skip(2s) = 2s
            // xs.Skip(2s).Skip(3s) = 3s

            return (duration <= this.duration)
                ? this
                : new SkipObservable<T>(source, duration, scheduler);
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            if (scheduler == null)
            {
                return source.Subscribe(new Skip(this, observer, cancel));
            }
            else
            {
                return new Skip_(this, observer, cancel).Run();
            }
        }

        class Skip : OperatorObserverBase<T, T>
        {
            int remaining;

            public Skip(SkipObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.remaining = parent.count;
            }

            public override void OnNext(T value)
            {
                if (remaining <= 0)
                {
                    base.observer.OnNext(value);
                }
                else
                {
                    remaining--;
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

        class Skip_ : OperatorObserverBase<T, T>
        {
            readonly SkipObservable<T> parent;
            volatile bool open;

            public Skip_(SkipObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
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
                open = true;
            }

            public override void OnNext(T value)
            {
                if (open)
                {
                    base.observer.OnNext(value);
                }
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); } finally { Dispose(); };
            }

            public override void OnCompleted()
            {
                try { observer.OnCompleted(); } finally { Dispose(); };
            }
        }
    }
}