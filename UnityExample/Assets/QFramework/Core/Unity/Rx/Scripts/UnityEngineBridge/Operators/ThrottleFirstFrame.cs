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

#if UniRxLibrary
using UnityObservable = QFramework.ObservableUnity;
#else
    using UnityObservable = Observable;
#endif
    
    
    public class ThrottleFirstFrameObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly int frameCount;
        readonly FrameCountType frameCountType;

        public ThrottleFirstFrameObservable(IObservable<T> source, int frameCount, FrameCountType frameCountType) : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.frameCount = frameCount;
            this.frameCountType = frameCountType;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new ThrottleFirstFrame(this, observer, cancel).Run();
        }

        class ThrottleFirstFrame : OperatorObserverBase<T, T>
        {
            readonly ThrottleFirstFrameObservable<T> parent;
            readonly object gate = new object();
            bool open = true;
            SerialDisposable cancelable;

            ThrottleFirstFrameTick tick;

            public ThrottleFirstFrame(ThrottleFirstFrameObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                tick = new ThrottleFirstFrameTick(this);
                cancelable = new SerialDisposable();

                var subscription = parent.source.Subscribe(this);
                return StableCompositeDisposable.Create(cancelable, subscription);
            }

            void OnNext()
            {
                lock (gate)
                {
                    open = true;
                }
            }

            public override void OnNext(T value)
            {
                lock (gate)
                {
                    if (!open) return;
                    observer.OnNext(value);
                    open = false;
                }

                var d = new SingleAssignmentDisposable();
                cancelable.Disposable = d;
                d.Disposable = UnityObservable.TimerFrame(parent.frameCount, parent.frameCountType)
                    .Subscribe(tick);
            }

            public override void OnError(Exception error)
            {
                cancelable.Dispose();

                lock (gate)
                {
                    try { observer.OnError(error); } finally { Dispose(); }
                }
            }

            public override void OnCompleted()
            {
                cancelable.Dispose();

                lock (gate)
                {
                    try { observer.OnCompleted(); } finally { Dispose(); }
                }
            }

            // immutable, can share.
            class ThrottleFirstFrameTick : IObserver<long>
            {
                readonly ThrottleFirstFrame parent;

                public ThrottleFirstFrameTick(ThrottleFirstFrame parent)
                {
                    this.parent = parent;
                }

                public void OnCompleted()
                {
                }

                public void OnError(Exception error)
                {
                }

                public void OnNext(long _)
                {
                    lock (parent.gate)
                    {
                        parent.open = true;
                    }
                }
            }
        }
    }
}