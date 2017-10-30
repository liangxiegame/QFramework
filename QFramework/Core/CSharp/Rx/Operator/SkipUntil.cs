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
    
    public class SkipUntilObservable<T, TOther> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly IObservable<TOther> other;

        public SkipUntilObservable(IObservable<T> source, IObservable<TOther> other)
            : base(source.IsRequiredSubscribeOnCurrentThread() || other.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.other = other;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new SkipUntilOuterObserver(this, observer, cancel).Run();
        }

        class SkipUntilOuterObserver : OperatorObserverBase<T, T>
        {
            readonly SkipUntilObservable<T, TOther> parent;

            public SkipUntilOuterObserver(SkipUntilObservable<T, TOther> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                var sourceSubscription = new SingleAssignmentDisposable();
                var sourceObserver = new SkipUntil(this, sourceSubscription);
                
                var otherSubscription = new SingleAssignmentDisposable();
                var otherObserver = new SkipUntilOther(this, sourceObserver, otherSubscription);

                sourceSubscription.Disposable = parent.source.Subscribe(sourceObserver);
                otherSubscription.Disposable = parent.other.Subscribe(otherObserver);

                return StableCompositeDisposable.Create(otherSubscription, sourceSubscription);
            }

            public override void OnNext(T value)
            {
            }

            public override void OnError(Exception error)
            {
            }

            public override void OnCompleted()
            {
            }

            class SkipUntil : IObserver<T>
            {
                public volatile IObserver<T> observer;
                readonly SkipUntilOuterObserver parent;
                readonly IDisposable subscription;

                public SkipUntil(SkipUntilOuterObserver parent, IDisposable subscription)
                {
                    this.parent = parent;
                    observer = EmptyObserver<T>.Instance;
                    this.subscription = subscription;
                }

                public void OnNext(T value)
                {
                    observer.OnNext(value);
                }

                public void OnError(Exception error)
                {
                    try { observer.OnError(error); }
                    finally { parent.Dispose(); }
                }

                public void OnCompleted()
                {
                    try { observer.OnCompleted(); }
                    finally { subscription.Dispose(); }
                }
            }

            class SkipUntilOther : IObserver<TOther>
            {
                readonly SkipUntilOuterObserver parent;
                readonly SkipUntil sourceObserver;
                readonly IDisposable subscription;

                public SkipUntilOther(SkipUntilOuterObserver parent, SkipUntil sourceObserver, IDisposable subscription)
                {
                    this.parent = parent;
                    this.sourceObserver = sourceObserver;
                    this.subscription = subscription;
                }

                public void OnNext(TOther value)
                {
                    sourceObserver.observer = parent.observer;
                    subscription.Dispose();
                }

                public void OnError(Exception error)
                {
                    try { parent.observer.OnError(error); } finally { parent.Dispose(); }
                }

                public void OnCompleted()
                {
                    subscription.Dispose();
                }
            }
        }
    }
}