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

using Loxodon.Log;
using System;
using System.Reflection;

namespace BindKit.Binding.Reflection
{
    public class ProxyPropertyInfo : IProxyPropertyInfo
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(ProxyPropertyInfo));

        private readonly bool isValueType;
        protected PropertyInfo propertyInfo;
        protected MethodInfo getMethod;
        protected MethodInfo setMethod;

        public ProxyPropertyInfo(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            this.propertyInfo = propertyInfo;
#if NETFX_CORE
            this.isValueType = this.propertyInfo.DeclaringType.GetTypeInfo().IsValueType;
#else
            this.isValueType = this.propertyInfo.DeclaringType.IsValueType;
#endif

            if (this.propertyInfo.CanRead)
                this.getMethod = propertyInfo.GetGetMethod();

            if (this.propertyInfo.CanWrite && !this.isValueType)
                this.setMethod = propertyInfo.GetSetMethod();
        }

        public virtual bool IsValueType { get { return isValueType; } }

        public virtual Type ValueType { get { return this.propertyInfo.PropertyType; } }

        public virtual Type DeclaringType { get { return this.propertyInfo.DeclaringType; } }

        public virtual string Name { get { return this.propertyInfo.Name; } }

        public virtual bool IsStatic { get { return this.propertyInfo.IsStatic(); } }

        public virtual object GetValue(object target)
        {
            if (this.getMethod == null)
                throw new MemberAccessException();

            return this.getMethod.Invoke(target, null);
        }

        public virtual void SetValue(object target, object value)
        {
            if (!propertyInfo.CanWrite)
                throw new MemberAccessException("The value is read-only.");

            if (this.IsValueType)
                throw new NotSupportedException("Assignments of Value type are not supported.");

            if (this.setMethod == null)
                throw new MemberAccessException("The value is read-only.");

            this.setMethod.Invoke(target, new object[] { value });
        }
    }

    public class ProxyPropertyInfo<T, TValue> : ProxyPropertyInfo, IProxyPropertyInfo<T, TValue>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ProxyPropertyInfo<T, TValue>));

        private Func<T, TValue> getter;
        private Action<T, TValue> setter;

        public ProxyPropertyInfo(string propertyName) : this(typeof(T).GetProperty(propertyName))
        {
        }

        public ProxyPropertyInfo(PropertyInfo propertyInfo) : base(propertyInfo)
        {
            if (!typeof(TValue).Equals(this.propertyInfo.PropertyType) || !propertyInfo.DeclaringType.IsAssignableFrom(typeof(T)))
                throw new ArgumentException("The property types do not match!");

            if (this.IsStatic)
                throw new ArgumentException("The property is static!");

            this.getter = this.MakeGetter(propertyInfo);
            this.setter = this.MakeSetter(propertyInfo);
        }

        public ProxyPropertyInfo(string propertyName, Func<T, TValue> getter, Action<T, TValue> setter) : this(typeof(T).GetProperty(propertyName), getter, setter)
        {
        }

        public ProxyPropertyInfo(PropertyInfo propertyInfo, Func<T, TValue> getter, Action<T, TValue> setter) : base(propertyInfo)
        {
            if (!typeof(TValue).Equals(this.propertyInfo.PropertyType) || !propertyInfo.DeclaringType.IsAssignableFrom(typeof(T)))
                throw new ArgumentException("The property types do not match!");

            if (this.IsStatic)
                throw new ArgumentException("The property is static!");

            this.getter = getter;
            this.setter = setter;
        }

        public override Type DeclaringType { get { return typeof(T); } }

        private Action<T, TValue> MakeSetter(PropertyInfo propertyInfo)
        {
            try
            {
                if (this.IsValueType)
                    return null;

                var setMethod = propertyInfo.GetSetMethod();
                if (setMethod == null)
                    return null;
#if NETFX_CORE || NET_4_6 || NET_STANDARD_2_0 || NET46 || NETSTANDARD2_0
                return (Action<T, TValue>)setMethod.CreateDelegate(typeof(Action<T, TValue>));
#elif !UNITY_IOS
                return (Action<T, TValue>)Delegate.CreateDelegate(typeof(Action<T, TValue>), setMethod);
#endif
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("{0}", e);
            }

            return null;
        }

        private Func<T, TValue> MakeGetter(PropertyInfo propertyInfo)
        {
            try
            {
                if (this.IsValueType)
                    return null;

                var getMethod = propertyInfo.GetGetMethod();
                if (getMethod == null)
                    return null;
#if NETFX_CORE || NET_4_6 || NET_STANDARD_2_0 || NET46 || NETSTANDARD2_0
                return (Func<T, TValue>)getMethod.CreateDelegate(typeof(Func<T, TValue>));
#elif !UNITY_IOS
                return (Func<T, TValue>)Delegate.CreateDelegate(typeof(Func<T, TValue>), getMethod);
#endif
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("{0}", e);
            }
            return null;
        }

        public TValue GetValue(T target)
        {
            if (this.getter != null)
                return this.getter(target);

            return (TValue)base.GetValue(target);
        }

        TValue IProxyPropertyInfo<TValue>.GetValue(object target)
        {
            return this.GetValue((T)target);
        }

        public override object GetValue(object target)
        {
            if (this.getter != null)
                return this.getter((T)target);

            return base.GetValue(target);
        }

        public void SetValue(T target, TValue value)
        {
            if (this.IsValueType)
                throw new NotSupportedException("Assignments of Value type are not supported.");

            if (setter != null)
            {
                setter(target, value);
                return;
            }

            base.SetValue(target, value);
        }

        public void SetValue(object target, TValue value)
        {
            this.SetValue((T)target, value);
        }

        public override void SetValue(object target, object value)
        {
            if (this.IsValueType)
                throw new NotSupportedException("Assignments of Value type are not supported.");

            if (setter != null)
            {
                setter((T)target, (TValue)value);
                return;
            }

            base.SetValue(target, value);
        }

    }
}
