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
    
    public class ForEachAsyncObservable<T> : OperatorObservableBase<Unit>
    {
        readonly IObservable<T> source;
        readonly Action<T> onNext;
        readonly Action<T, int> onNextWithIndex;

        public ForEachAsyncObservable(IObservable<T> source, Action<T> onNext)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.onNext = onNext;
        }

        public ForEachAsyncObservable(IObservable<T> source, Action<T, int> onNext)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.onNextWithIndex = onNext;
        }

        protected override IDisposable SubscribeCore(IObserver<Unit> observer, IDisposable cancel)
        {
            if (onNext != null)
            {
                return source.Subscribe(new ForEachAsync(this, observer, cancel));
            }
            else
            {
                return source.Subscribe(new ForEachAsync_(this, observer, cancel));
            }
        }

        class ForEachAsync : OperatorObserverBase<T, Unit>
        {
            readonly ForEachAsyncObservable<T> parent;

            public ForEachAsync(ForEachAsyncObservable<T> parent, IObserver<Unit> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public override void OnNext(T value)
            {
                try
                {
                    parent.onNext(value);
                }
                catch (Exception ex)
                {
                    try { observer.OnError(ex); }
                    finally { Dispose(); }
                    return;
                }
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); }
                finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                observer.OnNext(Unit.Default);

                try { observer.OnCompleted(); }
                finally { Dispose(); }
            }
        }

        // with index
        class ForEachAsync_ : OperatorObserverBase<T, Unit>
        {
            readonly ForEachAsyncObservable<T> parent;
            int index = 0;

            public ForEachAsync_(ForEachAsyncObservable<T> parent, IObserver<Unit> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public override void OnNext(T value)
            {
                try
                {
                    parent.onNextWithIndex(value, index++);
                }
                catch (Exception ex)
                {
                    try { observer.OnError(ex); }
                    finally { Dispose(); }
                    return;
                }
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); }
                finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                observer.OnNext(Unit.Default);

                try { observer.OnCompleted(); }
                finally { Dispose(); }
            }
        }
    }
}