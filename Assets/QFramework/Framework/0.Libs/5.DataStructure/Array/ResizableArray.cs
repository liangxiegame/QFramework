/****************************************************************************
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
    using System.Linq;

    public class ResizeableArray<T> where T : class
    {
        public T this[int index]
        {
            get { return mArray[index]; }
        }

        public int Count
        {
            get { return mArray.Length; }
        }

        private T[] mArray;

        public ResizeableArray(int initCount)
        {
            mArray = new T[initCount];
        }

        public void Append(T itemToAdd)
        {
            Array.Resize(ref mArray, mArray.Length + 1);
            mArray[mArray.Length - 1] = itemToAdd;
        }

        public void Clear()
        {
            Array.Clear(mArray, 0, mArray.Length);
        }

        public void Remove(
            T itemToRemove)
        {
            var size = mArray.Length;

            var newIndex = 0;
            for (var i = 0; i < size; i++)
            {
                if (mArray[i] == itemToRemove)
                {
                    mArray[i] = null;
                    continue;
                }

                mArray[newIndex] = mArray[i];
                newIndex++;
            }

            Array.Resize(ref mArray, mArray.Length - 1);
        }

        public void RemoveAt(int index)
        {
            var size = mArray.Length;

            mArray[index] = null;

            for (var i = index; i < size - 1; i++)
            {
                mArray[i] = mArray[i + 1];
            }

            Array.Resize(ref mArray, mArray.Length - 1);
        }


        public bool Contains(T item)
        {
            return mArray.Contains(item);
        }
    }
}