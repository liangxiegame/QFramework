/****************************************************************************
 * Copyright (c) 2020.10 ~ 2022 liangxiegame UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
	 
	 
	 /// <summary>
	 /// 链表对象池：存储相关对象
	 /// </summary>
	 /// <typeparam name="T"></typeparam>
	 internal static class ListPool<T>
	 {
		 /// <summary>
		 /// 栈对象：存储多个List
		 /// </summary>
		 static Stack<List<T>> mListStack = new Stack<List<T>>(8);

		 /// <summary>
		 /// 出栈：获取某个List对象
		 /// </summary>
		 /// <returns></returns>
		 public static List<T> Get()
		 {
			 if (mListStack.Count == 0)
			 {
				 return new List<T>(8);
			 }

			 return mListStack.Pop();
		 }

		 /// <summary>
		 /// 入栈：将List对象添加到栈中
		 /// </summary>
		 /// <param name="toRelease"></param>
		 public static void Release(List<T> toRelease)
		 {
			 toRelease.Clear();
			 mListStack.Push(toRelease);
		 }
	 }

	 /// <summary>
	 /// 链表对象池 拓展方法类
	 /// </summary>
	 internal static class ListPoolExtensions
	 {
		 /// <summary>
		 /// 给List拓展 自身入栈 的方法
		 /// </summary>
		 /// <typeparam name="T"></typeparam>
		 /// <param name="toRelease"></param>
		 public static void Release2Pool<T>(this List<T> toRelease)
		 {
			 ListPool<T>.Release(toRelease);
		 }
	 }
	 
	 /// <summary>
	 /// 字典对象池：用于存储相关对象
	 /// </summary>
	 /// <typeparam name="TKey"></typeparam>
	 /// <typeparam name="TValue"></typeparam>
	 internal class DictionaryPool<TKey, TValue>
	 {
		 /// <summary>
		 /// 栈对象：存储多个字典
		 /// </summary>
		 static Stack<Dictionary<TKey, TValue>> mListStack = new Stack<Dictionary<TKey, TValue>>(8);

		 /// <summary>
		 /// 出栈：从栈中获取某个字典数据
		 /// </summary>
		 /// <returns></returns>
		 public static Dictionary<TKey, TValue> Get()
		 {
			 if (mListStack.Count == 0)
			 {
				 return new Dictionary<TKey, TValue>(8);
			 }

			 return mListStack.Pop();
		 }

		 /// <summary>
		 /// 入栈：将字典数据存储到栈中 
		 /// </summary>
		 /// <param name="toRelease"></param>
		 public static void Release(Dictionary<TKey, TValue> toRelease)
		 {
			 toRelease.Clear();
			 mListStack.Push(toRelease);
		 }
	 }
    
	 /// <summary>
	 /// 对象池字典 拓展方法类
	 /// </summary>
	 internal static class DictionaryPoolExtensions
	 {
		 /// <summary>
		 /// 对字典拓展 自身入栈 的方法
		 /// </summary>
		 /// <typeparam name="TKey"></typeparam>
		 /// <typeparam name="TValue"></typeparam>
		 /// <param name="toRelease"></param>
		 public static void Release2Pool<TKey,TValue>(this Dictionary<TKey, TValue> toRelease)
		 {
			 DictionaryPool<TKey,TValue>.Release(toRelease);
		 }
	 }
}