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

    public class ContinueWithObservable<TSource, TResult> : OperatorObservableBase<TResult>
    {
        readonly IObservable<TSource> source;
        readonly Func<TSource, IObservable<TResult>> selector;

        public ContinueWithObservable(IObservable<TSource> source, Func<TSource, IObservable<TResult>> selector)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.selector = selector;
        }

        protected override IDisposable SubscribeCore(IObserver<TResult> observer, IDisposable cancel)
        {
            return new ContinueWith(this, observer, cancel).Run();
        }

        class ContinueWith : OperatorObserverBase<TSource, TResult>
        {
            readonly ContinueWithObservable<TSource, TResult> parent;
            readonly SerialDisposable serialDisposable = new SerialDisposable();

            bool seenValue;
            TSource lastValue;

            public ContinueWith(ContinueWithObservable<TSource, TResult> parent, IObserver<TResult> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                var sourceDisposable = new SingleAssignmentDisposable();
                serialDisposable.Disposable = sourceDisposable;

                sourceDisposable.Disposable = parent.source.Subscribe(this);
                return serialDisposable;
            }

            public override void OnNext(TSource value)
            {
                this.seenValue = true;
                this.lastValue = value;
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); } finally { Dispose(); };
            }

            public override void OnCompleted()
            {
                if (seenValue)
                {
                    var v = parent.selector(lastValue);
                    // dispose source subscription
                    serialDisposable.Disposable = v.Subscribe(observer);
                }
                else
                {
                    try { observer.OnCompleted(); } finally { Dispose(); };
                }
            }
        }
    }
}