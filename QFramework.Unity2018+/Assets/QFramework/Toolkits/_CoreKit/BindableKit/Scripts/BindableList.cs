/****************************************************************************
 * Copyright (c) 2016 - 2024 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace QFramework
{
    [Serializable]
    public class BindableList<T> : Collection<T>
    {
        public BindableList()
        {
        }

        public BindableList(IEnumerable<T> collection)
        {
            if (collection == null) throw new ArgumentNullException("collection");

            foreach (var item in collection)
            {
                Add(item);
            }
        }

        public BindableList(List<T> list)
            : base(list != null ? new List<T>(list) : null)
        {
        }

        protected override void ClearItems()
        {
            var beforeCount = Count;
            base.ClearItems();

            mOnClear?.Trigger();
            if (beforeCount > 0)
            {
                mOnCountChanged?.Trigger(Count);
            }
        }

        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);

            mCollectionAdd?.Trigger(index, item);
            mOnCountChanged?.Trigger(Count);
        }

        public void Move(int oldIndex, int newIndex) => MoveItem(oldIndex, newIndex);

        protected virtual void MoveItem(int oldIndex, int newIndex)
        {
            var item = this[oldIndex];
            base.RemoveItem(oldIndex);
            base.InsertItem(newIndex, item);

            mOnMove?.Trigger(oldIndex, newIndex, item);
        }

        protected override void RemoveItem(int index)
        {
            var item = this[index];
            base.RemoveItem(index);

            mOnRemove?.Trigger(index, item);
            mOnCountChanged?.Trigger(Count);
        }

        protected override void SetItem(int index, T item)
        {
            var oldItem = this[index];
            base.SetItem(index, item);

            mOnReplace?.Trigger(index, oldItem, item);
        }


        [NonSerialized] private EasyEvent<int> mOnCountChanged = null;
        public EasyEvent<int> OnCountChanged => mOnCountChanged ?? (mOnCountChanged = new EasyEvent<int>());

        [NonSerialized] private EasyEvent mOnClear = null;
        public EasyEvent OnClear => mOnClear ?? (mOnClear = new EasyEvent());

        [NonSerialized] private EasyEvent<int,T> mCollectionAdd = null;
        public EasyEvent<int,T> OnAdd => mCollectionAdd ?? (mCollectionAdd = new EasyEvent<int,T>());

        [NonSerialized] private EasyEvent<int,int,T> mOnMove = null;
        
        /// <summary>
        /// int:oldIndex
        /// int:newIndex
        /// T:item
        /// </summary>
        public EasyEvent<int,int,T> OnMove => mOnMove ?? (mOnMove = new EasyEvent<int,int,T>());

        [NonSerialized] private EasyEvent<int,T> mOnRemove = null;
        public EasyEvent<int,T> OnRemove => mOnRemove ?? (mOnRemove = new EasyEvent<int,T>());

        [NonSerialized] private EasyEvent<int,T,T> mOnReplace = null;
        
        /// <summary>
        /// int:index
        /// T:oldItem
        /// T:newItem
        /// </summary>
        public EasyEvent<int,T,T>  OnReplace => mOnReplace ?? (mOnReplace = new EasyEvent<int,T,T>());
    }

    public static class BindableListExtensions
    {
        public static BindableList<T> ToBindableList<T>(this IEnumerable<T> self) => new BindableList<T>(self);
    }
}