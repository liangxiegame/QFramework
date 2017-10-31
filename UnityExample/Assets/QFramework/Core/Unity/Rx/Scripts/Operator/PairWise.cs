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

    public class PairwiseObservable<T, TR> : OperatorObservableBase<TR>
    {
        readonly IObservable<T> source;
        readonly Func<T, T, TR> selector;

        public PairwiseObservable(IObservable<T> source, Func<T, T, TR> selector)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.selector = selector;
        }

        protected override IDisposable SubscribeCore(IObserver<TR> observer, IDisposable cancel)
        {
            return source.Subscribe(new Pairwise(this, observer, cancel));
        }

        class Pairwise : OperatorObserverBase<T, TR>
        {
            readonly PairwiseObservable<T, TR> parent;
            T prev = default(T);
            bool isFirst = true;

            public Pairwise(PairwiseObservable<T, TR> parent, IObserver<TR> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public override void OnNext(T value)
            {
                if (isFirst)
                {
                    isFirst = false;
                    prev = value;
                    return;
                }

                TR v;
                try
                {
                    v = parent.selector(prev, value);
                    prev = value;
                }
                catch (Exception ex)
                {
                    try { observer.OnError(ex); } finally { Dispose(); }
                    return;
                }

                observer.OnNext(v);
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
    }

    public class PairwiseObservable<T> : OperatorObservableBase<Pair<T>>
    {
        readonly IObservable<T> source;

        public PairwiseObservable(IObservable<T> source)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
        }

        protected override IDisposable SubscribeCore(IObserver<Pair<T>> observer, IDisposable cancel)
        {
            return source.Subscribe(new Pairwise(observer, cancel));
        }

        class Pairwise : OperatorObserverBase<T, Pair<T>>
        {
            T prev = default(T);
            bool isFirst = true;

            public Pairwise(IObserver<Pair<T>> observer, IDisposable cancel)
                : base(observer, cancel)
            {
            }

            public override void OnNext(T value)
            {
                if (isFirst)
                {
                    isFirst = false;
                    prev = value;
                    return;
                }

                var pair = new Pair<T>(prev, value);
                prev = value;
                observer.OnNext(pair);
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
    }
}