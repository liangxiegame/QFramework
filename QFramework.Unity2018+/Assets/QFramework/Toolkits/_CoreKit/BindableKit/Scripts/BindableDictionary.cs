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
using System.Runtime.Serialization;

namespace QFramework
{
    [Serializable]
    public class BindableDictionary<TKey, TValue> : IDictionary<TKey, TValue>,
        IDictionary,ISerializable, IDeserializationCallback
    {
        private readonly Dictionary<TKey, TValue> mInner;

        public BindableDictionary() => mInner = new Dictionary<TKey, TValue>();
        public BindableDictionary(IEqualityComparer<TKey> comparer) => mInner = new Dictionary<TKey, TValue>(comparer);
        public BindableDictionary(Dictionary<TKey, TValue> innerDictionary) => mInner = innerDictionary;

        public TValue this[TKey key]
        {
            get => mInner[key];
            set
            {
                TValue oldValue;
                if (TryGetValue(key, out oldValue))
                {
                    mInner[key] = value;
                    mOnReplace?.Trigger(key, oldValue, value);
                }
                else
                {
                    mInner[key] = value;
                    mOnAdd?.Trigger(key, value);
                    mOnCountChanged?.Trigger(Count);
                }
            }
        }



        public int Count => mInner.Count;
        public Dictionary<TKey, TValue>.KeyCollection Keys => mInner.Keys;
        public Dictionary<TKey, TValue>.ValueCollection Values => mInner.Values;

        public void Add(TKey key, TValue value)
        {
            mInner.Add(key, value);

            mOnAdd?.Trigger(key, value);
            mOnCountChanged?.Trigger(Count);
        }

        public void Clear()
        {
            var beforeCount = Count;
            mInner.Clear();

            mOnClear?.Trigger();
            if (beforeCount > 0)
            {
                mOnCountChanged?.Trigger(Count);
            }
        }

        public bool Remove(TKey key)
        {
            TValue oldValue;
            if (mInner.TryGetValue(key, out oldValue))
            {
                var isSuccessRemove = mInner.Remove(key);
                if (isSuccessRemove)
                {
                    mOnRemove?.Trigger(key, oldValue);
                    mOnCountChanged?.Trigger(Count);
                }
                return isSuccessRemove;
            }

            return false;
        }

        public bool ContainsKey(TKey key) => mInner.ContainsKey(key);

        public bool TryGetValue(TKey key, out TValue value) => mInner.TryGetValue(key, out value);

        public Dictionary<TKey, TValue>.Enumerator GetEnumerator() => mInner.GetEnumerator();
        
        
        [NonSerialized]
        private EasyEvent<int> mOnCountChanged;
        public EasyEvent<int> OnCountChanged => mOnCountChanged ?? (mOnCountChanged = new EasyEvent<int>());
        
            
        [NonSerialized]
        private EasyEvent mOnClear;
        public EasyEvent OnClear => mOnClear ?? (mOnClear = new EasyEvent());
        
        [NonSerialized]
        private EasyEvent<TKey, TValue> mOnAdd;
        public EasyEvent<TKey, TValue> OnAdd => mOnAdd ?? (mOnAdd = new EasyEvent<TKey, TValue>());
        
        [NonSerialized]
        private EasyEvent<TKey,TValue> mOnRemove;
        public EasyEvent<TKey, TValue> OnRemove => mOnRemove ?? (mOnRemove = new EasyEvent<TKey, TValue>());
        
        [NonSerialized]
        private EasyEvent<TKey, TValue,TValue> mOnReplace;

        /// <summary>
        /// TKey:key
        /// TValue:oldValue
        /// TValue:newValue
        /// </summary>
        public EasyEvent<TKey, TValue,TValue> OnReplace => mOnReplace ?? (mOnReplace = new EasyEvent<TKey, TValue, TValue>());



        #region implement explicit
        object IDictionary.this[object key]
        {
            get => this[(TKey)key];
            set => this[(TKey)key] = (TValue)value;
        }
        bool IDictionary.IsFixedSize => ((IDictionary)mInner).IsFixedSize;

        bool IDictionary.IsReadOnly => ((IDictionary)mInner).IsReadOnly;

        bool ICollection.IsSynchronized => ((IDictionary)mInner).IsSynchronized;

        ICollection IDictionary.Keys => ((IDictionary)mInner).Keys;

        object ICollection.SyncRoot => ((IDictionary)mInner).SyncRoot;

        ICollection IDictionary.Values => ((IDictionary)mInner).Values;
        
        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)mInner).IsReadOnly;

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => mInner.Keys;

        ICollection<TValue> IDictionary<TKey, TValue>.Values => mInner.Values;

        void IDictionary.Add(object key, object value) => Add((TKey)key, (TValue)value);

        bool IDictionary.Contains(object key) => ((IDictionary)mInner).Contains(key);

        void ICollection.CopyTo(Array array, int index) => ((IDictionary)mInner).CopyTo(array, index);

        public void GetObjectData(SerializationInfo info, StreamingContext context) => ((ISerializable)mInner).GetObjectData(info, context);

        public void OnDeserialization(object sender) => ((IDeserializationCallback)mInner).OnDeserialization(sender);

        void IDictionary.Remove(object key) => Remove((TKey)key);

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => ((ICollection<KeyValuePair<TKey, TValue>>)mInner).Contains(item);

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((ICollection<KeyValuePair<TKey, TValue>>)mInner).CopyTo(array, arrayIndex);

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => ((ICollection<KeyValuePair<TKey, TValue>>)mInner).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => mInner.GetEnumerator();

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            TValue v;
            if (TryGetValue(item.Key, out v))
            {
                if (EqualityComparer<TValue>.Default.Equals(v, item.Value))
                {
                    Remove(item.Key);
                    return true;
                }
            }

            return false;
        }

        IDictionaryEnumerator IDictionary.GetEnumerator() => ((IDictionary)mInner).GetEnumerator();

        #endregion
    }

    public static class BindableDictionaryExtensions
    {
        public static BindableDictionary<TKey, TValue> ToBindableDictionary<TKey, TValue>(
            this Dictionary<TKey, TValue> self) =>
            new BindableDictionary<TKey, TValue>(self);
    }
}