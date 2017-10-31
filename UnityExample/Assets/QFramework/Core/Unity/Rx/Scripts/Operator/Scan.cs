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
    
    public class ScanObservable<TSource> : OperatorObservableBase<TSource>
    {
        readonly IObservable<TSource> source;
        readonly Func<TSource, TSource, TSource> accumulator;

        public ScanObservable(IObservable<TSource> source, Func<TSource, TSource, TSource> accumulator)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.accumulator = accumulator;
        }

        protected override IDisposable SubscribeCore(IObserver<TSource> observer, IDisposable cancel)
        {
            return source.Subscribe(new Scan(this, observer, cancel));
        }

        class Scan : OperatorObserverBase<TSource, TSource>
        {
            readonly ScanObservable<TSource> parent;
            TSource accumulation;
            bool isFirst;

            public Scan(ScanObservable<TSource> parent, IObserver<TSource> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
                this.isFirst = true;
            }

            public override void OnNext(TSource value)
            {
                if (isFirst)
                {
                    isFirst = false;
                    accumulation = value;
                }
                else
                {
                    try
                    {
                        accumulation = parent.accumulator(accumulation, value);
                    }
                    catch (Exception ex)
                    {
                        try { observer.OnError(ex); }
                        finally { Dispose(); }
                        return;
                    }
                }

                observer.OnNext(accumulation);
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

    public class ScanObservable<TSource, TAccumulate> : OperatorObservableBase<TAccumulate>
    {
        readonly IObservable<TSource> source;
        readonly TAccumulate seed;
        readonly Func<TAccumulate, TSource, TAccumulate> accumulator;

        public ScanObservable(IObservable<TSource> source, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> accumulator)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.seed = seed;
            this.accumulator = accumulator;
        }

        protected override IDisposable SubscribeCore(IObserver<TAccumulate> observer, IDisposable cancel)
        {
            return source.Subscribe(new Scan(this, observer, cancel));
        }

        class Scan : OperatorObserverBase<TSource, TAccumulate>
        {
            readonly ScanObservable<TSource, TAccumulate> parent;
            TAccumulate accumulation;
            bool isFirst;

            public Scan(ScanObservable<TSource, TAccumulate> parent, IObserver<TAccumulate> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
                this.isFirst = true;
            }

            public override void OnNext(TSource value)
            {
                if (isFirst)
                {
                    isFirst = false;
                    accumulation = parent.seed;
                }

                try
                {
                    accumulation = parent.accumulator(accumulation, value);
                }
                catch (Exception ex)
                {
                    try { observer.OnError(ex); }
                    finally { Dispose(); }
                    return;
                }

                observer.OnNext(accumulation);
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