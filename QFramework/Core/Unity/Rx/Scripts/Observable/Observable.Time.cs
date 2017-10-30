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

    // Timer, Interval, etc...
    public static partial class Observable
    {
        public static IObservable<long> Interval(TimeSpan period)
        {
            return new TimerObservable(period, period, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<long> Interval(TimeSpan period, IScheduler scheduler)
        {
            return new TimerObservable(period, period, scheduler);
        }

        public static IObservable<long> Timer(TimeSpan dueTime)
        {
            return new TimerObservable(dueTime, null, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<long> Timer(DateTimeOffset dueTime)
        {
            return new TimerObservable(dueTime, null, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<long> Timer(TimeSpan dueTime, TimeSpan period)
        {
            return new TimerObservable(dueTime, period, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<long> Timer(DateTimeOffset dueTime, TimeSpan period)
        {
            return new TimerObservable(dueTime, period, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<long> Timer(TimeSpan dueTime, IScheduler scheduler)
        {
            return new TimerObservable(dueTime, null, scheduler);
        }

        public static IObservable<long> Timer(DateTimeOffset dueTime, IScheduler scheduler)
        {
            return new TimerObservable(dueTime, null, scheduler);
        }

        public static IObservable<long> Timer(TimeSpan dueTime, TimeSpan period, IScheduler scheduler)
        {
            return new TimerObservable(dueTime, period, scheduler);
        }

        public static IObservable<long> Timer(DateTimeOffset dueTime, TimeSpan period, IScheduler scheduler)
        {
            return new TimerObservable(dueTime, period, scheduler);
        }

        public static IObservable<Timestamped<TSource>> Timestamp<TSource>(this IObservable<TSource> source)
        {
            return Timestamp<TSource>(source, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<Timestamped<TSource>> Timestamp<TSource>(this IObservable<TSource> source, IScheduler scheduler)
        {
            return new TimestampObservable<TSource>(source, scheduler);
        }

        public static IObservable<TimeInterval<TSource>> TimeInterval<TSource>(this IObservable<TSource> source)
        {
            return TimeInterval(source, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<TimeInterval<TSource>> TimeInterval<TSource>(this IObservable<TSource> source, IScheduler scheduler)
        {
            return new TimeIntervalObservable<TSource>(source, scheduler);
        }

        public static IObservable<T> Delay<T>(this IObservable<T> source, TimeSpan dueTime)
        {
            return source.Delay(dueTime, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<TSource> Delay<TSource>(this IObservable<TSource> source, TimeSpan dueTime, IScheduler scheduler)
        {
            return new DelayObservable<TSource>(source, dueTime, scheduler);
        }

        public static IObservable<T> Sample<T>(this IObservable<T> source, TimeSpan interval)
        {
            return source.Sample(interval, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<T> Sample<T>(this IObservable<T> source, TimeSpan interval, IScheduler scheduler)
        {
            return new SampleObservable<T>(source, interval, scheduler);
        }

        public static IObservable<TSource> Throttle<TSource>(this IObservable<TSource> source, TimeSpan dueTime)
        {
            return source.Throttle(dueTime, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<TSource> Throttle<TSource>(this IObservable<TSource> source, TimeSpan dueTime, IScheduler scheduler)
        {
            return new ThrottleObservable<TSource>(source, dueTime, scheduler);
        }

        public static IObservable<TSource> ThrottleFirst<TSource>(this IObservable<TSource> source, TimeSpan dueTime)
        {
            return source.ThrottleFirst(dueTime, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<TSource> ThrottleFirst<TSource>(this IObservable<TSource> source, TimeSpan dueTime, IScheduler scheduler)
        {
            return new ThrottleFirstObservable<TSource>(source, dueTime, scheduler);
        }

        public static IObservable<T> Timeout<T>(this IObservable<T> source, TimeSpan dueTime)
        {
            return source.Timeout(dueTime, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<T> Timeout<T>(this IObservable<T> source, TimeSpan dueTime, IScheduler scheduler)
        {
            return new TimeoutObservable<T>(source, dueTime, scheduler);
        }

        public static IObservable<T> Timeout<T>(this IObservable<T> source, DateTimeOffset dueTime)
        {
            return source.Timeout(dueTime, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<T> Timeout<T>(this IObservable<T> source, DateTimeOffset dueTime, IScheduler scheduler)
        {
            return new TimeoutObservable<T>(source, dueTime, scheduler);
        }
    }
}