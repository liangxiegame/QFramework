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

    public interface ISelect<TR>
    {
        // IObservable<TR2> CombineSelector<TR2>(Func<TR, TR2> selector);
        IObservable<TR> CombinePredicate(Func<TR, bool> predicate);
    }

    public class SelectObservable<T, TR> : OperatorObservableBase<TR>, ISelect<TR>
    {
        readonly IObservable<T> source;
        readonly Func<T, TR> selector;
        readonly Func<T, int, TR> selectorWithIndex;

        public SelectObservable(IObservable<T> source, Func<T, TR> selector)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.selector = selector;
        }

        public SelectObservable(IObservable<T> source, Func<T, int, TR> selector)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.selectorWithIndex = selector;
        }

        // sometimes cause "which no ahead of time (AOT) code was generated." on IL2CPP...

        //public IObservable<TR2> CombineSelector<TR2>(Func<TR, TR2> combineSelector)
        //{
        //    if (this.selector != null)
        //    {
        //        return new Select<T, TR2>(source, x => combineSelector(this.selector(x)));
        //    }
        //    else
        //    {
        //        return new Select<TR, TR2>(this, combineSelector);
        //    }
        //}

        public IObservable<TR> CombinePredicate(Func<TR, bool> predicate)
        {
            if (this.selector != null)
            {
                return new SelectWhereObservable<T, TR>(this.source, this.selector, predicate);
            }
            else
            {
                return new WhereObservable<TR>(this, predicate); // can't combine
            }
        }

        protected override IDisposable SubscribeCore(IObserver<TR> observer, IDisposable cancel)
        {
            if (selector != null)
            {
                return source.Subscribe(new Select(this, observer, cancel));
            }
            else
            {
                return source.Subscribe(new Select_(this, observer, cancel));
            }
        }

        class Select : OperatorObserverBase<T, TR>
        {
            readonly SelectObservable<T, TR> parent;

            public Select(SelectObservable<T, TR> parent, IObserver<TR> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
            }

            public override void OnNext(T value)
            {
                var v = default(TR);
                try
                {
                    v = parent.selector(value);
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

        // with Index
        class Select_ : OperatorObserverBase<T, TR>
        {
            readonly SelectObservable<T, TR> parent;
            int index;

            public Select_(SelectObservable<T, TR> parent, IObserver<TR> observer, IDisposable cancel)
                : base(observer, cancel)
            {
                this.parent = parent;
                this.index = 0;
            }

            public override void OnNext(T value)
            {
                var v = default(TR);
                try
                {
                    v = parent.selectorWithIndex(value, index++);
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
}