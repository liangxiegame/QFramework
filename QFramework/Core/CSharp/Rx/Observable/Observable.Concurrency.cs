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
    
    public static partial class Observable
    {
        public static IObservable<T> Synchronize<T>(this IObservable<T> source)
        {
            return new SynchronizeObservable<T>(source, new object());
        }

        public static IObservable<T> Synchronize<T>(this IObservable<T> source, object gate)
        {
            return new SynchronizeObservable<T>(source, gate);
        }

        public static IObservable<T> ObserveOn<T>(this IObservable<T> source, IScheduler scheduler)
        {
            return new ObserveOnObservable<T>(source, scheduler);
        }

        public static IObservable<T> SubscribeOn<T>(this IObservable<T> source, IScheduler scheduler)
        {
            return new SubscribeOnObservable<T>(source, scheduler);
        }

        public static IObservable<T> DelaySubscription<T>(this IObservable<T> source, TimeSpan dueTime)
        {
            return new DelaySubscriptionObservable<T>(source, dueTime, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<T> DelaySubscription<T>(this IObservable<T> source, TimeSpan dueTime, IScheduler scheduler)
        {
            return new DelaySubscriptionObservable<T>(source, dueTime, scheduler);
        }

        public static IObservable<T> DelaySubscription<T>(this IObservable<T> source, DateTimeOffset dueTime)
        {
            return new DelaySubscriptionObservable<T>(source, dueTime, Scheduler.DefaultSchedulers.TimeBasedOperations);
        }

        public static IObservable<T> DelaySubscription<T>(this IObservable<T> source, DateTimeOffset dueTime, IScheduler scheduler)
        {
            return new DelaySubscriptionObservable<T>(source, dueTime, scheduler);
        }

        public static IObservable<T> Amb<T>(params IObservable<T>[] sources)
        {
            return Amb((IEnumerable<IObservable<T>>)sources);
        }

        public static IObservable<T> Amb<T>(IEnumerable<IObservable<T>> sources)
        {
            var result = Observable.Never<T>();
            foreach (var item in sources)
            {
                var second = item;
                result = result.Amb(second);
            }
            return result;
        }

        public static IObservable<T> Amb<T>(this IObservable<T> source, IObservable<T> second)
        {
            return new AmbObservable<T>(source, second);
        }
    }
}