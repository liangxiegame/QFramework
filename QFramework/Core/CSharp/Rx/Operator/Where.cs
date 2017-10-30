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
    
    public class WhereObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly Func<T, bool> predicate;
        readonly Func<T, int, bool> predicateWithIndex;

        public WhereObservable(IObservable<T> source, Func<T, bool> predicate)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.predicate = predicate;
        }

        public WhereObservable(IObservable<T> source, Func<T, int, bool> predicateWithIndex)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.predicateWithIndex = predicateWithIndex;
        }

        // Optimize for .Where().Where()

        public IObservable<T> CombinePredicate(Func<T, bool> combinePredicate)
        {
            if (this.predicate != null)
            {
                return new WhereObservable<T>(source, x => this.predicate(x) && combinePredicate(x));
            }
            else
            {
                return new WhereObservable<T>(this, combinePredicate);
            }
        }

        // Optimize for .Where().Select()

        public IObservable<TR> CombineSelector<TR>(Func<T, TR> selector)
        {
            if (this.predicate != null)
            {
                return new WhereSelectObservable<T, TR>(source, predicate, selector);
            }
            else
            {
                return new SelectObservable<T, TR>(this, selector); // can't combine
            }
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            if (predicate != null)
            {
                return source.Subscribe(new Where(this, observer, cancel));
            }
            else
            {
                return source.Subscribe(new Where_(this, observer, cancel));
            }
        }

        class Where : OperatorObserverBase<T, T>
        {
            readonly WhereObservable<T> parent;

            public Where(WhereObservable<T> parent, IObserver<T> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public override void OnNext(T value)
            {
                var isPassed = false;
                try
                {
                    isPassed = parent.predicate(value);
                }
                catch (Exception ex)
                {
                    try { observer.OnError(ex); } finally { Dispose(); }
                    return;
                }

                if (isPassed)
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

        class Where_ : OperatorObserverBase<T, T>
        {
            readonly WhereObservable<T> parent;
            int index;

            public Where_(WhereObservable<T> parent, IObserver<T> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
                this.index = 0;
            }

            public override void OnNext(T value)
            {
                var isPassed = false;
                try
                {
                    isPassed = parent.predicateWithIndex(value, index++);
                }
                catch (Exception ex)
                {
                    try { observer.OnError(ex); } finally { Dispose(); }
                    return;
                }

                if (isPassed)
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