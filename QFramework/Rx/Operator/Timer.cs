using System;

namespace QFramework.Core.Rx
{
    using QFramework.Core.Utils;
    using QFramework.Core.Utils.Scheduler;

    internal class TimerObservable : OperatorObservableBase<long>
    {
        readonly DateTimeOffset? dueTimeA;
        readonly TimeSpan? dueTimeB;
        readonly TimeSpan? period;
        readonly Utils.Scheduler.IScheduler scheduler;

        public TimerObservable(DateTimeOffset dueTime, TimeSpan? period, Utils.Scheduler.IScheduler scheduler)
            : base(scheduler == Utils.Scheduler.Scheduler.CurrentThread)
        {
            this.dueTimeA = dueTime;
            this.period = period;
            this.scheduler = scheduler;
        }

        public TimerObservable(TimeSpan dueTime, TimeSpan? period, Utils.Scheduler.IScheduler scheduler)
            : base(scheduler == Utils.Scheduler.Scheduler.CurrentThread)
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
                return scheduler.Schedule(Utils.Scheduler.Scheduler.Normalize(dueTime), () =>
                {
                    timerObserver.OnNext();
                    timerObserver.OnCompleted();
                });
            }
            else
            {
                var periodicScheduler = scheduler as Utils.Scheduler.ISchedulerPeriodic;
                if (periodicScheduler != null)
                {
                    if (dueTime == period.Value)
                    {
                        // same(Observable.Interval), run periodic
                        return periodicScheduler.SchedulePeriodic(Utils.Scheduler.Scheduler.Normalize(dueTime), timerObserver.OnNext);
                    }
                    else
                    {
                        // Schedule Once + Scheudle Periodic
                        var disposable = new SerialDisposable();

                        disposable.Disposable = scheduler.Schedule(Utils.Scheduler.Scheduler.Normalize(dueTime), () =>
                        {
                            timerObserver.OnNext(); // run first

                            var timeP = Utils.Scheduler.Scheduler.Normalize(period.Value);
                            disposable.Disposable = periodicScheduler.SchedulePeriodic(timeP, timerObserver.OnNext); // run periodic
                        });

                        return disposable;
                    }
                }
                else
                {
                    var timeP = Utils.Scheduler.Scheduler.Normalize(period.Value);

                    return scheduler.Schedule(Utils.Scheduler.Scheduler.Normalize(dueTime), self =>
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