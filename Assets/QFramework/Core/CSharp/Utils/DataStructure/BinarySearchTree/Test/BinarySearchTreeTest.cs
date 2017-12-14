/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 liangxie
 * 
 * http://liangxiegame.com
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
    public class BinarySearchTreeTest : BaseTestUnit
    {
        public class IntWrap : IBinarySearchTreeElement
        {
            private int mScore;

            public IntWrap(int score)
            {
                mScore = score;
            }

            public float SortScore
            {
                get { return mScore; }
            }
        }

        public override void StartTest()
        {
            WriteBegin("BinarySearchTree");
            SCBinarySearchTree<IntWrap> tree = new SCBinarySearchTree<IntWrap>();
            IntWrap[] data = BuildIntWrapArray(new int[] { 0, 1, 2, 3, 4, 5 });//BuildIntWrapArray(new int[] { 10, 5, 8, 4, 18, 12, 20, 21 });
            tree.Insert(data);
            VisitTree(tree);
            TestRemove(tree, data[5]);
            TestRemove(tree, data[4]);
            TestRemove(tree, data[3]);
            TestRemove(tree, data[2]);
            IntWrap[] data2 = BuildIntWrapArray(new int[] { 6, 12, 234, 20, 77, 888, 76 });
            tree.Insert(data2);
            TestIterator(tree);
            WriteEnd("BinarySearchTree");
        }

        protected void TestRemove<T>(SCBinarySearchTree<T> tree, T data) where T : IBinarySearchTreeElement
        {
            WriteLine(" RemoveTest:");
            tree.Remove(data);
        }

        private void TestIterator<T>(SCBinarySearchTree<T> tree) where T : IBinarySearchTreeElement
        {
            WriteBegin("Iterator:");
            var it = tree.Iterator();
            while (it.HasNext)
            {
                T data = it.Next;
                Write("     " + data.SortScore);
            }
            WriteLine("");
            WriteEnd("Iterator:");
        }

        private void VisitTree<T>(SCBinarySearchTree<T> tree) where T : IBinarySearchTreeElement
        {
            WriteLine("Visitor:");
            tree.Accept(VisitorData);
            WriteLine("");
        }

        private void VisitorData<T>(T data) where T : IBinarySearchTreeElement
        {
            Write("     " + data.SortScore);
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
    }
}


