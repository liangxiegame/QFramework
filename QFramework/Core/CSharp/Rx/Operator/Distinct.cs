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
    
    public class DistinctObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly IEqualityComparer<T> comparer;

        public DistinctObservable(IObservable<T> source, IEqualityComparer<T> comparer)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.comparer = comparer;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return source.Subscribe(new Distinct(this, observer, cancel));
        }

        class Distinct : OperatorObserverBase<T, T>
        {
            readonly HashSet<T> hashSet;

            public Distinct(DistinctObservable<T> parent, IObserver<T> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                hashSet = (parent.comparer == null)
                    ? new HashSet<T>()
                    : new HashSet<T>(parent.comparer);
            }

            public override void OnNext(T value)
            {
                var key = default(T);
                var isAdded = false;
                try
                {
                    key = value;
                    isAdded = hashSet.Add(key);
                }
                catch (Exception exception)
                {
                    try { observer.OnError(exception); } finally { Dispose(); }
                    return;
                }

                if (isAdded)
                {
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

    public class DistinctObservable<T, TKey> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly IEqualityComparer<TKey> comparer;
        readonly Func<T, TKey> keySelector;

        public DistinctObservable(IObservable<T> source, Func<T, TKey> keySelector, IEqualityComparer<TKey> comparer)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.comparer = comparer;
            this.keySelector = keySelector;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return source.Subscribe(new Distinct(this, observer, cancel));
        }

        class Distinct : OperatorObserverBase<T, T>
        {
            readonly DistinctObservable<T, TKey> parent;
            readonly HashSet<TKey> hashSet;

            public Distinct(DistinctObservable<T, TKey> parent, IObserver<T> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
                hashSet = (parent.comparer == null)
                    ? new HashSet<TKey>()
                    : new HashSet<TKey>(parent.comparer);
            }

            public override void OnNext(T value)
            {
                var key = default(TKey);
                var isAdded = false;
                try
                {
                    key = parent.keySelector(value);
                    isAdded = hashSet.Add(key);
                }
                catch (Exception exception)
                {
                    try { observer.OnError(exception); } finally { Dispose(); }
                    return;
                }

                if (isAdded)
                {
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