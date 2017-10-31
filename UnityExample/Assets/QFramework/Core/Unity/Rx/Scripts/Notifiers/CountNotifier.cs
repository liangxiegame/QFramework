/****************************************************************************
 * Copyright (c) 2017 liangxie
****************************************************************************/

namespace QFramework
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    
    
    /// <summary>Event kind of CountNotifier.</summary>
    public enum CountChangedStatus
    {
        /// <summary>Count incremented.</summary>
        Increment,
        /// <summary>Count decremented.</summary>
        Decrement,
        /// <summary>Count is zero.</summary>
        Empty,
        /// <summary>Count arrived max.</summary>
        Max
    }

    /// <summary>
    /// Notify event of count flag.
    /// </summary>
    public class CountNotifier : IObservable<CountChangedStatus>
    {
        readonly object mLockObject = new object();
        readonly Subject<CountChangedStatus> mStatusChanged = new Subject<CountChangedStatus>();
        readonly int mMax;

        public int Max { get { return mMax; } }
        public int Count { get; private set; }

        /// <summary>
        /// Setup max count of signal.
        /// </summary>
        public CountNotifier(int max = int.MaxValue)
        {
            if (max <= 0)
            {
                throw new ArgumentException("max");
            }

            this.mMax = max;
        }

        /// <summary>
        /// Increment count and notify status.
        /// </summary>
        public IDisposable Increment(int incrementCount = 1)
        {
            if (incrementCount < 0)
            {
                throw new ArgumentException("incrementCount");
            }

            lock (mLockObject)
            {
                if (Count == Max) return Disposable.Empty;
                else if (incrementCount + Count > Max) Count = Max;
                else Count += incrementCount;

                mStatusChanged.OnNext(CountChangedStatus.Increment);
                if (Count == Max) mStatusChanged.OnNext(CountChangedStatus.Max);

                return Disposable.Create(() => this.Decrement(incrementCount));
            }
        }

        /// <summary>
        /// Decrement count and notify status.
        /// </summary>
        public void Decrement(int decrementCount = 1)
        {
            if (decrementCount < 0)
            {
                throw new ArgumentException("decrementCount");
            }

            lock (mLockObject)
            {
                if (Count == 0) return;
                else if (Count - decrementCount < 0) Count = 0;
                else Count -= decrementCount;

                mStatusChanged.OnNext(CountChangedStatus.Decrement);
                if (Count == 0) mStatusChanged.OnNext(CountChangedStatus.Empty);
            }
        }

        /// <summary>
        /// Subscribe observer.
        /// </summary>
        public IDisposable Subscribe(IObserver<CountChangedStatus> observer)
        {
            return mStatusChanged.Subscribe(observer);
        }
    }
}