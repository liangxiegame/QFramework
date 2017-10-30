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

    public class TimeoutObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly TimeSpan? dueTime;
        readonly DateTimeOffset? dueTimeDT;
        readonly IScheduler scheduler;

        public TimeoutObservable(IObservable<T> source, TimeSpan dueTime, IScheduler scheduler) 
            : base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.dueTime = dueTime;
            this.scheduler = scheduler;
        }

        public TimeoutObservable(IObservable<T> source, DateTimeOffset dueTime, IScheduler scheduler) 
            : base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.dueTimeDT = dueTime;
            this.scheduler = scheduler;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            if (dueTime != null)
            {
                return new Timeout(this, observer, cancel).Run();
            }
            else
            {
                return new Timeout_(this, observer, cancel).Run();
            }
        }

        class Timeout : OperatorObserverBase<T, T>
        {
            readonly TimeoutObservable<T> parent;
            readonly object gate = new object();
            ulong objectId = 0ul;
            bool isTimeout = false;
            SingleAssignmentDisposable sourceSubscription;
            SerialDisposable timerSubscription;

            public Timeout(TimeoutObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                sourceSubscription = new SingleAssignmentDisposable();
                timerSubscription = new SerialDisposable();
                timerSubscription.Disposable = RunTimer(objectId);
                sourceSubscription.Disposable = parent.source.Subscribe(this);

                return StableCompositeDisposable.Create(timerSubscription, sourceSubscription);
            }

            IDisposable RunTimer(ulong timerId)
            {
                return parent.scheduler.Schedule(parent.dueTime.Value, () =>
                {
                    lock (gate)
                    {
                        if (objectId == timerId)
                        {
                            isTimeout = true;
                        }
                    }
                    if (isTimeout)
                    {
                        try { observer.OnError(new TimeoutException()); } finally { Dispose(); }
                    }
                });
            }

            public override void OnNext(T value)
            {
                ulong useObjectId;
                bool timeout;
                lock (gate)
                {
                    timeout = isTimeout;
                    objectId++;
                    useObjectId = objectId;
                }
                if (timeout) return;

                timerSubscription.Disposable = Disposable.Empty; // cancel old timer
                observer.OnNext(value);
                timerSubscription.Disposable = RunTimer(useObjectId);
            }

            public override void OnError(Exception error)
            {
                bool timeout;
                lock (gate)
                {
                    timeout = isTimeout;
                    objectId++;
                }
                if (timeout) return;

                timerSubscription.Dispose();
                try { observer.OnError(error); } finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                bool timeout;
                lock (gate)
                {
                    timeout = isTimeout;
                    objectId++;
                }
                if (timeout) return;

                timerSubscription.Dispose();
                try { observer.OnCompleted(); } finally { Dispose(); }
            }
        }

        class Timeout_ : OperatorObserverBase<T, T>
        {
            readonly TimeoutObservable<T> parent;
            readonly object gate = new object();
            bool isFinished = false;
            SingleAssignmentDisposable sourceSubscription;
            IDisposable timerSubscription;

            public Timeout_(TimeoutObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                sourceSubscription = new SingleAssignmentDisposable();

                timerSubscription = parent.scheduler.Schedule(parent.dueTimeDT.Value, OnNext);
                sourceSubscription.Disposable = parent.source.Subscribe(this);

                return StableCompositeDisposable.Create(timerSubscription, sourceSubscription);
            }

            // in timer
            void OnNext()
            {
                lock (gate)
                {
                    if (isFinished) return;
                    isFinished = true;
                }

                sourceSubscription.Dispose();
                try { observer.OnError(new TimeoutException()); } finally { Dispose(); }
            }

            public override void OnNext(T value)
            {
                lock (gate)
                {
                    if (!isFinished) observer.OnNext(value);
                }
            }

            public override void OnError(Exception error)
            {
                lock (gate)
                {
                    if (isFinished) return;
                    isFinished = true;
                    timerSubscription.Dispose();
                }
                try { observer.OnError(error); } finally { Dispose(); }
            }

            public override void OnCompleted()
            {

                lock (gate)
                {
                    if (!isFinished)
                    {
                        isFinished = true;
                        timerSubscription.Dispose();
                    }
                    try { observer.OnCompleted(); } finally { Dispose(); }
                }
            }
        }
    }
}