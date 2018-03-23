/****************************************************************************
 * Copyright (c) 2017 liangxie
 *
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
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
    using System.Threading;

    public class PriorityQueue<T> where T : IComparable<T>
    {
        private static long mCount = long.MinValue;

        private IndexedItem[] mItems;
        private int mSize;

        public PriorityQueue()
            : this(16)
        {
        }

        public PriorityQueue(int capacity)
        {
            mItems = new IndexedItem[capacity];
            mSize = 0;
        }

        private bool IsHigherPriority(int left, int right)
        {
            return mItems[left].CompareTo(mItems[right]) < 0;
        }

        private void Percolate(int index)
        {
            if (index >= mSize || index < 0)
                return;
            var parent = (index - 1) / 2;
            if (parent < 0 || parent == index)
                return;

            if (IsHigherPriority(index, parent))
            {
                var temp = mItems[index];
                mItems[index] = mItems[parent];
                mItems[parent] = temp;
                Percolate(parent);
            }
        }

        private void Heapify()
        {
            Heapify(0);
        }

        private void Heapify(int index)
        {
            if (index >= mSize || index < 0)
                return;

            var left = 2 * index + 1;
            var right = 2 * index + 2;
            var first = index;

            if (left < mSize && IsHigherPriority(left, first))
                first = left;
            if (right < mSize && IsHigherPriority(right, first))
                first = right;
            if (first != index)
            {
                var temp = mItems[index];
                mItems[index] = mItems[first];
                mItems[first] = temp;
                Heapify(first);
            }
        }

        public int Count { get { return mSize; } }

        public T Peek()
        {
            if (mSize == 0)
                throw new InvalidOperationException("HEAP is Empty");

            return mItems[0].Value;
        }

        private void RemoveAt(int index)
        {
            mItems[index] = mItems[--mSize];
            mItems[mSize] = default(IndexedItem);
            Heapify();
            if (mSize < mItems.Length / 4)
            {
                var temp = mItems;
                mItems = new IndexedItem[mItems.Length / 2];
                Array.Copy(temp, 0, mItems, 0, mSize);
            }
        }

        public T Dequeue()
        {
            var result = Peek();
            RemoveAt(0);
            return result;
        }

        public void Enqueue(T item)
        {
            if (mSize >= mItems.Length)
            {
                var temp = mItems;
                mItems = new IndexedItem[mItems.Length * 2];
                Array.Copy(temp, mItems, temp.Length);
            }

            var index = mSize++;
            mItems[index] = new IndexedItem { Value = item, Id = Interlocked.Increment(ref mCount) };
            Percolate(index);
        }

        public bool Remove(T item)
        {
            for (var i = 0; i < mSize; ++i)
            {
                if (EqualityComparer<T>.Default.Equals(mItems[i].Value, item))
                {
                    RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        struct IndexedItem : IComparable<IndexedItem>
        {
            public T Value;
            public long Id;

            public int CompareTo(IndexedItem other)
            {
                var c = Value.CompareTo(other.Value);
                if (c == 0)
                    c = Id.CompareTo(other.Id);
                return c;
            }
        }
    }
}