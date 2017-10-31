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
    
    public class SwitchObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<IObservable<T>> sources;

        public SwitchObservable(IObservable<IObservable<T>> sources)
            : base(true)
        {
            this.sources = sources;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new SwitchObserver(this, observer, cancel).Run();
        }

        class SwitchObserver : OperatorObserverBase<IObservable<T>, T>
        {
            readonly SwitchObservable<T> parent;

            readonly object gate = new object();
            readonly SerialDisposable innerSubscription = new SerialDisposable();
            bool isStopped = false;
            ulong latest = 0UL;
            bool hasLatest = false;

            public SwitchObserver(SwitchObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                var subscription = parent.sources.Subscribe(this);
                return StableCompositeDisposable.Create(subscription, innerSubscription);
            }

            public override void OnNext(IObservable<T> value)
            {
                var id = default(ulong);
                lock (gate)
                {
                    id = unchecked(++latest);
                    hasLatest = true;
                }

                var d = new SingleAssignmentDisposable();
                innerSubscription.Disposable = d;
                d.Disposable = value.Subscribe(new Switch(this, id));
            }

            public override void OnError(Exception error)
            {
                lock (gate)
                {
                    try { observer.OnError(error); }
                    finally { Dispose(); }
                }
            }

            public override void OnCompleted()
            {
                lock (gate)
                {
                    isStopped = true;
                    if (!hasLatest)
                    {
                        try { observer.OnCompleted(); }
                        finally { Dispose(); }
                    }
                }
            }

            class Switch : IObserver<T>
            {
                readonly SwitchObserver parent;
                readonly ulong id;

                public Switch(SwitchObserver observer, ulong id)
                {
                    this.parent = observer;
                    this.id = id;
                }

                public void OnNext(T value)
                {
                    lock (parent.gate)
                    {
                        if (parent.latest == id)
                        {
                            parent.observer.OnNext(value);
                        }
                    }
                }

                public void OnError(Exception error)
                {
                    lock (parent.gate)
                    {
                        if (parent.latest == id)
                        {
                            parent.observer.OnError(error);
                        }
                    }
                }

                public void OnCompleted()
                {
                    lock (parent.gate)
                    {
                        if (parent.latest == id)
                        {
                            parent.hasLatest = false;
                            if (parent.isStopped)
                            {
                                parent.observer.OnCompleted();
                            }
                        }
                    }
                }
            }
        }
    }
}
