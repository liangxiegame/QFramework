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
	using System;

    public class LinkedListTest : BaseTestUnit
    {
        public override void StartTest()
        {
            TestInt();
            TestString();
        }

#region String Test
        private void TestString()
        {
            WriteBegin("LinkListTest(String)");
            PTLinkedList<string> list = new PTLinkedList<string>();
            BuildStringLinkedListRandom(list, 0, 10);
            BuildStringLinkedListRandom(list, 11, 20);
            RemoveListAtIndex(list, 19);
            RemoveListAtIndex(list, 0);
            RemoveData(list, "Index:7");
            VisitList(list);
            FindData(list, "Index:9");
            WriteEnd("LinkListTest(String)");
        }

        private void BuildStringLinkedListRandom(PTLinkedList<string> list, int start, int end)
        {
            for (int i = start; i <= end; ++i)
            {
                list.InsertTail(string.Format("Index:{0}", i));
            }
            WriteLine("Build:[{0}:{1}]", start, end);
        }
#endregion

#region Int Test
        private void TestInt()
        {
            WriteBegin("LinkListTest(Int)");
            PTLinkedList<int> list = new PTLinkedList<int>();
            BuildIntLinkedListRandom(list, 0, 10);
            BuildIntLinkedListRandom(list, 11, 20);
            RemoveListAtIndex(list, 19);
            RemoveListAtIndex(list, 0);
            RemoveData(list, 7);
            VisitList(list);
            FindData(list, 9);
            WriteEnd("LinkListTest(Int)");
        }

        private void BuildIntLinkedListRandom(PTLinkedList<int> list, int start, int end)
        {
            for (int i = start; i <= end; ++i)
            {
                list.InsertTail(i);
            }
            WriteLine("Build:[{0}:{1}]", start, end);
        }
#endregion

        private void RemoveListAtIndex<T>(PTLinkedList<T> list, int index)
        {
            WriteLine("Remove At:{0}-Result:{1}", index, list.RemoveAt(index));
        }

        private void RemoveData<T>(PTLinkedList<T> list, T data)
        {
            WriteLine("Remove Data:{0}-Result:{1}", data, list.Remove(data));
        }

        private void VisitList<T>(PTLinkedList<T> list)
        {
            WriteLine("Data Begin:");
            list.Accept(VisitData);
            WriteLine("");
        }

        protected void FindData<T>(PTLinkedList<T> list, T data)
        {
            WriteLine("FindData{0}: Result:{1}", data, list.Query(data));
        }

        protected void VisitData<T>(T data)
        {
            if (data != null)
            {
                Write(string.Format("   {0}", data));
            }
            else
            {
                Write(" NULL ");
            }
        }
    }
}


