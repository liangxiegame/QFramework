/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 liangxie
 * 
 * https://qframework.cn
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

namespace QFramework
{
    using System;

    public enum BinaryHeapBuildMode
    {
        kNLog = 1,
        kN = 2,
    }

    public enum BinaryHeapSortMode
    {
        kMin = 0,
        kMax = 1,
    }

    //优先队列&二叉堆
    public class BinaryHeap<T> where T : IBinaryHeapElement
    {
        protected T[] mArray;

        protected float mGrowthFactor = 1.6f;
        protected int mLastChildIndex; //最后子节点的位置
        protected BinaryHeapSortMode mSortMode;

        public BinaryHeap(int minSize, BinaryHeapSortMode sortMode)
        {
            mSortMode = sortMode;
            mArray = new T[minSize];
            mLastChildIndex = 0;
        }

        public BinaryHeap(T[] dataArray, BinaryHeapSortMode sortMode)
        {
            mSortMode = sortMode;
            int minSize = 10;
            if (dataArray != null)
            {
                minSize = dataArray.Length + 1;
            }

            mArray = new T[minSize];
            mLastChildIndex = 0;
            Insert(dataArray, BinaryHeapBuildMode.kN);
        }

        #region 公开方法

        #region 清空

        public void Clear()
        {
            mArray = new T[10];
            mLastChildIndex = 0;
        }

        #endregion

        #region 插入

        public void Insert(T[] dataArray, BinaryHeapBuildMode buildMode)
        {
            if (dataArray == null)
            {
                throw new NullReferenceException("BinaryHeap Not Support Insert Null Object");
            }

            int totalLength = mLastChildIndex + dataArray.Length + 1;
            if (mArray.Length < totalLength)
            {
                ResizeArray(totalLength);
            }

            if (buildMode == BinaryHeapBuildMode.kNLog)
            {
                //方式1:直接添加，每次添加都会上浮
                for (int i = 0; i < dataArray.Length; ++i)
                {
                    Insert(dataArray[i]);
                }
            }
            else
            {
                //数量比较大的情况下会快一些
                //方式2:先添加完，然后排序
                for (int i = 0; i < dataArray.Length; ++i)
                {
                    mArray[++mLastChildIndex] = dataArray[i];
                }

                SortAsCurrentMode();
            }
        }

        public void Insert(T element)
        {
            if (element == null)
            {
                throw new NullReferenceException("BinaryHeap Not Support Insert Null Object");
            }

            int index = ++mLastChildIndex;

            if (index == mArray.Length)
            {
                ResizeArray();
            }

            mArray[index] = element;

            ProcolateUp(index);
        }

        #endregion

        #region 弹出

        public T Pop()
        {
            if (mLastChildIndex < 1)
            {
                return default(T);
            }

            T result = mArray[1];
            mArray[1] = mArray[mLastChildIndex--];
            ProcolateDown(1);
            return result;
        }

        public T Top()
        {
            if (mLastChildIndex < 1)
            {
                return default(T);
            }

            return mArray[1];
        }

        #endregion

        #region 重新排序

        public void Sort(BinaryHeapSortMode sortMode)
        {
            if (mSortMode == sortMode)
            {
                return;
            }
            mSortMode = sortMode;
            SortAsCurrentMode();
        }

        public void RebuildAtIndex(int index)
        {
            if (index > mLastChildIndex)
            {
                return;
            }

            //1.首先找父节点，是否比父节点小，如果满足则上浮,不满足下沉
            var element = mArray[index];

            int parentIndex = index >> 1;
            if (parentIndex > 0)
            {
                if (mSortMode == BinaryHeapSortMode.kMin)
                {
                    if (element.SortScore < mArray[parentIndex].SortScore)
                    {
                        ProcolateUp(index);
                    }
                    else
                    {
                        ProcolateDown(index);
                    }
                }
                else
                {
                    if (element.SortScore > mArray[parentIndex].SortScore)
                    {
                        ProcolateUp(index);
                    }
                    else
                    {
                        ProcolateDown(index);
                    }
                }
            }
            else
            {
                ProcolateDown(index);
            }
        }

