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
    
    public class FinallyObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly Action finallyAction;

        public FinallyObservable(IObservable<T> source, Action finallyAction)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.finallyAction = finallyAction;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new Finally(this, observer, cancel).Run();
        }

        class Finally : OperatorObserverBase<T, T>
        {
            readonly FinallyObservable<T> parent;

            public Finally(FinallyObservable<T> parent, IObserver<T> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                IDisposable subscription;
                try
                {
                    subscription = parent.source.Subscribe(this);
                }
                catch
                {
                    // This behaviour is not same as .NET Official Rx
                    parent.finallyAction();
                    throw;
                }

                return StableCompositeDisposable.Create(subscription, Disposable.Create(() =>
                {
                    parent.finallyAction();
                }));
            }

            public override void OnNext(T value)
            {
                base.observer.OnNext(value);
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