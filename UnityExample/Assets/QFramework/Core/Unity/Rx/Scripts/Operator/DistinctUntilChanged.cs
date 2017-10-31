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
    using System.Collections.Generic;
    
    public class DistinctUntilChangedObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly IEqualityComparer<T> comparer;

        public DistinctUntilChangedObservable(IObservable<T> source, IEqualityComparer<T> comparer)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.comparer = comparer;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return source.Subscribe(new DistinctUntilChanged(this, observer, cancel));
        }

        class DistinctUntilChanged : OperatorObserverBase<T, T>
        {
            readonly DistinctUntilChangedObservable<T> parent;
            bool isFirst = true;
            T prevKey = default(T);

            public DistinctUntilChanged(DistinctUntilChangedObservable<T> parent, IObserver<T> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public override void OnNext(T value)
            {
                T currentKey;
                try
                {
                    currentKey = value;
                }
                catch (Exception exception)
                {
                    try { observer.OnError(exception); } finally { Dispose(); }
                    return;
                }

                var sameKey = false;
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    try
                    {
                        sameKey = parent.comparer.Equals(currentKey, prevKey);
                    }
                    catch (Exception ex)
                    {
                        try { observer.OnError(ex); } finally { Dispose(); }
                        return;
                    }
                }

                if (!sameKey)
                {
                    prevKey = currentKey;
                    observer.OnNext(value);
                }
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

    public class DistinctUntilChangedObservable<T, TKey> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly IEqualityComparer<TKey> comparer;
        readonly Func<T, TKey> keySelector;

        public DistinctUntilChangedObservable(IObservable<T> source, Func<T, TKey> keySelector, IEqualityComparer<TKey> comparer)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.comparer = comparer;
            this.keySelector = keySelector;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return source.Subscribe(new DistinctUntilChanged(this, observer, cancel));
        }

        class DistinctUntilChanged : OperatorObserverBase<T, T>
        {
            readonly DistinctUntilChangedObservable<T, TKey> parent;
            bool isFirst = true;
            TKey prevKey = default(TKey);

            public DistinctUntilChanged(DistinctUntilChangedObservable<T, TKey> parent, IObserver<T> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public override void OnNext(T value)
            {
                TKey currentKey;
                try
                {
                    currentKey = parent.keySelector(value);
                }
                catch (Exception exception)
                {
                    try { observer.OnError(exception); } finally { Dispose(); }
                    return;
                }

                var sameKey = false;
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    try
                    {
                        sameKey = parent.comparer.Equals(currentKey, prevKey);
                    }
                    catch (Exception ex)
                    {
                        try { observer.OnError(ex); } finally { Dispose(); }
                        return;
                    }
                }

                if (!sameKey)
                {
                    prevKey = currentKey;
                    observer.OnNext(value);
                }
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