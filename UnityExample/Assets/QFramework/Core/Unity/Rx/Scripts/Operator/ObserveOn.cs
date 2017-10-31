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

    public class ObserveOnObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly IScheduler scheduler;

        public ObserveOnObservable(IObservable<T> source, IScheduler scheduler)
            : base(source.IsRequiredSubscribeOnCurrentThread())
        {
            this.source = source;
            this.scheduler = scheduler;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            var queueing = scheduler as ISchedulerQueueing;
            if (queueing == null)
            {
                return new ObserveOn(this, observer, cancel).Run();
            }
            else
            {
                return new ObserveOn_(this, queueing, observer, cancel).Run();
            }
        }

        class ObserveOn : OperatorObserverBase<T, T>
        {
            readonly ObserveOnObservable<T> parent;
            readonly LinkedList<IDisposable> scheduleDisposables = new LinkedList<IDisposable>();
            bool isDisposed;

            public ObserveOn(ObserveOnObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                isDisposed = false;

                var sourceDisposable = parent.source.Subscribe(this);

                return StableCompositeDisposable.Create(sourceDisposable, Disposable.Create(() =>
                {
                    lock (scheduleDisposables)
                    {
                        isDisposed = true;

                        foreach (var item in scheduleDisposables)
                        {
                            item.Dispose();
                        }
                        scheduleDisposables.Clear();
                    }
                }));
            }

            public override void OnNext(T value)
            {
                var self = new SingleAssignmentDisposable();
                LinkedListNode<IDisposable> node;
                lock (scheduleDisposables)
                {
                    if (isDisposed) return;

                    node = scheduleDisposables.AddLast(self);
                }
                self.Disposable = parent.scheduler.Schedule(() =>
                {
                    self.Dispose();
                    lock (scheduleDisposables)
                    {
                        if (node.List != null)
                        {
                            node.List.Remove(node);
                        }
                    }
                    base.observer.OnNext(value);
                });
            }

            public override void OnError(Exception error)
            {
                var self = new SingleAssignmentDisposable();
                LinkedListNode<IDisposable> node;
                lock (scheduleDisposables)
                {
                    if (isDisposed) return;

                    node = scheduleDisposables.AddLast(self);
                }
                self.Disposable = parent.scheduler.Schedule(() =>
                {
                    self.Dispose();
                    lock (scheduleDisposables)
                    {
                        if (node.List != null)
                        {
                            node.List.Remove(node);
                        }
                    }
                    try { observer.OnError(error); } finally { Dispose(); };
                });
            }

            public override void OnCompleted()
            {
                var self = new SingleAssignmentDisposable();
                LinkedListNode<IDisposable> node;
                lock (scheduleDisposables)
                {
                    if (isDisposed) return;

                    node = scheduleDisposables.AddLast(self);
                }
                self.Disposable = parent.scheduler.Schedule(() =>
                {
                    self.Dispose();
                    lock (scheduleDisposables)
                    {
                        if (node.List != null)
                        {
                            node.List.Remove(node);
                        }
                    }
                    try { observer.OnCompleted(); } finally { Dispose(); };
                });
            }
        }

        class ObserveOn_ : OperatorObserverBase<T, T>
        {
            readonly ObserveOnObservable<T> parent;
            readonly ISchedulerQueueing scheduler;
            readonly BooleanDisposable isDisposed;
            readonly Action<T> onNext;

            public ObserveOn_(ObserveOnObservable<T> parent, ISchedulerQueueing scheduler, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
                this.scheduler = scheduler;
                this.isDisposed = new BooleanDisposable();
                this.onNext = new Action<T>(OnNext_); // cache delegate
            }

            public IDisposable Run()
            {
                var sourceDisposable = parent.source.Subscribe(this);
                return StableCompositeDisposable.Create(sourceDisposable, isDisposed);
            }

            void OnNext_(T value)
            {
                base.observer.OnNext(value);
            }

            void OnError_(Exception error)
            {
                try { observer.OnError(error); } finally { Dispose(); };
            }

            void OnCompleted_(Unit _)
            {
                try { observer.OnCompleted(); } finally { Dispose(); };
            }

            public override void OnNext(T value)
            {
                scheduler.ScheduleQueueing(isDisposed, value, onNext);
            }

            public override void OnError(Exception error)
            {
                scheduler.ScheduleQueueing(isDisposed, error, OnError_);
            }

            public override void OnCompleted()
            {
                scheduler.ScheduleQueueing(isDisposed, Unit.Default, OnCompleted_);
            }
        }
    }
}