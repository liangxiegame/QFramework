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

    public class SingleObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly bool useDefault;
        readonly Func<T, bool> predicate;

        public SingleObservable(IObservable<T> source, bool useDefault)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.useDefault = useDefault;
        }

        public SingleObservable(IObservable<T> source, Func<T, bool> predicate, bool useDefault)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.predicate = predicate;
            this.useDefault = useDefault;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            if (predicate == null)
            {
                return source.Subscribe(new Single(this, observer, cancel));
            }
            else
            {
                return source.Subscribe(new Single_(this, observer, cancel));
            }
        }

        class Single : OperatorObserverBase<T, T>
        {
            readonly SingleObservable<T> parent;
            bool seenValue;
            T lastValue;

            public Single(SingleObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
                this.seenValue = false;
            }

            public override void OnNext(T value)
            {
                if (seenValue)
                {
                    try { observer.OnError(new InvalidOperationException("sequence is not single")); }
                    finally { Dispose(); }
                }
                else
                {
                    seenValue = true;
                    lastValue = value;
                }
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); }
                finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                if (parent.useDefault)
                {
                    if (!seenValue)
                    {
                        observer.OnNext(default(T));
                    }
                    else
                    {
                        observer.OnNext(lastValue);
                    }
                    try { observer.OnCompleted(); }
                    finally { Dispose(); }
                }
                else
                {
                    if (!seenValue)
                    {
                        try { observer.OnError(new InvalidOperationException("sequence is empty")); }
                        finally { Dispose(); }
                    }
                    else
                    {
                        observer.OnNext(lastValue);
                        try { observer.OnCompleted(); }
                        finally { Dispose(); }
                    }
                }
            }
        }

        class Single_ : OperatorObserverBase<T, T>
        {
            readonly SingleObservable<T> parent;
            bool seenValue;
            T lastValue;

            public Single_(SingleObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
                this.seenValue = false;
            }

            public override void OnNext(T value)
            {
                bool isPassed;
                try
                {
                    isPassed = parent.predicate(value);
                }
                catch (Exception ex)
                {
                    try { observer.OnError(ex); }
                    finally { Dispose(); }
                    return;
                }

                if (isPassed)
                {
                    if (seenValue)
                    {
                        try { observer.OnError(new InvalidOperationException("sequence is not single")); }
                        finally { Dispose(); }
                        return;
                    }
                    else
                    {
                        seenValue = true;
                        lastValue = value;
                    }
                }
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); }
                finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                if (parent.useDefault)
                {
                    if (!seenValue)
                    {
                        observer.OnNext(default(T));
                    }
                    else
                    {
                        observer.OnNext(lastValue);
                    }
                    try { observer.OnCompleted(); }
                    finally { Dispose(); }
                }
                else
                {
                    if (!seenValue)
                    {
                        try { observer.OnError(new InvalidOperationException("sequence is empty")); }
                        finally { Dispose(); }
                    }
                    else
                    {
                        observer.OnNext(lastValue);
                        try { observer.OnCompleted(); }
                        finally { Dispose(); }
                    }
                }
            }
        }
    }
}