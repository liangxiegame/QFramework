using System;

namespace QFramework.Core.Rx
{
    // Timer, Interval, etc...
    public static partial class Observable
    {
        public static IObservable<long> Interval(TimeSpan period)
        {
            return new TimerObservable(period, period, Utils.Scheduler.Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<long> Interval(TimeSpan period, Utils.Scheduler.IScheduler scheduler)
        {
            return new TimerObservable(period, period, scheduler);
        }

        public static IObservable<long> Timer(TimeSpan dueTime)
        {
            return new TimerObservable(dueTime, null, Utils.Scheduler.Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<long> Timer(DateTimeOffset dueTime)
        {
            return new TimerObservable(dueTime, null, Utils.Scheduler.Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<long> Timer(TimeSpan dueTime, TimeSpan period)
        {
            return new TimerObservable(dueTime, period, Utils.Scheduler.Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<long> Timer(DateTimeOffset dueTime, TimeSpan period)
        {
            return new TimerObservable(dueTime, period, Utils.Scheduler.Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<long> Timer(TimeSpan dueTime, Utils.Scheduler.IScheduler scheduler)
        {
            return new TimerObservable(dueTime, null, scheduler);
        }

        public static IObservable<long> Timer(DateTimeOffset dueTime, Utils.Scheduler.IScheduler scheduler)
        {
            return new TimerObservable(dueTime, null, scheduler);
        }

        public static IObservable<long> Timer(TimeSpan dueTime, TimeSpan period, Utils.Scheduler.IScheduler scheduler)
        {
            return new TimerObservable(dueTime, period, scheduler);
        }

        public static IObservable<long> Timer(DateTimeOffset dueTime, TimeSpan period, Utils.Scheduler.IScheduler scheduler)
        {
            return new TimerObservable(dueTime, period, scheduler);
        }

        public static IObservable<Timestamped<TSource>> Timestamp<TSource>(this IObservable<TSource> source)
        {
            return Timestamp<TSource>(source, Utils.Scheduler.Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<Timestamped<TSource>> Timestamp<TSource>(this IObservable<TSource> source, Utils.Scheduler.IScheduler scheduler)
        {
            return new TimestampObservable<TSource>(source, scheduler);
        }

        public static IObservable<TimeInterval<TSource>> TimeInterval<TSource>(this IObservable<TSource> source)
        {
            return TimeInterval(source, Utils.Scheduler.Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<TimeInterval<TSource>> TimeInterval<TSource>(this IObservable<TSource> source, Utils.Scheduler.IScheduler scheduler)
        {
            return new TimeIntervalObservable<TSource>(source, scheduler);
        }

        public static IObservable<T> Delay<T>(this IObservable<T> source, TimeSpan dueTime)
        {
            return source.Delay(dueTime, Utils.Scheduler.Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<TSource> Delay<TSource>(this IObservable<TSource> source, TimeSpan dueTime, Utils.Scheduler.IScheduler scheduler)
        {
            return new DelayObservable<TSource>(source, dueTime, scheduler);
        }

        public static IObservable<T> Sample<T>(this IObservable<T> source, TimeSpan interval)
        {
            return source.Sample(interval, Utils.Scheduler.Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<T> Sample<T>(this IObservable<T> source, TimeSpan interval, Utils.Scheduler.IScheduler scheduler)
        {
            return new SampleObservable<T>(source, interval, scheduler);
        }

        public static IObservable<TSource> Throttle<TSource>(this IObservable<TSource> source, TimeSpan dueTime)
        {
            return source.Throttle(dueTime, Utils.Scheduler.Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<TSource> Throttle<TSource>(this IObservable<TSource> source, TimeSpan dueTime, Utils.Scheduler.IScheduler scheduler)
        {
            return new ThrottleObservable<TSource>(source, dueTime, scheduler);
        }

        public static IObservable<TSource> ThrottleFirst<TSource>(this IObservable<TSource> source, TimeSpan dueTime)
        {
            return source.ThrottleFirst(dueTime, Utils.Scheduler.Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<TSource> ThrottleFirst<TSource>(this IObservable<TSource> source, TimeSpan dueTime, Utils.Scheduler.IScheduler scheduler)
        {
            return new ThrottleFirstObservable<TSource>(source, dueTime, scheduler);
        }

        public static IObservable<T> Timeout<T>(this IObservable<T> source, TimeSpan dueTime)
        {
            return source.Timeout(dueTime, Utils.Scheduler.Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<T> Timeout<T>(this IObservable<T> source, TimeSpan dueTime, Utils.Scheduler.IScheduler scheduler)
        {
            return new TimeoutObservable<T>(source, dueTime, scheduler);
        }

        public static IObservable<T> Timeout<T>(this IObservable<T> source, DateTimeOffset dueTime)
        {
            return source.Timeout(dueTime, Utils.Scheduler.Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<T> Timeout<T>(this IObservable<T> source, DateTimeOffset dueTime, Utils.Scheduler.IScheduler scheduler)
        {
            return new TimeoutObservable<T>(source, dueTime, scheduler);
        }
    }
}