/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace BindKit.Binding.Reflection
{
    public class ProxyItemInfo : IProxyItemInfo
    {
        private readonly bool isValueType;
        protected PropertyInfo propertyInfo;
        protected MethodInfo getMethod;
        protected MethodInfo setMethod;

        public ProxyItemInfo(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if (!propertyInfo.Name.Equals("Item"))
                throw new ArgumentException("The property types do not match!");

            if (!typeof(ICollection).IsAssignableFrom(propertyInfo.DeclaringType))
                throw new ArgumentException("The property types do not match!");

            this.propertyInfo = propertyInfo;
#if NETFX_CORE
            this.isValueType = this.propertyInfo.DeclaringType.GetTypeInfo().IsValueType;
#else
            this.isValueType = this.propertyInfo.DeclaringType.IsValueType;
#endif

            if (this.propertyInfo.CanRead)
                this.getMethod = propertyInfo.GetGetMethod();

            if (this.propertyInfo.CanWrite)
                this.setMethod = propertyInfo.GetSetMethod();
        }

        public bool IsValueType { get { return isValueType; } }

        public Type ValueType { get { return this.propertyInfo.PropertyType; } }

        public Type DeclaringType { get { return this.propertyInfo.DeclaringType; } }

        public string Name { get { return this.propertyInfo.Name; } }

        public bool IsStatic { get { return this.propertyInfo.IsStatic(); } }

        public object GetValue(object target, object key)
        {
            if (target is IList)
            {
                int index = (int)key;
                IList list = target as IList;

                if (index < 0 || index >= list.Count)
                    throw new ArgumentOutOfRangeException("key", string.Format("The index is out of range, the key value is {0}, it is not between 0 and {1}", index, list.Count));

                return list[index];
            }

            if (target is IDictionary)
            {
                IDictionary dict = target as IDictionary;
                if (!dict.Contains(key))
                    return null;

                return dict[key];
            }

            if (this.getMethod == null)
                throw new MemberAccessException();

            return this.getMethod.Invoke(target, new object[] { key });
        }

        public void SetValue(object target, object key, object value)
        {
            if (target is IList)
            {
                int index = (int)key;
                IList list = target as IList;

                if (index < 0 || index >= list.Count)
                    throw new ArgumentOutOfRangeException("key", string.Format("The index is out of range, the key value is {0}, it is not between 0 and {1}", index, list.Count));

                list[index] = value;
                return;
            }

            if (target is IDictionary)
            {
                ((IDictionary)target)[key] = value;
                return;
            }

            if (this.setMethod == null)
                throw new MemberAccessException();

            this.setMethod.Invoke(target, new object[] { key, value });
        }
    }


    public class ListProxyItemInfo<T, TValue> : ProxyItemInfo, IProxyItemInfo<T, int, TValue> where T : IList<TValue>
    {
        public ListProxyItemInfo(PropertyInfo propertyInfo) : base(propertyInfo)
        {
            if (!typeof(TValue).Equals(this.propertyInfo.PropertyType) || !typeof(IList<TValue>).IsAssignableFrom(propertyInfo.DeclaringType))
                throw new ArgumentException("The property types do not match!");
        }

        public TValue GetValue(T target, int key)
        {
            if (key < 0 || key >= target.Count)
                throw new ArgumentOutOfRangeException("key", string.Format("The index is out of range, the key value is {0}, it is not between 0 and {1}", key, target.Count));

            return target[key];
        }

        public TValue GetValue(object target, int key)
        {
            return this.GetValue((T)target, key);
        }

        public void SetValue(T target, int key, TValue value)
        {
            if (key < 0 || key >= target.Count)
                throw new ArgumentOutOfRangeException("key", string.Format("The index is out of range, the key value is {0}, it is not between 0 and {1}", key, target.Count));

            target[key] = value;
        }

        public void SetValue(object target, int key, TValue value)
        {
            this.SetValue((T)target, key, value);
        }
    }

    public class DictionaryProxyItemInfo<T, TKey, TValue> : ProxyItemInfo, IProxyItemInfo<T, TKey, TValue> where T : IDictionary<TKey, TValue>
    {
        public DictionaryProxyItemInfo(PropertyInfo propertyInfo) : base(propertyInfo)
        {
            if (!typeof(TValue).Equals(this.propertyInfo.PropertyType) || !typeof(IDictionary<TKey, TValue>).IsAssignableFrom(propertyInfo.DeclaringType))
                throw new ArgumentException("The property types do not match!");
        }

        public TValue GetValue(T target, TKey key)
        {
            if (!target.ContainsKey(key))
                return default(TValue);

            return target[key];
        }

        public TValue GetValue(object target, TKey key)
        {
            return this.GetValue((T)target, key);
        }

        public void SetValue(T target, TKey key, TValue value)
        {
            target[key] = value;
        }

        public void SetValue(object target, TKey key, TValue value)
        {
            this.SetValue((T)target, key, value);
        }
    }

    public class ArrayProxyItemInfo : IProxyItemInfo
    {
        protected readonly Type type;
        public ArrayProxyItemInfo(Type type)
        {
            this.type = type;
            if (this.type == null || !this.type.IsArray)
                throw new ArgumentException();
        }

        public Type ValueType { get { return type.HasElementType ? type.GetElementType() : typeof(object); } }

        public Type DeclaringType { get { return this.type; } }

        public string Name { get { return "Item"; } }

        public bool IsStatic { get { return false; } }

        public virtual object GetValue(object target, object key)
        {
            int index = (int)key;
            Array array = target as Array;
            if (index < 0 || index >= array.Length)
                throw new ArgumentOutOfRangeException("key", string.Format("The index is out of range, the key value is {0}, it is not between 0 and {1}", index, array.Length));

            return array.GetValue(index);
        }

        public virtual void SetValue(object target, object key, object value)
        {
            int index = (int)key;
            Array array = target as Array;
            if (index < 0 || index >= array.Length)
                throw new ArgumentOutOfRangeException("key", string.Format("The index is out of range, the key value is {0}, it is not between 0 and {1}", index, array.Length));

            array.SetValue(value, index);
        }
    }

    public class ArrayProxyItemInfo<T, TValue> : ArrayProxyItemInfo, IProxyItemInfo<T, int, TValue> where T : IList<TValue>
    {
        public ArrayProxyItemInfo() : base(typeof(T))
        {
        }

        public TValue GetValue(T target, int key)
        {
            if (key < 0 || key >= target.Count)
                throw new ArgumentOutOfRangeException("key", string.Format("The index is out of range, the key value is {0}, it is not between 0 and {1}", key, target.Count));

            return target[key];
        }

        public TValue GetValue(object target, int key)
        {
            return GetValue((T)target, key);
        }

        public void SetValue(T target, int key, TValue value)
        {
            if (key < 0 || key >= target.Count)
                throw new ArgumentOutOfRangeException("key", string.Format("The index is out of range, the key value is {0}, it is not between 0 and {1}", key, target.Count));

            target[key] = value;
        }

        public void SetValue(object target, int key, TValue value)
        {
            SetValue((T)target, key, value);
        }
    }
}
