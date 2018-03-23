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
    public interface IPTList<T>
    {
        void Accept(IListVisitor<T> visitor);
        void Accept(ListVisitorDelegate<T> visitor);
    }

    //列表访问器
    public interface IListVisitor<T>
    {
        void Visit(T data);
    }

    public delegate void ListVisitorDelegate<T>(T data);

    public interface Iterator<T>
    {
        bool HasNext
        {
            get;
        }

        T Next
        {
            get;
        }
    }

    public interface Iteratable<T>
    {
        Iterator<T> Iterator(); 
    }

    public class ListNode<T>
    {
		private T mData;
        ListNode<T> mNext;
        
        public T Data
        {
            get { return mData; }
            set { mData = value; }
        }

        public ListNode<T> Next
        {
			get { return mNext; }
            set { mNext = value; }
        }
    }
}