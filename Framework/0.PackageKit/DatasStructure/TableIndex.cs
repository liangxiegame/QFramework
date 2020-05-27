using System;
using System.Collections.Generic;
using System.Linq;

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