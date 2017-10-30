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
    // needs to more improvement
    using System;
    using System.Collections.Generic;

    public class ConcatObservable<T> : OperatorObservableBase<T>
    {
        readonly IEnumerable<IObservable<T>> sources;

        public ConcatObservable(IEnumerable<IObservable<T>> sources)
            : base(true)
        {
            this.sources = sources;
        }

        public IObservable<T> Combine(IEnumerable<IObservable<T>> combineSources)
        {
            return new ConcatObservable<T>(CombineSources(this.sources, combineSources));
        }

        static IEnumerable<IObservable<T>> CombineSources(IEnumerable<IObservable<T>> first, IEnumerable<IObservable<T>> second)
        {
            foreach (var item in first)
            {
                yield return item;
            }
            foreach (var item in second)
            {
                yield return item;
            }
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new Concat(this, observer, cancel).Run();
        }

        class Concat : OperatorObserverBase<T, T>
        {
            readonly ConcatObservable<T> parent;
            readonly object gate = new object();

            bool isDisposed;
            IEnumerator<IObservable<T>> e;
            SerialDisposable subscription;
            Action nextSelf;

            public Concat(ConcatObservable<T> parent, IObserver<T> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                isDisposed = false;
                e = parent.sources.GetEnumerator();
                subscription = new SerialDisposable();

                var schedule = Scheduler.DefaultSchedulers.TailRecursion.Schedule(RecursiveRun);

                return StableCompositeDisposable.Create(schedule, subscription, Disposable.Create(() =>
               {
                   lock (gate)
                   {
                       this.isDisposed = true;
                       this.e.Dispose();
                   }
               }));
            }

            void RecursiveRun(Action self)
            {
                lock (gate)
                {
                    this.nextSelf = self;
                    if (isDisposed) return;

                    var current = default(IObservable<T>);
                    var hasNext = false;
                    var ex = default(Exception);

                    try
                    {
                        hasNext = e.MoveNext();
                        if (hasNext)
                        {
                            current = e.Current;
                            if (current == null) throw new InvalidOperationException("sequence is null.");
                        }
                        else
                        {
                            e.Dispose();
                        }
                    }
                    catch (Exception exception)
                    {
                        ex = exception;
                        e.Dispose();
                    }

                    if (ex != null)
                    {
                        try { observer.OnError(ex); }
                        finally { Dispose(); }
                        return;
                    }

                    if (!hasNext)
                    {
                        try { observer.OnCompleted(); }
                        finally { Dispose(); }
                        return;
                    }

                    var source = current;
                    var d = new SingleAssignmentDisposable();
                    subscription.Disposable = d;
                    d.Disposable = source.Subscribe(this);
                }
            }

            public override void OnNext(T value)
            {
                base.observer.OnNext(value);
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); }
                finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                this.nextSelf();
            }
        }
    }
}
