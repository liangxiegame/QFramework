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
    
    
    public class ThrottleFrameObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly int frameCount;
        readonly FrameCountType frameCountType;

        public ThrottleFrameObservable(IObservable<T> source, int frameCount, FrameCountType frameCountType) : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.frameCount = frameCount;
            this.frameCountType = frameCountType;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new ThrottleFrame(this, observer, cancel).Run();
        }

        class ThrottleFrame : OperatorObserverBase<T, T>
        {
            readonly ThrottleFrameObservable<T> parent;
            readonly object gate = new object();
            T latestValue = default(T);
            bool hasValue = false;
            SerialDisposable cancelable;
            ulong id = 0;

            public ThrottleFrame(ThrottleFrameObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                cancelable = new SerialDisposable();
                var subscription = parent.source.Subscribe(this);

                return StableCompositeDisposable.Create(cancelable, subscription);
            }

            public override void OnNext(T value)
            {
                ulong currentid;
                lock (gate)
                {
                    hasValue = true;
                    latestValue = value;
                    id = unchecked(id + 1);
                    currentid = id;
                }

                var d = new SingleAssignmentDisposable();
                cancelable.Disposable = d;
                d.Disposable = UnityObservable.TimerFrame(parent.frameCount, parent.frameCountType)
                    .Subscribe(new ThrottleFrameTick(this, currentid));
            }

            public override void OnError(Exception error)
            {
                cancelable.Dispose();

                lock (gate)
                {
                    hasValue = false;
                    id = unchecked(id + 1);
                    try { observer.OnError(error); } finally { Dispose(); }
                }
            }

            public override void OnCompleted()
            {
                cancelable.Dispose();

                lock (gate)
                {
                    if (hasValue)
                    {
                        observer.OnNext(latestValue);
                    }
                    hasValue = false;
                    id = unchecked(id + 1);
                    try { observer.OnCompleted(); } finally { Dispose(); }
                }
            }

            class ThrottleFrameTick : IObserver<long>
            {
                readonly ThrottleFrame parent;
                readonly ulong currentid;

                public ThrottleFrameTick(ThrottleFrame parent, ulong currentid)
                {
                    this.parent = parent;
                    this.currentid = currentid;
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
                        if (parent.hasValue && parent.id == currentid)
                        {
                            parent.observer.OnNext(parent.latestValue);
                        }
                        parent.hasValue = false;
                    }
                }
            }
        }
    }
}