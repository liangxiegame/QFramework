using System;
using System.Collections;
using System.Collections.Generic;

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