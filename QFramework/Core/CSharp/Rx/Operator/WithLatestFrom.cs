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
    
    public class WithLatestFromObservable<TLeft, TRight, TResult> : OperatorObservableBase<TResult>
    {
        readonly IObservable<TLeft> left;
        readonly IObservable<TRight> right;
        readonly Func<TLeft, TRight, TResult> selector;

        public WithLatestFromObservable(IObservable<TLeft> left, IObservable<TRight> right, Func<TLeft, TRight, TResult> selector)
            : base(left.IsRequiredSubscribeOnCurrentThread() || right.IsRequiredSubscribeOnCurrentThread())
        {
            this.left = left;
            this.right = right;
            this.selector = selector;
        }

        protected override IDisposable SubscribeCore(IObserver<TResult> observer, IDisposable cancel)
        {
            return new WithLatestFrom(this, observer, cancel).Run();
        }

        class WithLatestFrom : OperatorObserverBase<TResult, TResult>
        {
            readonly WithLatestFromObservable<TLeft, TRight, TResult> parent;
            readonly object gate = new object();

            volatile bool hasLatest;
            TRight latestValue = default(TRight);

            public WithLatestFrom(WithLatestFromObservable<TLeft, TRight, TResult> parent, IObserver<TResult> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                var l = parent.left.Subscribe(new LeftObserver(this));
                var rSubscription = new SingleAssignmentDisposable();
                rSubscription.Disposable  = parent.right.Subscribe(new RightObserver(this, rSubscription));

                return StableCompositeDisposable.Create(l, rSubscription);
            }

            public override void OnNext(TResult value)
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
                try { observer.OnCompleted(); }
                finally { Dispose(); }
            }

            class LeftObserver : IObserver<TLeft>
            {
                readonly WithLatestFrom parent;

                public LeftObserver(WithLatestFrom parent)
                {
                    this.parent = parent;
                }

                public void OnNext(TLeft value)
                {
                    if (parent.hasLatest)
                    {
                        var result = default(TResult);
                        try
                        {
                            result = parent.parent.selector(value, parent.latestValue);
                        }
                        catch (Exception ex)
                        {
                            lock (parent.gate)
                            {
                                parent.OnError(ex);
                            }
                            return;
                        }

                        lock (parent.gate)
                        {
                            parent.OnNext(result);
                        }
                    }
                }

                public void OnError(Exception error)
                {
                    lock (parent.gate)
                    {
                        parent.OnError(error);
                    }
                }

                public void OnCompleted()
                {
                    lock (parent.gate)
                    {
                        parent.OnCompleted();
                    }
                }
            }

            class RightObserver : IObserver<TRight>
            {
                readonly WithLatestFrom parent;
                readonly IDisposable selfSubscription;

                public RightObserver(WithLatestFrom parent, IDisposable subscription)
                {
                    this.parent = parent;
                    this.selfSubscription = subscription;
                }

                public void OnNext(TRight value)
                {
                    parent.latestValue = value;
                    parent.hasLatest = true;
                }

                public void OnError(Exception error)
                {
                    lock (parent.gate)
                    {
                        parent.OnError(error);
                    }
                }

                public void OnCompleted()
                {
                    selfSubscription.Dispose();
                }
            }
        }
    }
}