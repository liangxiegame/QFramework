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
using System.Collections.Generic;
using System.Reflection;
using BindKit.Binding.Reflection;
using UnityEngine.Events;

using Loxodon.Log;
using BindKit.Observables;

namespace BindKit.Binding.Proxy.Targets
{
    public class UnityTargetProxyFactory : ITargetProxyFactory
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(UnityTargetProxyFactory));

        public ITargetProxy CreateProxy(object target, BindingDescription description)
        {
            IProxyType type = target.GetType().AsProxy();
            IProxyMemberInfo memberInfo = type.GetMember(description.TargetName);
            if (memberInfo == null)
                memberInfo = type.GetMember(description.TargetName, BindingFlags.Instance | BindingFlags.NonPublic);

            if (memberInfo == null)
                return null;

            UnityEventBase updateTrigger = null;
            if (!string.IsNullOrEmpty(description.UpdateTrigger))
            {
                var updateTriggerPropertyInfo = type.GetProperty(description.UpdateTrigger);
                if (updateTriggerPropertyInfo != null)
                    updateTrigger = updateTriggerPropertyInfo.GetValue(target) as UnityEventBase;

                if (updateTriggerPropertyInfo == null)
                {
                    var updateTriggerFieldInfo = type.GetField(description.UpdateTrigger);
                    if (updateTriggerFieldInfo != null)
                        updateTrigger = updateTriggerFieldInfo.GetValue(target) as UnityEventBase;
                }
            }

            var propertyInfo = memberInfo as IProxyPropertyInfo;
            if (propertyInfo != null)
            {
                if (typeof(IObservableProperty).IsAssignableFrom(propertyInfo.ValueType))
                    return null;

                if (typeof(UnityEventBase).IsAssignableFrom(propertyInfo.ValueType))
                {
                    //Event Type
                    object unityEvent = propertyInfo.GetValue(target);
                    Type[] paramTypes = GetUnityEventParametersType(propertyInfo.ValueType);
                    return CreateUnityEventProxy(target, (UnityEventBase)unityEvent, paramTypes);
                }

                //Other Property Type
                if (updateTrigger == null)/* by UniversalTargetProxyFactory */
                    return null;

                return CreateUnityPropertyProxy(target, propertyInfo, updateTrigger);
            }

            var fieldInfo = memberInfo as IProxyFieldInfo;
            if (fieldInfo != null)
            {
                if (typeof(IObservableProperty).IsAssignableFrom(fieldInfo.ValueType))
                    return null;

                if (typeof(UnityEventBase).IsAssignableFrom(fieldInfo.ValueType))
                {
                    //Event Type
                    object unityEvent = fieldInfo.GetValue(target);
                    Type[] paramTypes = GetUnityEventParametersType(fieldInfo.ValueType);
                    return CreateUnityEventProxy(target, (UnityEventBase)unityEvent, paramTypes);
                }

                //Other Property Type
                if (updateTrigger == null)/* by UniversalTargetProxyFactory */
                    return null;

                return CreateUnityFieldProxy(target, fieldInfo, updateTrigger);
            }

            return null;
        }

        protected virtual ITargetProxy CreateUnityPropertyProxy(object target, IProxyPropertyInfo propertyInfo, UnityEventBase updateTrigger)
        {
            Type type = propertyInfo.ValueType;
#if NETFX_CORE
            TypeCode typeCode = WinRTLegacy.TypeExtensions.GetTypeCode(type);
#else
            TypeCode typeCode = Type.GetTypeCode(type);
#endif

            switch (typeCode)
            {
                case TypeCode.String: return new UnityPropertyProxy<string>(target, propertyInfo, (UnityEvent<string>)updateTrigger);
                case TypeCode.Boolean: return new UnityPropertyProxy<bool>(target, propertyInfo, (UnityEvent<bool>)updateTrigger);
                case TypeCode.SByte: return new UnityPropertyProxy<sbyte>(target, propertyInfo, (UnityEvent<sbyte>)updateTrigger);
                case TypeCode.Byte: return new UnityPropertyProxy<byte>(target, propertyInfo, (UnityEvent<byte>)updateTrigger);
                case TypeCode.Int16: return new UnityPropertyProxy<short>(target, propertyInfo, (UnityEvent<short>)updateTrigger);
                case TypeCode.UInt16: return new UnityPropertyProxy<ushort>(target, propertyInfo, (UnityEvent<ushort>)updateTrigger);
                case TypeCode.Int32: return new UnityPropertyProxy<int>(target, propertyInfo, (UnityEvent<int>)updateTrigger);
                case TypeCode.UInt32: return new UnityPropertyProxy<uint>(target, propertyInfo, (UnityEvent<uint>)updateTrigger);
                case TypeCode.Int64: return new UnityPropertyProxy<long>(target, propertyInfo, (UnityEvent<long>)updateTrigger);
                case TypeCode.UInt64: return new UnityPropertyProxy<ulong>(target, propertyInfo, (UnityEvent<ulong>)updateTrigger);
                case TypeCode.Char: return new UnityPropertyProxy<char>(target, propertyInfo, (UnityEvent<char>)updateTrigger);
                case TypeCode.Single: return new UnityPropertyProxy<float>(target, propertyInfo, (UnityEvent<float>)updateTrigger);
                case TypeCode.Double: return new UnityPropertyProxy<double>(target, propertyInfo, (UnityEvent<double>)updateTrigger);
                case TypeCode.Decimal: return new UnityPropertyProxy<decimal>(target, propertyInfo, (UnityEvent<decimal>)updateTrigger);
                case TypeCode.DateTime: return new UnityPropertyProxy<DateTime>(target, propertyInfo, (UnityEvent<DateTime>)updateTrigger);
                case TypeCode.Object:
                default:
#if UNITY_IOS
                    throw new NotSupportedException();
#else
                    return (ITargetProxy)Activator.CreateInstance(typeof(UnityPropertyProxy<>).MakeGenericType(propertyInfo.ValueType), target, propertyInfo, updateTrigger);
#endif
            }
        }

        protected virtual ITargetProxy CreateUnityFieldProxy(object target, IProxyFieldInfo fieldInfo, UnityEventBase updateTrigger)
        {
            Type type = fieldInfo.ValueType;
#if NETFX_CORE
            TypeCode typeCode = WinRTLegacy.TypeExtensions.GetTypeCode(type);
#else
            TypeCode typeCode = Type.GetTypeCode(type);
#endif

            switch (typeCode)
            {
                case TypeCode.String: return new UnityFieldProxy<string>(target, fieldInfo, (UnityEvent<string>)updateTrigger);
                case TypeCode.Boolean: return new UnityFieldProxy<bool>(target, fieldInfo, (UnityEvent<bool>)updateTrigger);
                case TypeCode.SByte: return new UnityFieldProxy<sbyte>(target, fieldInfo, (UnityEvent<sbyte>)updateTrigger);
                case TypeCode.Byte: return new UnityFieldProxy<byte>(target, fieldInfo, (UnityEvent<byte>)updateTrigger);
                case TypeCode.Int16: return new UnityFieldProxy<short>(target, fieldInfo, (UnityEvent<short>)updateTrigger);
                case TypeCode.UInt16: return new UnityFieldProxy<ushort>(target, fieldInfo, (UnityEvent<ushort>)updateTrigger);
                case TypeCode.Int32: return new UnityFieldProxy<int>(target, fieldInfo, (UnityEvent<int>)updateTrigger);
                case TypeCode.UInt32: return new UnityFieldProxy<uint>(target, fieldInfo, (UnityEvent<uint>)updateTrigger);
                case TypeCode.Int64: return new UnityFieldProxy<long>(target, fieldInfo, (UnityEvent<long>)updateTrigger);
                case TypeCode.UInt64: return new UnityFieldProxy<ulong>(target, fieldInfo, (UnityEvent<ulong>)updateTrigger);
                case TypeCode.Char: return new UnityFieldProxy<char>(target, fieldInfo, (UnityEvent<char>)updateTrigger);
                case TypeCode.Single: return new UnityFieldProxy<float>(target, fieldInfo, (UnityEvent<float>)updateTrigger);
                case TypeCode.Double: return new UnityFieldProxy<double>(target, fieldInfo, (UnityEvent<double>)updateTrigger);
                case TypeCode.Decimal: return new UnityFieldProxy<decimal>(target, fieldInfo, (UnityEvent<decimal>)updateTrigger);
                case TypeCode.DateTime: return new UnityFieldProxy<DateTime>(target, fieldInfo, (UnityEvent<DateTime>)updateTrigger);
                case TypeCode.Object:
                default:
#if UNITY_IOS
                    throw new NotSupportedException();
#else
                    return (ITargetProxy)Activator.CreateInstance(typeof(UnityFieldProxy<>).MakeGenericType(fieldInfo.ValueType), target, fieldInfo, updateTrigger);
#endif
            }
        }

        protected virtual ITargetProxy CreateUnityEventProxy(object target, UnityEventBase unityEvent, Type[] paramTypes)
        {
            switch (paramTypes.Length)
            {
                case 0:
                    return new UnityEventProxy(target, (UnityEvent)unityEvent);
                case 1:
#if NETFX_CORE
                    TypeCode typeCode = WinRTLegacy.TypeExtensions.GetTypeCode(paramTypes[0]);
#else
                    TypeCode typeCode = Type.GetTypeCode(paramTypes[0]);
#endif
                    switch (typeCode)
                    {
                        case TypeCode.String:
                            return new UnityEventProxy<string>(target, (UnityEvent<string>)unityEvent);
                        case TypeCode.Boolean:
                            return new UnityEventProxy<bool>(target, (UnityEvent<bool>)unityEvent);
                        case TypeCode.SByte:
                            return new UnityEventProxy<sbyte>(target, (UnityEvent<sbyte>)unityEvent);
                        case TypeCode.Byte:
                            return new UnityEventProxy<byte>(target, (UnityEvent<byte>)unityEvent);
                        case TypeCode.Int16:
                            return new UnityEventProxy<short>(target, (UnityEvent<short>)unityEvent);
                        case TypeCode.UInt16:
                            return new UnityEventProxy<ushort>(target, (UnityEvent<ushort>)unityEvent);
                        case TypeCode.Int32:
                            return new UnityEventProxy<int>(target, (UnityEvent<int>)unityEvent);
                        case TypeCode.UInt32:
                            return new UnityEventProxy<uint>(target, (UnityEvent<uint>)unityEvent);
                        case TypeCode.Int64:
                            return new UnityEventProxy<long>(target, (UnityEvent<long>)unityEvent);
                        case TypeCode.UInt64:
                            return new UnityEventProxy<ulong>(target, (UnityEvent<ulong>)unityEvent);
                        case TypeCode.Char:
                            return new UnityEventProxy<char>(target, (UnityEvent<char>)unityEvent);
                        case TypeCode.Single:
                            return new UnityEventProxy<float>(target, (UnityEvent<float>)unityEvent);
                        case TypeCode.Double:
                            return new UnityEventProxy<double>(target, (UnityEvent<double>)unityEvent);
                        case TypeCode.Decimal:
                            return new UnityEventProxy<decimal>(target, (UnityEvent<decimal>)unityEvent);
                        case TypeCode.DateTime:
                            return new UnityEventProxy<DateTime>(target, (UnityEvent<DateTime>)unityEvent);
                        case TypeCode.Object:
                        default:
#if UNITY_IOS
                            throw new NotSupportedException();
#else
                            return (ITargetProxy)Activator.CreateInstance(typeof(UnityEventProxy<>).MakeGenericType(paramTypes[0]), target, unityEvent);
#endif
                    }
                default:
                    throw new NotSupportedException("Too many parameters");
            }
        }

        protected Type[] GetUnityEventParametersType(Type type)
        {
            MethodInfo info = type.GetMethod("Invoke");
            if (info == null)
                return new Type[0];

            ParameterInfo[] parameters = info.GetParameters();
            if (parameters == null || parameters.Length <= 0)
                return new Type[0];

            List<Type> types = new List<Type>();
            foreach (ParameterInfo parameter in parameters)
            {
                if (types.Contains(parameter.ParameterType))
                    continue;

                types.Add(parameter.ParameterType);
            }

            return types.ToArray();
        }
    }
}
