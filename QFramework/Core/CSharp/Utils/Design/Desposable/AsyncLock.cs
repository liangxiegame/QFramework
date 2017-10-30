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

// this code is borrowed from RxOfficial(rx.codeplex.com) and modified

namespace QFramework
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Asynchronous lock. TODO: refactor variable name
    /// </summary>
    public sealed class AsyncLock : IDisposable
    {
        private readonly Queue<Action> queue = new Queue<Action>();
        private bool isAcquired = false;
        private bool hasFaulted = false;

        /// <summary>
        /// Queues the action for execution. If the caller acquires the lock and becomes the owner,
        /// the queue is processed. If the lock is already owned, the action is queued and will get
        /// processed by the owner.
        /// </summary>
        /// <param name="action">Action to queue for execution.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        public void Wait(Action action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            var isOwner = false;
            lock (queue)
            {
                if (!hasFaulted)
                {
                    queue.Enqueue(action);
                    isOwner = !isAcquired;
                    isAcquired = true;
                }
            }

            if (isOwner)
            {
                while (true)
                {
                    var work = default(Action);
                    lock (queue)
                    {
                        if (queue.Count > 0)
                            work = queue.Dequeue();
                        else
                        {
                            isAcquired = false;
                            break;
                        }
                    }

                    try
                    {
                        work();
                    }
                    catch
                    {
                        lock (queue)
                        {
                            queue.Clear();
                            hasFaulted = true;
                        }
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Clears the work items in the queue and drops further work being queued.
        /// </summary>
        public void Dispose()
        {
            lock (queue)
            {
                queue.Clear();
                hasFaulted = true;
            }
        }
    }
}
