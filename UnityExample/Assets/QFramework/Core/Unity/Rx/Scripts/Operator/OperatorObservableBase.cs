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

    // implements note : all field must be readonly.
    public abstract class OperatorObservableBase<T> : IOptimizedObservable<T>
    {
        readonly bool isRequiredSubscribeOnCurrentThread;

        public OperatorObservableBase(bool isRequiredSubscribeOnCurrentThread)
        {
            this.isRequiredSubscribeOnCurrentThread = isRequiredSubscribeOnCurrentThread;
        }

        public bool IsRequiredSubscribeOnCurrentThread()
        {
            return isRequiredSubscribeOnCurrentThread;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            var subscription = new SingleAssignmentDisposable();

            // note:
            // does not make the safe observer, it breaks exception durability.
            // var safeObserver = Observer.CreateAutoDetachObserver<T>(observer, subscription);

            if (isRequiredSubscribeOnCurrentThread && Scheduler.IsCurrentThreadSchedulerScheduleRequired)
            {
                Scheduler.CurrentThread.Schedule(() => subscription.Disposable = SubscribeCore(observer, subscription));
            }
            else
            {
                subscription.Disposable = SubscribeCore(observer, subscription);
            }

            return subscription;
        }

        protected abstract IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel);
    }
}