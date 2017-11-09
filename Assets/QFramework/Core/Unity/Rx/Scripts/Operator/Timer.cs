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

    public class TimerObservable : OperatorObservableBase<long>
    {
        readonly DateTimeOffset? dueTimeA;
        readonly TimeSpan? dueTimeB;
        readonly TimeSpan? period;
        readonly IScheduler scheduler;

        public TimerObservable(DateTimeOffset dueTime, TimeSpan? period, IScheduler scheduler)
            : base(scheduler == Scheduler.CurrentThread)
        {
            this.dueTimeA = dueTime;
            this.period = period;
            this.scheduler = scheduler;
        }

        public TimerObservable(TimeSpan dueTime, TimeSpan? period, IScheduler scheduler)
            : base(scheduler == Scheduler.CurrentThread)
        {
            this.dueTimeB = dueTime;
            this.period = period;
            this.scheduler = scheduler;
        }

        protected override IDisposable SubscribeCore(IObserver<long> observer, IDisposable cancel)
        {
            var timerObserver = new Timer(observer, cancel);

            var dueTime = (dueTimeA != null)
                ? dueTimeA.Value - scheduler.Now
                : dueTimeB.Value;

            // one-shot
            if (period == null)
            {
                return scheduler.Schedule(Scheduler.Normalize(dueTime), () =>
                {
                    timerObserver.OnNext();
                    timerObserver.OnCompleted();
                });
            }
            else
            {
                var periodicScheduler = scheduler as ISchedulerPeriodic;
                if (periodicScheduler != null)
                {
                    if (dueTime == period.Value)
                    {
                        // same(Observable.Interval), run periodic
                        return periodicScheduler.SchedulePeriodic(Scheduler.Normalize(dueTime), timerObserver.OnNext);
                    }
                    else
                    {
                        // Schedule Once + Scheudle Periodic
                        var disposable = new SerialDisposable();

                        disposable.Disposable = scheduler.Schedule(Scheduler.Normalize(dueTime), () =>
                        {
                            timerObserver.OnNext(); // run first

                            var timeP = Scheduler.Normalize(period.Value);
                            disposable.Disposable = periodicScheduler.SchedulePeriodic(timeP, timerObserver.OnNext); // run periodic
                        });

                        return disposable;
                    }
                }
                else
                {
                    var timeP = Scheduler.Normalize(period.Value);

                    return scheduler.Schedule(Scheduler.Normalize(dueTime), self =>
                    {
                        timerObserver.OnNext();
                        self(timeP);
                    });
                }
            }
        }

        class Timer : OperatorObserverBase<long, long>
        {
            long index = 0;

            public Timer(IObserver<long> observer, IDisposable cancel)
                : base(observer, cancel)
            {
            }

            public void OnNext()
            {
                try
                {
                    base.observer.OnNext(index++);
                }
                catch
                {
                    Dispose();
                    throw;
                }
            }

            public override void OnNext(long value)
            {
                // no use.
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