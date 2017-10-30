using System;
using System.Collections.Generic;

namespace QFramework.Core.Rx
{
    using QFramework.Core.Utils.Scheduler;

    internal class DelaySubscriptionObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly Utils.Scheduler.IScheduler scheduler;
        readonly TimeSpan? dueTimeT;
        readonly DateTimeOffset? dueTimeD;

        public DelaySubscriptionObservable(IObservable<T> source,TimeSpan dueTime, Utils.Scheduler.IScheduler scheduler)
            : base(scheduler == Utils.Scheduler.Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.scheduler = scheduler;
            this.dueTimeT = dueTime;
        }

        public DelaySubscriptionObservable(IObservable<T> source, DateTimeOffset dueTime, Utils.Scheduler.IScheduler scheduler)
            : base(scheduler == Utils.Scheduler.Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.scheduler = scheduler;
            this.dueTimeD = dueTime;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            if (dueTimeT != null)
            {
                var d = new MultipleAssignmentDisposable();
                var dt = Utils.Scheduler.Scheduler.Normalize(dueTimeT.Value);

                d.Disposable = scheduler.Schedule(dt, () =>
                {
                    d.Disposable = source.Subscribe(observer);
                });

                return d;
            }
            else
            {
                var d = new MultipleAssignmentDisposable();

                d.Disposable = scheduler.Schedule(dueTimeD.Value, () =>
                {
                    d.Disposable = source.Subscribe(observer);
                });

                return d;
            }
        }
    }
}