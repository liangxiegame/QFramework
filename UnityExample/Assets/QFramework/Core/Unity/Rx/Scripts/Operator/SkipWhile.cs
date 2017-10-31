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

    public class SkipWhileObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly Func<T, bool> predicate;
        readonly Func<T, int, bool> predicateWithIndex;

        public SkipWhileObservable(IObservable<T> source, Func<T, bool> predicate)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.predicate = predicate;
        }

        public SkipWhileObservable(IObservable<T> source, Func<T, int, bool> predicateWithIndex)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.predicateWithIndex = predicateWithIndex;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            if (predicate != null)
            {
                return new SkipWhile(this, observer, cancel).Run();
            }
            else
            {
                return new SkipWhile_(this, observer, cancel).Run();
            }
        }

        class SkipWhile : OperatorObserverBase<T, T>
        {
            readonly SkipWhileObservable<T> parent;
            bool endSkip = false;

            public SkipWhile(SkipWhileObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                return parent.source.Subscribe(this);
            }

            public override void OnNext(T value)
            {
                if (!endSkip)
                {
                    try
                    {
                        endSkip = !parent.predicate(value);
                    }
                    catch (Exception ex)
                    {
                        try { observer.OnError(ex); } finally { Dispose(); }
                        return;
                    }

                    if (!endSkip) return;
                }

                observer.OnNext(value);
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

        class SkipWhile_ : OperatorObserverBase<T, T>
        {
            readonly SkipWhileObservable<T> parent;
            bool endSkip = false;
            int index = 0;

            public SkipWhile_(SkipWhileObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                return parent.source.Subscribe(this);
            }

            public override void OnNext(T value)
            {
                if (!endSkip)
                {
                    try
                    {
                        endSkip = !parent.predicateWithIndex(value, index++);
                    }
                    catch (Exception ex)
                    {
                        try { observer.OnError(ex); } finally { Dispose(); }
                        return;
                    }

                    if (!endSkip) return;
                }

                observer.OnNext(value);
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