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
    using System.Collections.Generic;

    public class DelayObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly TimeSpan dueTime;
        readonly IScheduler scheduler;

        public DelayObservable(IObservable<T> source, TimeSpan dueTime, IScheduler scheduler) 
            : base(scheduler == Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.dueTime = dueTime;
            this.scheduler = scheduler;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new Delay(this, observer, cancel).Run();
        }

        class Delay : OperatorObserverBase<T, T>
        {
            readonly DelayObservable<T> parent;
            readonly object gate = new object();
            bool hasFailed;
            bool running;
            bool active;
            Exception exception;
            Queue<Timestamped<T>> queue;
            bool onCompleted;
            DateTimeOffset completeAt;
            IDisposable sourceSubscription;
            TimeSpan delay;
            bool ready;
            SerialDisposable cancelable;

            public Delay(DelayObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                cancelable = new SerialDisposable();

                active = false;
                running = false;
                queue = new Queue<Timestamped<T>>();
                onCompleted = false;
                completeAt = default(DateTimeOffset);
                hasFailed = false;
                exception = default(Exception);
                ready = true;
                delay = Scheduler.Normalize(parent.dueTime);

                var _sourceSubscription = new SingleAssignmentDisposable();
                sourceSubscription = _sourceSubscription; // assign to field
                _sourceSubscription.Disposable = parent.source.Subscribe(this);

                return StableCompositeDisposable.Create(sourceSubscription, cancelable);
            }

            public override void OnNext(T value)
            {
                var next = parent.scheduler.Now.Add(delay);
                var shouldRun = false;

                lock (gate)
                {
                    queue.Enqueue(new Timestamped<T>(value, next));

                    shouldRun = ready && !active;
                    active = true;
                }

                if (shouldRun)
                {
                    cancelable.Disposable = parent.scheduler.Schedule(delay, DrainQueue);
                }
            }

            public override void OnError(Exception error)
            {
                sourceSubscription.Dispose();

                var shouldRun = false;

                lock (gate)
                {
                    queue.Clear();

                    exception = error;
                    hasFailed = true;

                    shouldRun = !running;
                }

                if (shouldRun)
                {
                    try { base.observer.OnError(error); } finally { Dispose(); }
                }
            }

            public override void OnCompleted()
            {
                sourceSubscription.Dispose();

                var next = parent.scheduler.Now.Add(delay);
                var shouldRun = false;

                lock (gate)
                {
                    completeAt = next;
                    onCompleted = true;

                    shouldRun = ready && !active;
                    active = true;
                }

                if (shouldRun)
                {
                    cancelable.Disposable = parent.scheduler.Schedule(delay, DrainQueue);
                }
            }

            void DrainQueue(Action<TimeSpan> recurse)
            {
                lock (gate)
                {
                    if (hasFailed) return;
                    running = true;
                }

                var shouldYield = false;

                while (true)
                {
                    var hasFailed = false;
                    var error = default(Exception);

                    var hasValue = false;
                    var value = default(T);
                    var hasCompleted = false;

                    var shouldRecurse = false;
                    var recurseDueTime = default(TimeSpan);

                    lock (gate)
                    {
                        if (hasFailed)
                        {
                            error = exception;
                            hasFailed = true;
                            running = false;
                        }
                        else
                        {
                            if (queue.Count > 0)
                            {
                                var nextDue = queue.Peek().Timestamp;

                                if (nextDue.CompareTo(parent.scheduler.Now) <= 0 && !shouldYield)
                                {
                                    value = queue.Dequeue().Value;
                                    hasValue = true;
                                }
                                else
                                {
                                    shouldRecurse = true;
                                    recurseDueTime = Scheduler.Normalize(nextDue.Subtract(parent.scheduler.Now));
                                    running = false;
                                }
                            }
                            else if (onCompleted)
                            {
                                if (completeAt.CompareTo(parent.scheduler.Now) <= 0 && !shouldYield)
                                {
                                    hasCompleted = true;
                                }
                                else
                                {
                                    shouldRecurse = true;
                                    recurseDueTime = Scheduler.Normalize(completeAt.Subtract(parent.scheduler.Now));
                                    running = false;
                                }
                            }
                            else
                            {
                                running = false;
                                active = false;
                            }
                        }
                    }

                    if (hasValue)
                    {
                        base.observer.OnNext(value);
                        shouldYield = true;
                    }
                    else
                    {
                        if (hasCompleted)
                        {
                            try { base.observer.OnCompleted(); } finally { Dispose(); }
                        }
                        else if (hasFailed)
                        {
                            try { base.observer.OnError(error); } finally { Dispose(); }
                        }
                        else if (shouldRecurse)
                        {
                            recurse(recurseDueTime);
                        }

                        return;
                    }
                }
            }
        }
    }
}