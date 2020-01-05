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

using UnityEngine;

namespace BindKit.Binding.Reflection
{
    public class ProxyType : IProxyType
    {
        private readonly Dictionary<string, IProxyEventInfo> events = new Dictionary<string, IProxyEventInfo>();

        private readonly Dictionary<string, IProxyFieldInfo> fields = new Dictionary<string, IProxyFieldInfo>();

        private readonly Dictionary<string, IProxyPropertyInfo> properties = new Dictionary<string, IProxyPropertyInfo>();

        private readonly Dictionary<string, List<IProxyMethodInfo>> methods = new Dictionary<string, List<IProxyMethodInfo>>();

        private IProxyItemInfo itemInfo;

        private readonly object _lock = new object();
        private readonly ProxyFactory factory;
        private readonly Type type;
        private IProxyType baseType;

        public ProxyType(Type type, ProxyFactory factory)
        {
            this.factory = factory;
            this.type = type;
        }

        public Type Type { get { return this.type; } }

        protected void AddMethodInfo(IProxyMethodInfo methodInfo)
        {
            lock (_lock)
            {
                string name = methodInfo.Name;
                List<IProxyMethodInfo> list;
                if (!this.methods.TryGetValue(name, out list))
                {
                    list = new List<IProxyMethodInfo>();
                    this.methods.Add(name, list);
                }
                list.Add(methodInfo);
            }
        }

        protected void RemoveMethodInfo(IProxyMethodInfo methodInfo)
        {
            lock (_lock)
            {
                string name = methodInfo.Name;
                List<IProxyMethodInfo> list;
                if (!this.methods.TryGetValue(name, out list))
                    return;

                list.Remove(methodInfo);
            }
        }

        protected IProxyMethodInfo GetMethodInfo(string name, Type[] parameterTypes)
        {
            lock (_lock)
            {
                if (!this.methods.ContainsKey(name))
                    return null;

                List<IProxyMethodInfo> list = this.methods[name];
                foreach (IProxyMethodInfo info in list)
                {
                    if (IsParameterMatch(info, parameterTypes))
                        return info;
                }
                return null;
            }
        }

