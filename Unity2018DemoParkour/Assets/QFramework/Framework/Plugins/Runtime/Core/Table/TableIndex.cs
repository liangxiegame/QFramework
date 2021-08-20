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
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace QFramework
{
    public class TableIndex<TKeyType, TDataItem>: IDisposable
    {
        private Dictionary<TKeyType, List<TDataItem>> mIndex = new Dictionary<TKeyType, List<TDataItem>>();

        private Func<TDataItem, TKeyType> mGetKeyByDataItem = null;

        public TableIndex(Func<TDataItem, TKeyType> keyGetter)
        {
            mGetKeyByDataItem = keyGetter;
        }

        public IDictionary<TKeyType, List<TDataItem>> Dictionary
        {
            get { return mIndex; }
        }

        public void Add(TDataItem dataItem)
        {
            var key = mGetKeyByDataItem(dataItem);

            if (mIndex.ContainsKey(key))
            {
                mIndex[key].Add(dataItem);
            }
            else
            {
                var list = ListPool<TDataItem>.Get();

                list.Add(dataItem);

                mIndex.Add(key, list);
            }
        }

        public void Remove(TDataItem dataItem)
        {
            var key = mGetKeyByDataItem(dataItem);

            mIndex[key].Remove(dataItem);
        }

        public IEnumerable<TDataItem> Get(TKeyType key)
        {
            List<TDataItem> retList = null;

            if (mIndex.TryGetValue(key, out retList))
            {
                return retList;
            }

            // 返回一个空的集合
            return Enumerable.Empty<TDataItem>();
        }

        public void Clear()
        {
            foreach (var value in mIndex.Values)
            {
                value.Clear();
            }

            mIndex.Clear();
        }
        

        public void Dispose()
        {
            foreach (var value in mIndex.Values)
            {
                value.Release2Pool();
            }

            mIndex.Release2Pool();

            mIndex = null;
        }
    }
}