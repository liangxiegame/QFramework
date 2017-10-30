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

    public class LastObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly bool useDefault;
        readonly Func<T, bool> predicate;

        public LastObservable(IObservable<T> source, bool useDefault)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.useDefault = useDefault;
        }

        public LastObservable(IObservable<T> source, Func<T, bool> predicate, bool useDefault)
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
                return source.Subscribe(new Last(this, observer, cancel));
            }
            else
            {
                return source.Subscribe(new Last_(this, observer, cancel));
            }
        }

        class Last : OperatorObserverBase<T, T>
        {
            readonly LastObservable<T> parent;
            bool notPublished;
            T lastValue;

            public Last(LastObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
                this.notPublished = true;
            }

            public override void OnNext(T value)
            {
                notPublished = false;
                lastValue = value;
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
                    if (notPublished)
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
                    if (notPublished)
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

        class Last_ : OperatorObserverBase<T, T>
        {
            readonly LastObservable<T> parent;
            bool notPublished;
            T lastValue;

            public Last_(LastObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
                this.notPublished = true;
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
                    notPublished = false;
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
                    if (notPublished)
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
                    if (notPublished)
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