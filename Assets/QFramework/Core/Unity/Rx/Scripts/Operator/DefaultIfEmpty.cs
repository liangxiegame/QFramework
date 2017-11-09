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

    public class DefaultIfEmptyObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly T defaultValue;

        public DefaultIfEmptyObservable(IObservable<T> source, T defaultValue)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.defaultValue = defaultValue;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return source.Subscribe(new DefaultIfEmpty(this, observer, cancel));
        }

        class DefaultIfEmpty : OperatorObserverBase<T, T>
        {
            readonly DefaultIfEmptyObservable<T> parent;
            bool hasValue;

            public DefaultIfEmpty(DefaultIfEmptyObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
                this.hasValue = false;
            }

            public override void OnNext(T value)
            {
                hasValue = true;
                observer.OnNext(value);
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); }
                finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                if (!hasValue)
                {
                    observer.OnNext(parent.defaultValue);
                }

                try { observer.OnCompleted(); }
                finally { Dispose(); }
            }
        }
    }
}