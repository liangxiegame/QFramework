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

// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.


namespace QFramework
{
    using System;

    /// <summary>
    /// Abstract base class for scheduled work items.
    /// </summary>
    public class ScheduledItem : IComparable<ScheduledItem>
    {
        private readonly BooleanDisposable mDisposable = new BooleanDisposable();
        private readonly TimeSpan mDueTime;
        private readonly Action mAction;

        /// <summary>
        /// Creates a new scheduled work item to run at the specified time.
        /// </summary>
        /// <param name="dueTime">Absolute time at which the work item has to be executed.</param>
        public ScheduledItem(Action action, TimeSpan dueTime)
        {
            mDueTime = dueTime;
            mAction = action;
        }

        /// <summary>
        /// Gets the absolute time at which the item is due for invocation.
        /// </summary>
        public TimeSpan DueTime
        {
            get { return mDueTime; }
        }

        /// <summary>
        /// Invokes the work item.
        /// </summary>
        public void Invoke()
        {
            if (!mDisposable.IsDisposed)
            {
                mAction();
            }
        }

        #region Inequality

        /// <summary>
        /// Compares the work item with another work item based on absolute time values.
        /// </summary>
        /// <param name="other">Work item to compare the current work item to.</param>
        /// <returns>Relative ordering between this and the specified work item.</returns>
        /// <remarks>The inequality operators are overloaded to provide results consistent with the IComparable implementation. Equality operators implement traditional reference equality semantics.</remarks>
        public int CompareTo(ScheduledItem other)
        {
            // MSDN: By definition, any object compares greater than null, and two null references compare equal to each other. 
            if (object.ReferenceEquals(other, null))
                return 1;

            return DueTime.CompareTo(other.DueTime);
        }

        /// <summary>
        /// Determines whether one specified ScheduledItem&lt;TAbsolute&gt; object is due before a second specified ScheduledItem&lt;TAbsolute&gt; object.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if the DueTime value of left is earlier than the DueTime value of right; otherwise, false.</returns>
        /// <remarks>This operator provides results consistent with the IComparable implementation.</remarks>
        public static bool operator <(ScheduledItem left, ScheduledItem right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>
        /// Determines whether one specified ScheduledItem&lt;TAbsolute&gt; object is due before or at the same of a second specified ScheduledItem&lt;TAbsolute&gt; object.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if the DueTime value of left is earlier than or simultaneous with the DueTime value of right; otherwise, false.</returns>
        /// <remarks>This operator provides results consistent with the IComparable implementation.</remarks>
        public static bool operator <=(ScheduledItem left, ScheduledItem right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <summary>
        /// Determines whether one specified ScheduledItem&lt;TAbsolute&gt; object is due after a second specified ScheduledItem&lt;TAbsolute&gt; object.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if the DueTime value of left is later than the DueTime value of right; otherwise, false.</returns>
        /// <remarks>This operator provides results consistent with the IComparable implementation.</remarks>
        public static bool operator >(ScheduledItem left, ScheduledItem right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>
        /// Determines whether one specified ScheduledItem&lt;TAbsolute&gt; object is due after or at the same time of a second specified ScheduledItem&lt;TAbsolute&gt; object.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if the DueTime value of left is later than or simultaneous with the DueTime value of right; otherwise, false.</returns>
        /// <remarks>This operator provides results consistent with the IComparable implementation.</remarks>
        public static bool operator >=(ScheduledItem left, ScheduledItem right)
        {
            return left.CompareTo(right) >= 0;
        }

        #endregion

        #region Equality

        /// <summary>
        /// Determines whether two specified ScheduledItem&lt;TAbsolute, TValue&gt; objects are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if both ScheduledItem&lt;TAbsolute, TValue&gt; are equal; otherwise, false.</returns>
        /// <remarks>This operator does not provide results consistent with the IComparable implementation. Instead, it implements reference equality.</remarks>
        public static bool operator ==(ScheduledItem left, ScheduledItem right)
        {
            return object.ReferenceEquals(left, right);
        }

        /// <summary>
        /// Determines whether two specified ScheduledItem&lt;TAbsolute, TValue&gt; objects are inequal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>true if both ScheduledItem&lt;TAbsolute, TValue&gt; are inequal; otherwise, false.</returns>
        /// <remarks>This operator does not provide results consistent with the IComparable implementation. Instead, it implements reference equality.</remarks>
        public static bool operator !=(ScheduledItem left, ScheduledItem right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Determines whether a ScheduledItem&lt;TAbsolute&gt; object is equal to the specified object.
        /// </summary>
        /// <param name="obj">The object to compare to the current ScheduledItem&lt;TAbsolute&gt; object.</param>
        /// <returns>true if the obj parameter is a ScheduledItem&lt;TAbsolute&gt; object and is equal to the current ScheduledItem&lt;TAbsolute&gt; object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return object.ReferenceEquals(this, obj);
        }

        /// <summary>
        /// Returns the hash code for the current ScheduledItem&lt;TAbsolute&gt; object.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion

        public IDisposable Cancellation
        {
            get { return mDisposable; }
        }

        /// <summary>
        /// Gets whether the work item has received a cancellation request.
        /// </summary>
        public bool IsCanceled
        {
            get { return mDisposable.IsDisposed; }
        }
    }

    /// <summary>
    /// Efficient scheduler queue that maintains scheduled items sorted by absolute time.
    /// </summary>
    /// <remarks>This type is not thread safe; users should ensure proper synchronization.</remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming",
        "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "But it *is* a queue!")]
    public class SchedulerQueue
    {
        private readonly PriorityQueue<ScheduledItem> mQueue;

        /// <summary>
        /// Creates a new scheduler queue with a default initial capacity.
        /// </summary>
        public SchedulerQueue()
            : this(1024)
        {
        }

        /// <summary>
        /// Creats a new scheduler queue with the specified initial capacity.
        /// </summary>
        /// <param name="capacity">Initial capacity of the scheduler queue.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public SchedulerQueue(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException("capacity");

            mQueue = new PriorityQueue<ScheduledItem>(capacity);
        }

        /// <summary>
        /// Gets the number of scheduled items in the scheduler queue.
        /// </summary>
        public int Count
        {
            get { return mQueue.Count; }
        }

        /// <summary>
        /// Enqueues the specified work item to be scheduled.
        /// </summary>
        /// <param name="scheduledItem">Work item to be scheduled.</param>
        public void Enqueue(ScheduledItem scheduledItem)
        {
            mQueue.Enqueue(scheduledItem);
        }

        /// <summary>
        /// Removes the specified work item from the scheduler queue.
        /// </summary>
        /// <param name="scheduledItem">Work item to be removed from the scheduler queue.</param>
        /// <returns>true if the item was found; false otherwise.</returns>
        public bool Remove(ScheduledItem scheduledItem)
        {
            return mQueue.Remove(scheduledItem);
        }

        /// <summary>
        /// Dequeues the next work item from the scheduler queue.
        /// </summary>
        /// <returns>Next work item in the scheduler queue (removed).</returns>
        public ScheduledItem Dequeue()
        {
            return mQueue.Dequeue();
        }

        /// <summary>
        /// Peeks the next work item in the scheduler queue.
        /// </summary>
        /// <returns>Next work item in the scheduler queue (not removed).</returns>
        public ScheduledItem Peek()
        {
            return mQueue.Peek();
        }
    }
}