        protected bool IsParameterMatch(IProxyMethodInfo proxyMethodInfo, Type[] parameterTypes)
        {
            ParameterInfo[] parameters = proxyMethodInfo.Parameters;
            if ((parameters == null || parameters.Length == 0) && (parameterTypes == null || parameterTypes.Length == 0))
                return true;

            if (parameters != null && parameterTypes != null && parameters.Length == parameters.Length)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (!parameters[i].Equals(parameterTypes[i]))
                        return false;
                }
                return true;
            }
            return false;
        }

        public void Register(IProxyMemberInfo memberInfo)
        {
            if (!memberInfo.DeclaringType.Equals(type))
                throw new ArgumentException();

            string name = memberInfo.Name;
            if (memberInfo is IProxyPropertyInfo)
            {
                this.properties.Add(name, (IProxyPropertyInfo)memberInfo);
            }
            else if (memberInfo is IProxyMethodInfo)
            {
                this.AddMethodInfo((IProxyMethodInfo)memberInfo);
            }
            else if (memberInfo is IProxyFieldInfo)
            {
                this.fields.Add(name, (IProxyFieldInfo)memberInfo);
            }
            else if (memberInfo is IProxyEventInfo)
            {
                this.events.Add(name, (IProxyEventInfo)memberInfo);
            }
            else if (memberInfo is IProxyItemInfo)
            {
                this.itemInfo = (IProxyItemInfo)memberInfo;
            }
        }

        public void Unregister(IProxyMemberInfo memberInfo)
        {
            if (!memberInfo.DeclaringType.Equals(type))
                throw new ArgumentException();

            string name = memberInfo.Name;
            if (memberInfo is IProxyPropertyInfo)
            {
                this.properties.Remove(name);
            }
            else if (memberInfo is IProxyMethodInfo)
            {
                this.RemoveMethodInfo((IProxyMethodInfo)memberInfo);
            }
            else if (memberInfo is IProxyFieldInfo)
            {
                this.fields.Remove(name);
            }
            else if (memberInfo is IProxyEventInfo)
            {
                this.events.Remove(name);
            }
            else if (memberInfo is IProxyItemInfo)
            {
                if (this.itemInfo == memberInfo)
                    this.itemInfo = null;
            }
        }

        public IProxyType GetBase()
        {
            if (this.baseType != null)
                return this.baseType;

#if NETFX_CORE
            Type _baseType = type.GetTypeInfo().BaseType;
#else
            Type _baseType = type.BaseType;
#endif

            if (_baseType == null)
                return null;

            this.baseType = factory.Get(_baseType);
            return this.baseType;
        }

        public IProxyMemberInfo GetMember(string name)
        {
            if (name.Equals("Item") && typeof(ICollection).IsAssignableFrom(type))
            {
                return GetItem();
            }

            IProxyMemberInfo info = GetProperty(name);
            if (info != null)
                return info;

            info = GetMethod(name);
            if (info != null)
                return info;

            info = GetField(name);
            if (info != null)
                return info;

            info = GetEvent(name);
            if (info != null)
                return info;

            return null;
        }

        public IProxyMemberInfo GetMember(string name, BindingFlags flags)
        {
            if (name.Equals("Item") && typeof(ICollection).IsAssignableFrom(type))
            {
                return GetItem();
            }

            IProxyMemberInfo info = GetProperty(name, flags);
            if (info != null)
                return info;

            info = GetMethod(name, flags);
            if (info != null)
                return info;

            info = GetField(name, flags);
            if (info != null)
                return info;

            info = GetEvent(name);
            if (info != null)
                return info;

            return null;
        }

        public IProxyEventInfo GetEvent(string name)
        {
            IProxyEventInfo info;
            if (this.events.TryGetValue(name, out info))
                return info;

            EventInfo eventInfo = this.type.GetEvent(name);
            if (eventInfo != null && eventInfo.DeclaringType.Equals(type))
            {
                return this.CreateProxyEventInfo(eventInfo);
            }

            IProxyType baseTypeInfo = this.GetBase();
            if (baseTypeInfo == null)
                return null;

            return baseTypeInfo.GetEvent(name);
        }

        public IProxyFieldInfo GetField(string name)
        {
            IProxyFieldInfo info;
            if (this.fields.TryGetValue(name, out info))
                return info;

            FieldInfo fieldInfo = this.type.GetField(name);
            if (fieldInfo != null && fieldInfo.DeclaringType.Equals(type))
            {
                return this.CreateProxyFieldInfo(fieldInfo);
            }

            IProxyType baseTypeInfo = this.GetBase();
            if (baseTypeInfo == null)
                return null;

            return baseTypeInfo.GetField(name);
        }
        public IProxyFieldInfo GetField(string name, BindingFlags flags)
        {
            IProxyFieldInfo info;
            if (this.fields.TryGetValue(name, out info))
                return info;

            FieldInfo fieldInfo = this.type.GetField(name, flags);
            if (fieldInfo != null && fieldInfo.DeclaringType.Equals(type))
            {
                return this.CreateProxyFieldInfo(fieldInfo);
            }

            IProxyType baseTypeInfo = this.GetBase();
            if (baseTypeInfo == null)
                return null;

            return baseTypeInfo.GetField(name, flags);
        }

        public IProxyPropertyInfo GetProperty(string name)
        {
            IProxyPropertyInfo info;
            if (this.properties.TryGetValue(name, out info))
                return info;

            PropertyInfo propertyInfo = this.type.GetProperty(name);
            if (propertyInfo != null && propertyInfo.DeclaringType.Equals(type))
            {
                return this.CreateProxyPropertyInfo(propertyInfo);
            }

            IProxyType baseTypeInfo = this.GetBase();
            if (baseTypeInfo == null)
                return null;

            return baseTypeInfo.GetProperty(name);
        }

        public IProxyPropertyInfo GetProperty(string name, BindingFlags flags)
        {
            IProxyPropertyInfo info;
            if (this.properties.TryGetValue(name, out info))
                return info;

            PropertyInfo propertyInfo = this.type.GetProperty(name, flags);
            if (propertyInfo != null && propertyInfo.DeclaringType.Equals(type))
            {
                return this.CreateProxyPropertyInfo(propertyInfo);
            }

            IProxyType baseTypeInfo = this.GetBase();
            if (baseTypeInfo == null)
                return null;

            return baseTypeInfo.GetProperty(name, flags);
        }

        public IProxyItemInfo GetItem()
        {
            if (this.itemInfo != null)
                return this.itemInfo;

            if (type.IsArray)
            {
                return this.CreateArrayProxyItemInfo(type);
            }
            else
            {
                PropertyInfo propertyInfo = this.type.GetProperty("Item");
                if (propertyInfo != null && propertyInfo.DeclaringType.Equals(type))
                {
                    return this.CreateProxyItemInfo(propertyInfo);
                }

                IProxyType baseTypeInfo = this.GetBase();
                if (baseTypeInfo == null)
                    return null;

                return baseTypeInfo.GetItem();
            }
        }

        public IProxyMethodInfo GetMethod(string name)
        {
            MemberInfo memberInfo = this.type.FindFirstMemberInfo(name);
            if (memberInfo == null)
                return null;

            if (!(memberInfo is MethodInfo))
                return null;

            return this.GetMethod(name, ((MethodInfo)memberInfo).GetParameterTypes().ToArray());
        }

        public virtual IProxyMethodInfo GetMethod(string name, Type[] parameterTypes)
        {
            IProxyMethodInfo info = this.GetMethodInfo(name, parameterTypes);
            if (info != null)
                return info;

            MethodInfo methodInfo = this.type.GetMethod(name, parameterTypes);
            if (methodInfo != null && methodInfo.DeclaringType.Equals(type))
            {
                return this.CreateProxyMethodInfo(methodInfo);
            }

            IProxyType baseTypeInfo = this.GetBase();
            if (baseTypeInfo == null)
                return null;

            return baseTypeInfo.GetMethod(name, parameterTypes);
        }

        public IProxyMethodInfo GetMethod(string name, BindingFlags flags)
        {
            MemberInfo memberInfo = this.type.FindFirstMemberInfo(name, flags);
            if (memberInfo == null)
                return null;

            if (!(memberInfo is MethodInfo))
                return null;

            return this.GetMethod(name, ((MethodInfo)memberInfo).GetParameterTypes().ToArray(), flags);
        }

        public IProxyMethodInfo GetMethod(string name, Type[] parameterTypes, BindingFlags flags)
        {
            IProxyMethodInfo info = this.GetMethodInfo(name, parameterTypes);
            if (info != null)
                return info;

#if NETFX_CORE
            MethodInfo methodInfo = this.type.GetMethod(name, flags);
#else
            MethodInfo methodInfo = this.type.GetMethod(name, flags, null, parameterTypes, null);
#endif
            if (methodInfo != null && methodInfo.DeclaringType.Equals(type))
            {
                return this.CreateProxyMethodInfo(methodInfo);
            }

            IProxyType baseTypeInfo = this.GetBase();
            if (baseTypeInfo == null)
                return null;

            return baseTypeInfo.GetMethod(name, parameterTypes, flags);
        }

        protected IProxyEventInfo CreateProxyEventInfo(EventInfo eventInfo)
        {
            ProxyEventInfo info = new ProxyEventInfo(eventInfo);
            this.events.Add(info.Name, info);
            return info;
        }

        protected IProxyFieldInfo CreateProxyFieldInfo(FieldInfo fieldInfo)
        {
            IProxyFieldInfo info = null;
#if UNITY_IOS
            info = new ProxyFieldInfo(fieldInfo);
#else
            try
            {
                info = (IProxyFieldInfo)Activator.CreateInstance(typeof(ProxyFieldInfo<,>).MakeGenericType(fieldInfo.DeclaringType, fieldInfo.FieldType), fieldInfo);
            }
            catch (Exception)
            {
                info = new ProxyFieldInfo(fieldInfo);
            }
#endif
            if (info != null)
                this.fields.Add(info.Name, info);
            return info;
        }

        protected IProxyPropertyInfo CreateProxyPropertyInfo(PropertyInfo propertyInfo)
        {
            IProxyPropertyInfo info = null;
#if UNITY_IOS
            info = new ProxyPropertyInfo(propertyInfo);
#else
            try
            {
                Type type = propertyInfo.DeclaringType;
                if (propertyInfo.IsStatic())
                {
                    ParameterInfo[] parameters = propertyInfo.GetIndexParameters();
                    if (parameters != null && parameters.Length > 0)
                        throw new NotSupportedException();

                    info = (IProxyPropertyInfo)Activator.CreateInstance(typeof(StaticProxyPropertyInfo<,>).MakeGenericType(type, propertyInfo.PropertyType), propertyInfo);
                }
                else
                {
                    ParameterInfo[] parameters = propertyInfo.GetIndexParameters();
                    if (parameters != null && parameters.Length == 1)
                        throw new NotSupportedException();

                    info = (IProxyPropertyInfo)Activator.CreateInstance(typeof(ProxyPropertyInfo<,>).MakeGenericType(type, propertyInfo.PropertyType), propertyInfo);
                }
            }
            catch (NotSupportedException e)
            {
                throw e;
            }
            catch (Exception)
            {
                info = new ProxyPropertyInfo(propertyInfo);
            }
#endif
            if (info != null)
                this.properties.Add(info.Name, info);
            return info;
        }

        protected IProxyMethodInfo CreateProxyMethodInfo(MethodInfo methodInfo)
        {
            IProxyMethodInfo info = null;
#if UNITY_IOS
            info = new ProxyMethodInfo(methodInfo);
#else
            try
            {
                Type returnType = methodInfo.ReturnType;
                ParameterInfo[] parameters = methodInfo.GetParameters();
                Type type = methodInfo.DeclaringType;
                if (typeof(void).Equals(returnType))
                {
                    if (methodInfo.IsStatic)
                    {
                        if (parameters == null || parameters.Length == 0)
                        {
                            info = (IProxyMethodInfo)Activator.CreateInstance(typeof(StaticProxyActionInfo<>).MakeGenericType(type), methodInfo);
                        }
                        else if (parameters.Length == 1)
                        {
                            info = (IProxyMethodInfo)Activator.CreateInstance(typeof(StaticProxyActionInfo<,>).MakeGenericType(type, parameters[0].ParameterType), methodInfo);
                        }
                        else if (parameters.Length == 2)
                        {
                            info = (IProxyMethodInfo)Activator.CreateInstance(typeof(StaticProxyActionInfo<,,>).MakeGenericType(type, parameters[0].ParameterType, parameters[1].ParameterType), methodInfo);
                        }
                        else if (parameters.Length == 3)
                        {
                            info = (IProxyMethodInfo)Activator.CreateInstance(typeof(StaticProxyActionInfo<,,,>).MakeGenericType(type, parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType), methodInfo);
                        }
                        else
                        {
                            info = new ProxyMethodInfo(methodInfo);
                        }
                    }
                    else
                    {
                        if (parameters == null || parameters.Length == 0)
                        {
                            info = (IProxyMethodInfo)Activator.CreateInstance(typeof(ProxyActionInfo<>).MakeGenericType(type), methodInfo);
                        }
                        else if (parameters.Length == 1)
                        {
                            info = (IProxyMethodInfo)Activator.CreateInstance(typeof(ProxyActionInfo<,>).MakeGenericType(type, parameters[0].ParameterType), methodInfo);
                        }
                        else if (parameters.Length == 2)
                        {
                            info = (IProxyMethodInfo)Activator.CreateInstance(typeof(ProxyActionInfo<,,>).MakeGenericType(type, parameters[0].ParameterType, parameters[1].ParameterType), methodInfo);
                        }
                        else if (parameters.Length == 3)
                        {
                            info = (IProxyMethodInfo)Activator.CreateInstance(typeof(ProxyActionInfo<,,,>).MakeGenericType(type, parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType), methodInfo);
                        }
                        else
                        {
                            info = new ProxyMethodInfo(methodInfo);
                        }
                    }
                }
                else
                {
                    if (methodInfo.IsStatic)
                    {
                        if (parameters == null || parameters.Length == 0)
                        {
                            info = (IProxyMethodInfo)Activator.CreateInstance(typeof(StaticProxyFuncInfo<,>).MakeGenericType(type, returnType), methodInfo);
                        }
                        else if (parameters.Length == 1)
                        {
                            info = (IProxyMethodInfo)Activator.CreateInstance(typeof(StaticProxyFuncInfo<,,>).MakeGenericType(type, parameters[0].ParameterType, returnType), methodInfo);
                        }
                        else if (parameters.Length == 2)
                        {
                            info = (IProxyMethodInfo)Activator.CreateInstance(typeof(StaticProxyFuncInfo<,,,>).MakeGenericType(type, parameters[0].ParameterType, parameters[1].ParameterType, returnType), methodInfo);
                        }
                        else if (parameters.Length == 3)
                        {
                            info = (IProxyMethodInfo)Activator.CreateInstance(typeof(StaticProxyFuncInfo<,,,,>).MakeGenericType(type, parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType, returnType), methodInfo);
                        }
                        else
                        {
                            info = new ProxyMethodInfo(methodInfo);
                        }
                    }
                    else
                    {
                        if (parameters == null || parameters.Length == 0)
                        {
                            info = (IProxyMethodInfo)Activator.CreateInstance(typeof(ProxyFuncInfo<,>).MakeGenericType(type, returnType), methodInfo);
                        }
                        else if (parameters.Length == 1)
                        {
                            info = (IProxyMethodInfo)Activator.CreateInstance(typeof(ProxyFuncInfo<,,>).MakeGenericType(type, parameters[0].ParameterType, returnType), methodInfo);
                        }
                        else if (parameters.Length == 2)
                        {
                            info = (IProxyMethodInfo)Activator.CreateInstance(typeof(ProxyFuncInfo<,,,>).MakeGenericType(type, parameters[0].ParameterType, parameters[1].ParameterType, returnType), methodInfo);
                        }
                        else if (parameters.Length == 3)
                        {
                            info = (IProxyMethodInfo)Activator.CreateInstance(typeof(ProxyFuncInfo<,,,,>).MakeGenericType(type, parameters[0].ParameterType, parameters[1].ParameterType, parameters[2].ParameterType, returnType), methodInfo);
                        }
                        else
                        {
                            info = new ProxyMethodInfo(methodInfo);
                        }
                    }
                }
            }
            catch (Exception)
            {
                info = new ProxyMethodInfo(methodInfo);
            }
#endif
            if (info != null)
                this.AddMethodInfo(info);

            return info;
        }

        protected IProxyItemInfo CreateArrayProxyItemInfo(Type type)
        {
            var elementType = type.GetElementType();

            IProxyItemInfo info = null;
#if UNITY_IOS
            var code = Type.GetTypeCode(elementType);
            switch (code)
            {
                case TypeCode.Boolean:
                    info = new ArrayProxyItemInfo<bool[], bool>();
                    break;
                case TypeCode.Byte:
                    info = new ArrayProxyItemInfo<byte[], byte>();
                    break;
                case TypeCode.Char:
                    info = new ArrayProxyItemInfo<char[], char>();
                    break;
                case TypeCode.DateTime:
                    info = new ArrayProxyItemInfo<DateTime[], DateTime>();
                    break;
                case TypeCode.Decimal:
                    info = new ArrayProxyItemInfo<Decimal[], Decimal>();
                    break;
                case TypeCode.Double:
                    info = new ArrayProxyItemInfo<Double[], Double>();
                    break;
                case TypeCode.Int16:
                    info = new ArrayProxyItemInfo<Int16[], Int16>();
                    break;
                case TypeCode.Int32:
                    info = new ArrayProxyItemInfo<Int32[], Int32>();
                    break;
                case TypeCode.Int64:
                    info = new ArrayProxyItemInfo<Int64[], Int64>();
                    break;
                case TypeCode.SByte:
                    info = new ArrayProxyItemInfo<SByte[], SByte>();
                    break;
                case TypeCode.Single:
                    info = new ArrayProxyItemInfo<Single[], Single>();
                    break;
                case TypeCode.String:
                    info = new ArrayProxyItemInfo<string[], string>();
                    break;
                case TypeCode.UInt16:
                    info = new ArrayProxyItemInfo<UInt16[], UInt16>();
                    break;
                case TypeCode.UInt32:
                    info = new ArrayProxyItemInfo<UInt32[], UInt32>();
                    break;
                case TypeCode.UInt64:
                    info = new ArrayProxyItemInfo<UInt64[], UInt64>();
                    break;
                case TypeCode.Object:
                    {
                        if (type.Equals(typeof(Vector2)))
                            info = new ArrayProxyItemInfo<Vector2[], Vector2>();
                        else if (type.Equals(typeof(Vector3)))
                            info = new ArrayProxyItemInfo<Vector3[], Vector3>();
                        else if (type.Equals(typeof(Vector4)))
                            info = new ArrayProxyItemInfo<Vector4[], Vector4>();
                        else if (type.Equals(typeof(Color)))
                            info = new ArrayProxyItemInfo<Color[], Color>();
                        else if (type.Equals(typeof(Rect)))
                            info = new ArrayProxyItemInfo<Rect[], Rect>();
                        else if (type.Equals(typeof(Quaternion)))
                            info = new ArrayProxyItemInfo<Quaternion[], Quaternion>();
                        else if (type.Equals(typeof(Version)))
                            info = new ArrayProxyItemInfo<Version[], Version>();
                        else
                            info = new ArrayProxyItemInfo(type);
                        break;
                    }                   
                default:
                    info = new ArrayProxyItemInfo(type);
                    break;
            }
#else

            info = (IProxyItemInfo)Activator.CreateInstance(typeof(ArrayProxyItemInfo<,>).MakeGenericType(type, elementType));
#endif
            if (info != null)
                this.itemInfo = info;
            return info;
        }

        protected IProxyItemInfo CreateProxyItemInfo(PropertyInfo propertyInfo)
        {
            Type type = propertyInfo.DeclaringType;
            ParameterInfo[] parameters = propertyInfo.GetIndexParameters();
            if (parameters == null || parameters.Length != 1)
                throw new NotSupportedException();

            IProxyItemInfo info = null;
#if UNITY_IOS
            if (type.IsSubclassOfGenericTypeDefinition(typeof(IDictionary<,>)))
            {
                info = this.CreateDictionaryProxyItemInfo(propertyInfo);
            }
            else if (type.IsSubclassOfGenericTypeDefinition(typeof(IList<>)))
            {
                info = this.CreateListProxyItemInfo(propertyInfo);
            }
            else
            {
                info = new ProxyItemInfo(propertyInfo);
            }
#else
            try
            {
                if (type.IsSubclassOfGenericTypeDefinition(typeof(IDictionary<,>)))
                {
                    info = (IProxyItemInfo)Activator.CreateInstance(typeof(DictionaryProxyItemInfo<,,>).MakeGenericType(type, parameters[0].ParameterType, propertyInfo.PropertyType), propertyInfo);
                }
                else if (type.IsSubclassOfGenericTypeDefinition(typeof(IList<>)))
                {
                    info = (IProxyItemInfo)Activator.CreateInstance(typeof(ListProxyItemInfo<,>).MakeGenericType(type, propertyInfo.PropertyType), propertyInfo);
                }
                else
                {
                    info = new ProxyItemInfo(propertyInfo);
                }

            }
            catch (Exception)
            {
                info = new ProxyItemInfo(propertyInfo);
            }
#endif
            if (info != null)
                this.itemInfo = info;
            return info;
        }

        protected virtual IProxyItemInfo CreateListProxyItemInfo(PropertyInfo propertyInfo)
        {
            var type = propertyInfo.PropertyType;
#if NETFX_CORE
            TypeCode typeCode = WinRTLegacy.TypeExtensions.GetTypeCode(type);
#else
            TypeCode typeCode = Type.GetTypeCode(type);
#endif
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    return new ListProxyItemInfo<IList<bool>, bool>(propertyInfo);
                case TypeCode.Byte:
                    return new ListProxyItemInfo<IList<byte>, byte>(propertyInfo);
                case TypeCode.Char:
                    return new ListProxyItemInfo<IList<char>, char>(propertyInfo);
                case TypeCode.DateTime:
                    return new ListProxyItemInfo<IList<DateTime>, DateTime>(propertyInfo);
                case TypeCode.Decimal:
                    return new ListProxyItemInfo<IList<Decimal>, Decimal>(propertyInfo);
                case TypeCode.Double:
                    return new ListProxyItemInfo<IList<Double>, Double>(propertyInfo);
                case TypeCode.Int16:
                    return new ListProxyItemInfo<IList<Int16>, Int16>(propertyInfo);
                case TypeCode.Int32:
                    return new ListProxyItemInfo<IList<Int32>, Int32>(propertyInfo);
                case TypeCode.Int64:
                    return new ListProxyItemInfo<IList<Int64>, Int64>(propertyInfo);
                case TypeCode.SByte:
                    return new ListProxyItemInfo<IList<SByte>, SByte>(propertyInfo);
                case TypeCode.Single:
                    return new ListProxyItemInfo<IList<Single>, Single>(propertyInfo);
                case TypeCode.String:
                    return new ListProxyItemInfo<IList<String>, String>(propertyInfo);
                case TypeCode.UInt16:
                    return new ListProxyItemInfo<IList<UInt16>, UInt16>(propertyInfo);
                case TypeCode.UInt32:
                    return new ListProxyItemInfo<IList<UInt32>, UInt32>(propertyInfo);
                case TypeCode.UInt64:
                    return new ListProxyItemInfo<IList<UInt64>, UInt64>(propertyInfo);
                case TypeCode.Object:
                    {
                        if (type.Equals(typeof(Vector2)))
                            return new ListProxyItemInfo<IList<Vector2>, Vector2>(propertyInfo);
                        if (type.Equals(typeof(Vector3)))
                            return new ListProxyItemInfo<IList<Vector3>, Vector3>(propertyInfo);
                        if (type.Equals(typeof(Vector4)))
                            return new ListProxyItemInfo<IList<Vector4>, Vector4>(propertyInfo);
                        if (type.Equals(typeof(Color)))
                            return new ListProxyItemInfo<IList<Color>, Color>(propertyInfo);
                        if (type.Equals(typeof(Rect)))
                            return new ListProxyItemInfo<IList<Rect>, Rect>(propertyInfo);
                        if (type.Equals(typeof(Quaternion)))
                            return new ListProxyItemInfo<IList<Quaternion>, Quaternion>(propertyInfo);
                        if (type.Equals(typeof(Version)))
                            return new ListProxyItemInfo<IList<Version>, Version>(propertyInfo);

                        return new ProxyItemInfo(propertyInfo);
                    }
                default:
                    return new ProxyItemInfo(propertyInfo);
            }
        }

        protected virtual IProxyItemInfo CreateDictionaryProxyItemInfo(PropertyInfo propertyInfo)
        {
            ParameterInfo[] parameters = propertyInfo.GetIndexParameters();
            var type = propertyInfo.PropertyType;
#if NETFX_CORE
            TypeCode typeCode = WinRTLegacy.TypeExtensions.GetTypeCode(type);
#else
            TypeCode typeCode = Type.GetTypeCode(type);
#endif

            if (parameters[0].ParameterType.Equals(typeof(string)))
            {
                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        return new DictionaryProxyItemInfo<IDictionary<string, bool>, string, bool>(propertyInfo);
                    case TypeCode.Byte:
                        return new DictionaryProxyItemInfo<IDictionary<string, byte>, string, byte>(propertyInfo);
                    case TypeCode.Char:
                        return new DictionaryProxyItemInfo<IDictionary<string, char>, string, char>(propertyInfo);
                    case TypeCode.DateTime:
                        return new DictionaryProxyItemInfo<IDictionary<string, DateTime>, string, DateTime>(propertyInfo);
                    case TypeCode.Decimal:
                        return new DictionaryProxyItemInfo<IDictionary<string, Decimal>, string, Decimal>(propertyInfo);
                    case TypeCode.Double:
                        return new DictionaryProxyItemInfo<IDictionary<string, Double>, string, Double>(propertyInfo);
                    case TypeCode.Int16:
                        return new DictionaryProxyItemInfo<IDictionary<string, Int16>, string, Int16>(propertyInfo);
                    case TypeCode.Int32:
                        return new DictionaryProxyItemInfo<IDictionary<string, Int32>, string, Int32>(propertyInfo);
                    case TypeCode.Int64:
                        return new DictionaryProxyItemInfo<IDictionary<string, Int64>, string, Int64>(propertyInfo);
                    case TypeCode.SByte:
                        return new DictionaryProxyItemInfo<IDictionary<string, SByte>, string, SByte>(propertyInfo);
                    case TypeCode.Single:
                        return new DictionaryProxyItemInfo<IDictionary<string, Single>, string, Single>(propertyInfo);
                    case TypeCode.String:
                        return new DictionaryProxyItemInfo<IDictionary<string, string>, string, string>(propertyInfo);
                    case TypeCode.UInt16:
                        return new DictionaryProxyItemInfo<IDictionary<string, UInt16>, string, UInt16>(propertyInfo);
                    case TypeCode.UInt32:
                        return new DictionaryProxyItemInfo<IDictionary<string, UInt32>, string, UInt32>(propertyInfo);
                    case TypeCode.UInt64:
                        return new DictionaryProxyItemInfo<IDictionary<string, UInt64>, string, UInt64>(propertyInfo);
                    case TypeCode.Object:
                        {
                            if (type.Equals(typeof(Vector2)))
                                return new DictionaryProxyItemInfo<IDictionary<string, Vector2>, string, Vector2>(propertyInfo);
                            if (type.Equals(typeof(Vector3)))
                                return new DictionaryProxyItemInfo<IDictionary<string, Vector3>, string, Vector3>(propertyInfo);
                            if (type.Equals(typeof(Vector4)))
                                return new DictionaryProxyItemInfo<IDictionary<string, Vector4>, string, Vector4>(propertyInfo);
                            if (type.Equals(typeof(Color)))
                                return new DictionaryProxyItemInfo<IDictionary<string, Color>, string, Color>(propertyInfo);
                            if (type.Equals(typeof(Rect)))
                                return new DictionaryProxyItemInfo<IDictionary<string, Rect>, string, Rect>(propertyInfo);
                            if (type.Equals(typeof(Quaternion)))
                                return new DictionaryProxyItemInfo<IDictionary<string, Quaternion>, string, Quaternion>(propertyInfo);
                            if (type.Equals(typeof(Version)))
                                return new DictionaryProxyItemInfo<IDictionary<string, Version>, string, Version>(propertyInfo);

                            return new ProxyItemInfo(propertyInfo);
                        }
                    default:
                        return new ProxyItemInfo(propertyInfo);
                }
            }
            else if (parameters[0].ParameterType.Equals(typeof(int)))
            {
                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        return new DictionaryProxyItemInfo<IDictionary<int, bool>, int, bool>(propertyInfo);
                    case TypeCode.Byte:
                        return new DictionaryProxyItemInfo<IDictionary<int, byte>, int, byte>(propertyInfo);
                    case TypeCode.Char:
                        return new DictionaryProxyItemInfo<IDictionary<int, char>, int, char>(propertyInfo);
                    case TypeCode.DateTime:
                        return new DictionaryProxyItemInfo<IDictionary<int, DateTime>, int, DateTime>(propertyInfo);
                    case TypeCode.Decimal:
                        return new DictionaryProxyItemInfo<IDictionary<int, Decimal>, int, Decimal>(propertyInfo);
                    case TypeCode.Double:
                        return new DictionaryProxyItemInfo<IDictionary<int, Double>, int, Double>(propertyInfo);
                    case TypeCode.Int16:
                        return new DictionaryProxyItemInfo<IDictionary<int, Int16>, int, Int16>(propertyInfo);
                    case TypeCode.Int32:
                        return new DictionaryProxyItemInfo<IDictionary<int, Int32>, int, Int32>(propertyInfo);
                    case TypeCode.Int64:
                        return new DictionaryProxyItemInfo<IDictionary<int, Int64>, int, Int64>(propertyInfo);
                    case TypeCode.SByte:
                        return new DictionaryProxyItemInfo<IDictionary<int, SByte>, int, SByte>(propertyInfo);
                    case TypeCode.Single:
                        return new DictionaryProxyItemInfo<IDictionary<int, Single>, int, Single>(propertyInfo);
                    case TypeCode.String:
                        return new DictionaryProxyItemInfo<IDictionary<int, string>, int, string>(propertyInfo);
                    case TypeCode.UInt16:
                        return new DictionaryProxyItemInfo<IDictionary<int, UInt16>, int, UInt16>(propertyInfo);
                    case TypeCode.UInt32:
                        return new DictionaryProxyItemInfo<IDictionary<int, UInt32>, int, UInt32>(propertyInfo);
                    case TypeCode.UInt64:
                        return new DictionaryProxyItemInfo<IDictionary<int, UInt64>, int, UInt64>(propertyInfo);
                    case TypeCode.Object:
                        {
                            if (type.Equals(typeof(Vector2)))
                                return new DictionaryProxyItemInfo<IDictionary<int, Vector2>, int, Vector2>(propertyInfo);
                            if (type.Equals(typeof(Vector3)))
                                return new DictionaryProxyItemInfo<IDictionary<int, Vector3>, int, Vector3>(propertyInfo);
                            if (type.Equals(typeof(Vector4)))
                                return new DictionaryProxyItemInfo<IDictionary<int, Vector4>, int, Vector4>(propertyInfo);
                            if (type.Equals(typeof(Color)))
                                return new DictionaryProxyItemInfo<IDictionary<int, Color>, int, Color>(propertyInfo);
                            if (type.Equals(typeof(Rect)))
                                return new DictionaryProxyItemInfo<IDictionary<int, Rect>, int, Rect>(propertyInfo);
                            if (type.Equals(typeof(Quaternion)))
                                return new DictionaryProxyItemInfo<IDictionary<int, Quaternion>, int, Quaternion>(propertyInfo);
                            if (type.Equals(typeof(Version)))
                                return new DictionaryProxyItemInfo<IDictionary<int, Version>, int, Version>(propertyInfo);

                            return new ProxyItemInfo(propertyInfo);
                        }
                    default:
                        return new ProxyItemInfo(propertyInfo);
                }
            }
            else
            {
                return new ProxyItemInfo(propertyInfo);
            }
        }
    }
}
