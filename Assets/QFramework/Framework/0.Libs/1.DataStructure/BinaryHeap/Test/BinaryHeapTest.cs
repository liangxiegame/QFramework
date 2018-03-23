/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 liangxie
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

namespace QFramework
{
    public class BinaryHeapTest : BaseTestUnit
    {
        class IntWrap : IBinaryHeapElement
        {
            private int mSortValue;
            private int mHeapIndex;

            public IntWrap(int value)
            {
                mSortValue = value;
            }

            public float SortScore
            {
                get { return mSortValue; }
                set { mSortValue = (int) value; }
            }

            public int HeapIndex
            {
                get { return mHeapIndex; }
                set { mHeapIndex = value; }
            }

            public void RebuildHeap<T>(BinaryHeap<T> heap) where T : IBinaryHeapElement
            {
                heap.RebuildAtIndex(mHeapIndex);
            }
        }

        public override void StartTest()
        {
            //测试范围：1创建，2，数组创建，3.插入，4，弹出，重建，指定位置重建，指定位置删除
            BinaryHeap<IntWrap> heap1 = new BinaryHeap<IntWrap>(10, BinaryHeapSortMode.kMin);
            BinaryHeap<IntWrap> heap2 = new BinaryHeap<IntWrap>(10, BinaryHeapSortMode.kMin);
            IntWrap[]
                mTestData = BuildRandomIntWrapArray(
                    20); //BuildIntWrapArray(new int[]{ 10, 121, 11, 15, 18, 21, 7, 99, 182, 0, 87, 26, 21 });
            //TimeDebugger.S.Begin("Insert:NLog");
            heap1.Insert(mTestData, BinaryHeapBuildMode.kNLog);
            //TimeDebugger.S.End();

            //TimeDebugger.S.Begin("Insert:N");
            heap2.Insert(mTestData, BinaryHeapBuildMode.kN);
            //TimeDebugger.S.End();
            //TimeDebugger.S.Dump();

            WriteBegin("BinaryHeap");

            IntWrap insertItem = new IntWrap(39);
            heap1.Insert(insertItem);
            int oldIndex = insertItem.HeapIndex;
            IntWrap findResult = heap1.GetElement(oldIndex);
            if (insertItem.Equals(findResult))
            {
                WriteLine("## Success: Find Old Element In Index");
            }
            insertItem.SortScore = 7;
            insertItem.RebuildHeap(heap1);
            int newIndex = insertItem.HeapIndex;
            findResult = heap1.GetElement(newIndex);
            if (insertItem.Equals(findResult))
            {
                WriteLine("## Success: Find New Element In Index");
            }
            WriteLine("## InserTest: OldIndex:{0}, newIndex:{1}", oldIndex, insertItem.HeapIndex);

            IntWrap element = null;
            int processCount = 0;
            heap1.Sort(BinaryHeapSortMode.kMax);
            newIndex = insertItem.HeapIndex;
            WriteLine("## InsertTest: NewIndex After Sort:{0}", newIndex);
            heap1.RemoveAt(insertItem.HeapIndex);
            while (heap1.HasValue() && ++processCount < 100)
            {
                element = heap1.Pop();
                if (element != null)
                {
                    Write("     " + element.SortScore);
                }
                else
                {
                    Write("     NULL");
                }
            }
            WriteLine("");
            WriteEnd("BinaryHeap");
        }

        private IntWrap[] BuildIntWrapArray(int[] dataArray)
        {
            IntWrap[] result = new IntWrap[dataArray.Length];
            for (int i = 0; i < dataArray.Length; ++i)
            {
                result[i] = new IntWrap(dataArray[i]);
            }
            return result;
        }

        private IntWrap[] BuildRandomIntWrapArray(int totalCount)
        {
            IntWrap[] result = new IntWrap[totalCount];
            //Random r = new Random();
            for (int i = 0; i < totalCount; ++i)
            {
                result[i] = new IntWrap(i);
            }
            return result;
        }
    }
}