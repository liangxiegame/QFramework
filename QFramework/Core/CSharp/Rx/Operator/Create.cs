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

// this code is borrowed from RxOfficial(rx.codeplex.com) and modified

// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace QFramework
{
    using System;

    public class CreateObservable<T> : OperatorObservableBase<T>
    {
        readonly Func<IObserver<T>, IDisposable> subscribe;

        public CreateObservable(Func<IObserver<T>, IDisposable> subscribe)
            : base(true) // fail safe
        {
            this.subscribe = subscribe;
        }

        public CreateObservable(Func<IObserver<T>, IDisposable> subscribe, bool isRequiredSubscribeOnCurrentThread)
            : base(isRequiredSubscribeOnCurrentThread)
        {
            this.subscribe = subscribe;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            observer = new Create(observer, cancel);
            return subscribe(observer) ?? Disposable.Empty;
        }

        class Create : OperatorObserverBase<T, T>
        {
            public Create(IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
            }

            public override void OnNext(T value)
            {
                base.observer.OnNext(value);
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); }
                finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                try { observer.OnCompleted(); }
                finally { Dispose(); }
            }
        }
    }

    public class CreateObservable<T, TState> : OperatorObservableBase<T>
    {
        readonly TState state;
        readonly Func<TState, IObserver<T>, IDisposable> subscribe;

        public CreateObservable(TState state, Func<TState, IObserver<T>, IDisposable> subscribe)
            : base(true) // fail safe
        {
            this.state = state;
            this.subscribe = subscribe;
        }

        public CreateObservable(TState state, Func<TState, IObserver<T>, IDisposable> subscribe, bool isRequiredSubscribeOnCurrentThread)
            : base(isRequiredSubscribeOnCurrentThread)
        {
            this.state = state;
            this.subscribe = subscribe;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            observer = new Create(observer, cancel);
            return subscribe(state, observer) ?? Disposable.Empty;
        }

        class Create : OperatorObserverBase<T, T>
        {
            public Create(IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
            }

            public override void OnNext(T value)
            {
                base.observer.OnNext(value);
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); }
                finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                try { observer.OnCompleted(); }
                finally { Dispose(); }
            }
        }
    }

    public class CreateSafeObservable<T> : OperatorObservableBase<T>
    {
        readonly Func<IObserver<T>, IDisposable> subscribe;

        public CreateSafeObservable(Func<IObserver<T>, IDisposable> subscribe)
            : base(true) // fail safe
        {
            this.subscribe = subscribe;
        }

        public CreateSafeObservable(Func<IObserver<T>, IDisposable> subscribe, bool isRequiredSubscribeOnCurrentThread)
            : base(isRequiredSubscribeOnCurrentThread)
        {
            this.subscribe = subscribe;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            observer = new CreateSafe(observer, cancel);
            return subscribe(observer) ?? Disposable.Empty;
        }

        class CreateSafe : OperatorObserverBase<T, T>
        {
            public CreateSafe(IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
            }

            public override void OnNext(T value)
            {
                try
                {
                    base.observer.OnNext(value);
                }
                catch
                {
                    Dispose(); // safe
                    throw;
                }
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); }
                finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                try { observer.OnCompleted(); }
                finally { Dispose(); }
            }
        }
    }
}