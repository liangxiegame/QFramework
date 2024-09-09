/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 liangxie UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
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

        #region 公开方法

        #region 清空

        public void Clear()
        {
            mArray = new T[10];
            mLastChildIndex = 0;
        }

        #endregion

        #region 插入

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