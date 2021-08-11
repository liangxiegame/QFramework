/****************************************************************************
 * Copyright (c) 2017 Chris Foulston
 * 
 * https://github.com/cfoulston/Unity-Reorderable-List
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    [Serializable]
    public abstract class ReorderableArray<T> : ICloneable, IList<T>, ICollection<T>, IEnumerable<T> {

        [SerializeField]
        private List<T> array = new List<T>();

        public ReorderableArray()
            : this(0) {
        }

        public ReorderableArray(int length) {

            array = new List<T>(length);
        }

        public T this[int index] {

            get { return array[index]; }
            set { array[index] = value; }
        }
		
        public int Length {
			
            get { return array.Count; }
        }

        public bool IsReadOnly {

            get { return false; }
        }

        public int Count {

            get { return array.Count; }
        }

        public object Clone() {

            return new List<T>(array);
        }

        public void CopyFrom(IEnumerable<T> value) {

            array.Clear();
            array.AddRange(value);
        }

        public bool Contains(T value) {

            return array.Contains(value);
        }

        public int IndexOf(T value) {

            return array.IndexOf(value);
        }

        public void Insert(int index, T item) {

            array.Insert(index, item);
        }

        public void RemoveAt(int index) {

            array.RemoveAt(index);
        }

        public void Add(T item) {

            array.Add(item);
        }

        public void Clear() {

            array.Clear();
        }

        public void CopyTo(T[] array, int arrayIndex) {

            this.array.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item) {

            return array.Remove(item);
        }

        public T[] ToArray() {

            return array.ToArray();
        }

        public IEnumerator<T> GetEnumerator() {

            return array.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {

            return array.GetEnumerator();
        }
    }
}