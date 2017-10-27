using System;

namespace QFramework.Core.Rx
{
    using QFramework.Core.Utils;
    using QFramework.Core.Utils.Scheduler;

    internal class SubscribeOnObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly Utils.Scheduler.IScheduler scheduler;

        public SubscribeOnObservable(IObservable<T> source, Utils.Scheduler.IScheduler scheduler)
            : base(scheduler == Utils.Scheduler.Scheduler.CurrentThread || source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.scheduler = scheduler;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            var m = new SingleAssignmentDisposable();
            var d = new SerialDisposable();
            d.Disposable = m;

            m.Disposable = scheduler.Schedule(() =>
            {
                d.Disposable = new ScheduledDisposable(scheduler, source.Subscribe(observer));
            });

            return d;
        }
    }
}