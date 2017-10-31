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
    
    public class Wait<T> : IObserver<T>
    {
        static readonly TimeSpan InfiniteTimeSpan = new TimeSpan(0, 0, 0, 0, -1); // from .NET 4.5

        readonly IObservable<T> source;
        readonly TimeSpan timeout;

        System.Threading.ManualResetEvent semaphore;

        bool seenValue = false;
        T value = default(T);
        Exception ex = default(Exception);

        public Wait(IObservable<T> source, TimeSpan timeout)
        {
            this.source = source;
            this.timeout = timeout;
        }

        public T Run()
        {
            semaphore = new System.Threading.ManualResetEvent(false);
            using (source.Subscribe(this))
            {
                var waitComplete = (timeout == InfiniteTimeSpan)
                    ? semaphore.WaitOne()
                    : semaphore.WaitOne(timeout);

                if (!waitComplete)
                {
                    throw new TimeoutException("OnCompleted not fired.");
                }
            }

            if (ex != null) throw ex;
            if (!seenValue) throw new InvalidOperationException("No Elements.");

            return value;
        }

        public void OnNext(T value)
        {
            seenValue = true;
            this.value = value;
        }

        public void OnError(Exception error)
        {
            this.ex = error;
            semaphore.Set();
        }

        public void OnCompleted()
        {
            semaphore.Set();
        }
    }
}