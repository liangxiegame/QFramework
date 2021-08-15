/****************************************************************************
 * Copyright (c) 2020.10 ~ 12 liangxie
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
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

// ReSharper disable once CheckNamespace
namespace QFramework
{
	public abstract class Table<TDataItem> : IEnumerable<TDataItem>, IDisposable
	{
		public void Add(TDataItem item)
		{
			OnAdd(item);
		}

		public void Remove(TDataItem item)
		{
			OnRemove(item);
		}

		public void Clear()
		{
			OnClear();
		}

		// 改，由于 TDataItem 是引用类型，所以直接改值即可。
		public void Update()
		{
		}

		protected abstract void OnAdd(TDataItem item);
		protected abstract void OnRemove(TDataItem item);

		protected abstract void OnClear();


		public abstract IEnumerator<TDataItem> GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Dispose()
		{
			OnDispose();
		}

		protected abstract void OnDispose();
	}
}