        private void SortAsCurrentMode()
        {
            int startChild = mLastChildIndex >> 1;
            for (int i = startChild; i > 0; --i)
            {
                ProcolateDown(i);
            }
        }

        #endregion

        #region 指定位置删除

        public void RemoveAt(int index)
        {
            if (index > mLastChildIndex || index < 1)
            {
                return;
            }

            if (index == mLastChildIndex)
            {
                --mLastChildIndex;
                mArray[index] = default(T);
                return;
            }

            mArray[index] = mArray[mLastChildIndex--];
            mArray[index].HeapIndex = index;
            RebuildAtIndex(index);
        }

        #endregion

        #region 索引查找

        //这个索引和大小排序之间没有任何关系
        public T GetElement(int index)
        {
            if (index > mLastChildIndex)
            {
                return default(T);
            }
            return mArray[index];
        }

        #endregion

        #region 判定辅助

        public bool HasValue()
        {
            return mLastChildIndex > 0;
        }

        #endregion

        #region 内部方法

        protected void ResizeArray(int newSize = -1)
        {
            if (newSize < 0)
            {
                newSize = System.Math.Max(mArray.Length + 4, (int) System.Math.Round(mArray.Length * mGrowthFactor));
            }

            if (newSize > 1 << 30)
            {
                throw new System.Exception(
                    "Binary Heap Size really large (2^18). A heap size this large is probably the cause of pathfinding running in an infinite loop. " +
                    "\nRemove this check (in BinaryHeap.cs) if you are sure that it is not caused by a bug");
            }

            T[] tmp = new T[newSize];
            for (int i = 0; i < mArray.Length; i++)
            {
                tmp[i] = mArray[i];
            }

            mArray = tmp;
        }

        //上浮:空穴思想
        protected void ProcolateUp(int index)
        {
            var element = mArray[index];
            if (element == null)
            {
                return;
            }

            float sortScore = element.SortScore;

            int parentIndex = index >> 1;

            if (mSortMode == BinaryHeapSortMode.kMin)
            {
                while (parentIndex >= 1 && sortScore < mArray[parentIndex].SortScore)
                {
                    mArray[index] = mArray[parentIndex];
                    mArray[index].HeapIndex = index;
                    index = parentIndex;
                    parentIndex = index >> 1;
                }
            }
            else
            {
                while (parentIndex >= 1 && sortScore > mArray[parentIndex].SortScore)
                {
                    mArray[index] = mArray[parentIndex];
                    mArray[index].HeapIndex = index;
                    index = parentIndex;
                    parentIndex = index >> 1;
                }
            }
            mArray[index] = element;
            mArray[index].HeapIndex = index;
        }

        protected void ProcolateDown(int index)
        {
            var element = mArray[index];
            if (element == null)
            {
                return;
            }

            int childIndex = index << 1;

            if (mSortMode == BinaryHeapSortMode.kMin)
            {
                while (childIndex <= mLastChildIndex)
                {
                    if (childIndex != mLastChildIndex)
                    {
                        if (mArray[childIndex + 1].SortScore < mArray[childIndex].SortScore)
                        {
                            childIndex = childIndex + 1;
                        }
                    }

                    if (mArray[childIndex].SortScore < element.SortScore)
                    {
                        mArray[index] = mArray[childIndex];
                        mArray[index].HeapIndex = index;
                    }
                    else
                    {
                        break;
                    }

                    index = childIndex;
                    childIndex = index << 1;
                }
            }
            else
            {
                while (childIndex <= mLastChildIndex)
                {
                    if (childIndex != mLastChildIndex)
                    {
                        if (mArray[childIndex + 1].SortScore > mArray[childIndex].SortScore)
                        {
                            childIndex = childIndex + 1;
                        }
                    }

                    if (mArray[childIndex].SortScore > element.SortScore)
                    {
                        mArray[index] = mArray[childIndex];
                        mArray[index].HeapIndex = index;
                    }
                    else
                    {
                        break;
                    }

                    index = childIndex;
                    childIndex = index << 1;
                }
            }

            mArray[index] = element;
            mArray[index].HeapIndex = index;
        }
        #endregion
        #endregion
    }
}