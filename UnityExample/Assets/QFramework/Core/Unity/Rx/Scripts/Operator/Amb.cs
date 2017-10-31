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
    
    public class AmbObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly IObservable<T> second;

        public AmbObservable(IObservable<T> source, IObservable<T> second)
            : base(source.IsRequiredSubscribeOnCurrentThread() || second.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.second = second;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new AmbOuterObserver(this, observer, cancel).Run();
        }

        class AmbOuterObserver : OperatorObserverBase<T, T>
        {
            enum AmbState
            {
                Left, Right, Neither
            }

            readonly AmbObservable<T> parent;
            readonly object gate = new object();
            SingleAssignmentDisposable leftSubscription;
            SingleAssignmentDisposable rightSubscription;
            AmbState choice = AmbState.Neither;

            public AmbOuterObserver(AmbObservable<T> parent, IObserver<T> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                leftSubscription = new SingleAssignmentDisposable();
                rightSubscription = new SingleAssignmentDisposable();
                var d = StableCompositeDisposable.Create(leftSubscription, rightSubscription);

                var left = new Amb();
                left.targetDisposable = d;
                left.targetObserver = new AmbDecisionObserver(this, AmbState.Left, rightSubscription, left);

                var right = new Amb();
                right.targetDisposable = d;
                right.targetObserver = new AmbDecisionObserver(this, AmbState.Right, leftSubscription, right);

                leftSubscription.Disposable = parent.source.Subscribe(left);
                rightSubscription.Disposable = parent.second.Subscribe(right);

                return d;
            }

            public override void OnNext(T value)
            {
                // no use
            }

            public override void OnError(Exception error)
            {
                // no use
            }

            public override void OnCompleted()
            {
                // no use
            }

            class Amb : IObserver<T>
            {
                public IObserver<T> targetObserver;
                public IDisposable targetDisposable;

                public void OnNext(T value)
                {
                    targetObserver.OnNext(value);
                }

                public void OnError(Exception error)
                {
                    try
                    {
                        targetObserver.OnError(error);
                    }
                    finally
                    {
                        targetObserver = EmptyObserver<T>.Instance;
                        targetDisposable.Dispose();
                    }
                }

                public void OnCompleted()
                {
                    try
                    {
                        targetObserver.OnCompleted();
                    }
                    finally
                    {
                        targetObserver = EmptyObserver<T>.Instance;
                        targetDisposable.Dispose();
                    }
                }
            }

            class AmbDecisionObserver : IObserver<T>
            {
                readonly AmbOuterObserver parent;
                readonly AmbState me;
                readonly IDisposable otherSubscription;
                readonly Amb self;

                public AmbDecisionObserver(AmbOuterObserver parent, AmbState me, IDisposable otherSubscription, Amb self)
                {
                    this.parent = parent;
                    this.me = me;
                    this.otherSubscription = otherSubscription;
                    this.self = self;
                }

                public void OnNext(T value)
                {
                    lock (parent.gate)
                    {
                        if (parent.choice == AmbState.Neither)
                        {
                            parent.choice = me;
                            otherSubscription.Dispose();
                            self.targetObserver = parent.observer;
                        }

                        if (parent.choice == me) self.targetObserver.OnNext(value);
                    }
                }

                public void OnError(Exception error)
                {
                    lock (parent.gate)
                    {
                        if (parent.choice == AmbState.Neither)
                        {
                            parent.choice = me;
                            otherSubscription.Dispose();
                            self.targetObserver = parent.observer;
                        }

                        if (parent.choice == me)
                        {
                            self.targetObserver.OnError(error);
                        }
                    }
                }

                public void OnCompleted()
                {
                    lock (parent.gate)
                    {
                        if (parent.choice == AmbState.Neither)
                        {
                            parent.choice = me;
                            otherSubscription.Dispose();
                            self.targetObserver = parent.observer;
                        }

                        if (parent.choice == me)
                        {
                            self.targetObserver.OnCompleted();
                        }
                    }
                }
            }
        }
    }
}