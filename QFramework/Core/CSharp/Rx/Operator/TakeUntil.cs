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

    public class TakeUntilObservable<T, TOther> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly IObservable<TOther> other;

        public TakeUntilObservable(IObservable<T> source, IObservable<TOther> other)
            : base(source.IsRequiredSubscribeOnCurrentThread() || other.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.other = other;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new TakeUntil(this, observer, cancel).Run();
        }

        class TakeUntil : OperatorObserverBase<T, T>
        {
            readonly TakeUntilObservable<T, TOther> parent;
            object gate = new object();
            bool open;

            public TakeUntil(TakeUntilObservable<T, TOther> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                var otherSubscription = new SingleAssignmentDisposable();
                var otherObserver = new TakeUntilOther(this, otherSubscription);
                otherSubscription.Disposable = parent.other.Subscribe(otherObserver);

                var sourceSubscription = parent.source.Subscribe(this);

                return StableCompositeDisposable.Create(otherSubscription, sourceSubscription);
            }

            public override void OnNext(T value)
            {
                if (open)
                {
                    observer.OnNext(value);
                }
                else
                {
                    lock (gate)
                    {
                        observer.OnNext(value);
                    }
                }
            }

            public override void OnError(Exception error)
            {
                lock (gate)
                {
                    try { observer.OnError(error); } finally { Dispose(); }
                }
            }

            public override void OnCompleted()
            {
                lock (gate)
                {
                    try { observer.OnCompleted(); } finally { Dispose(); }
                }
            }

            class TakeUntilOther : IObserver<TOther>
            {
                readonly TakeUntil sourceObserver;
                readonly IDisposable subscription;

                public TakeUntilOther(TakeUntil sourceObserver, IDisposable subscription)
                {
                    this.sourceObserver = sourceObserver;
                    this.subscription = subscription;
                }

                public void OnNext(TOther value)
                {
                    lock (sourceObserver.gate)
                    {
                        try { sourceObserver.observer.OnCompleted(); } finally { sourceObserver.Dispose(); }
                    }
                }

                public void OnError(Exception error)
                {
                    lock (sourceObserver.gate)
                    {
                        try { sourceObserver.observer.OnError(error); } finally { sourceObserver.Dispose(); }
                    }
                }

                public void OnCompleted()
                {
                    lock (sourceObserver.gate)
                    {
                        sourceObserver.open = true;
                        subscription.Dispose();
                    }
                }
            }
        }
    }
}