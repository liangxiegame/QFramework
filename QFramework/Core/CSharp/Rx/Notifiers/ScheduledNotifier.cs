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
    
    /// <summary>
    /// Notify value on setuped scheduler.
    /// </summary>
    public class ScheduledNotifier<T> : IObservable<T>, IProgress<T>
    {
        readonly IScheduler scheduler;
        readonly Subject<T> trigger = new Subject<T>();

        /// <summary>
        /// Use scheduler is Scheduler.DefaultSchedulers.ConstantTimeOperations.
        /// </summary>
        public ScheduledNotifier()
        {
            this.scheduler = Scheduler.DefaultSchedulers.ConstantTimeOperations;
        }
        /// <summary>
        /// Use scheduler is argument's scheduler.
        /// </summary>
        public ScheduledNotifier(IScheduler scheduler)
        {
            if (scheduler == null)
            {
                throw new ArgumentNullException("scheduler");
            }

            this.scheduler = scheduler;
        }

        /// <summary>
        /// Push value to subscribers on setuped scheduler.
        /// </summary>
        public void Report(T value)
        {
            scheduler.Schedule(() => trigger.OnNext(value));
        }

        /// <summary>
        /// Push value to subscribers on setuped scheduler.
        /// </summary>
        public IDisposable Report(T value, TimeSpan dueTime)
        {
            var cancel = scheduler.Schedule(dueTime, () => trigger.OnNext(value));
            return cancel;
        }

        /// <summary>
        /// Push value to subscribers on setuped scheduler.
        /// </summary>
        public IDisposable Report(T value, DateTimeOffset dueTime)
        {
            var cancel = scheduler.Schedule(dueTime, () => trigger.OnNext(value));
            return cancel;
        }

        /// <summary>
        /// Subscribe observer.
        /// </summary>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException("observer");
            }

            return trigger.Subscribe(observer);
        }
    }
}