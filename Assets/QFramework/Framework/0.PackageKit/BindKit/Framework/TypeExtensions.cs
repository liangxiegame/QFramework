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
using System.Reflection;
using BindKit.Observables;
using Loxodon.Log;

namespace BindKit
{
    public static class TypeExtensions
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(TypeExtensions));

        public static bool IsSubclassOfGenericTypeDefinition(this Type type, Type genericTypeDefinition)
        {
#if NETFX_CORE
            if (!genericTypeDefinition.GetTypeInfo().IsGenericTypeDefinition)
#else
            if (!genericTypeDefinition.IsGenericTypeDefinition)
#endif
                return false;

#if NETFX_CORE
            if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition().Equals(genericTypeDefinition))
#else
            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(genericTypeDefinition))
#endif
                return true;

#if NETFX_CORE
            Type baseType = type.GetTypeInfo().BaseType;
#else
            Type baseType = type.BaseType;
#endif
            if (baseType != null && baseType != typeof(object))
            {
                if (IsSubclassOfGenericTypeDefinition(baseType, genericTypeDefinition))
                    return true;
            }

            foreach (Type t in type.GetInterfaces())
            {
                if (IsSubclassOfGenericTypeDefinition(t, genericTypeDefinition))
                    return true;
            }

            return false;
        }

        public static object CreateDefault(this Type type)
        {
            if (type == null)
                return null;

            if (type.Equals(typeof(string)))
                return "";
#if NETFX_CORE
            if (!type.GetTypeInfo().IsValueType)
#else
            if (!type.IsValueType)
#endif
                return null;

            if (Nullable.GetUnderlyingType(type) != null)
                return null;

            return Activator.CreateInstance(type);
        }

        public static bool IsStatic(this MemberInfo info)
        {
            var fieldInfo = info as FieldInfo;
            if (fieldInfo != null)
                return fieldInfo.IsStatic;

            var propertyInfo = info as PropertyInfo;
            if (propertyInfo != null)
            {
                var method = propertyInfo.GetGetMethod();
                if (method != null)
                    return method.IsStatic;

                method = propertyInfo.GetSetMethod();
                if (method != null)
                    return method.IsStatic;
            }

            var methodInfo = info as MethodInfo;
            if (methodInfo != null)
                return methodInfo.IsStatic;

            var eventInfo = info as EventInfo;
            if (eventInfo != null)
            {
                var method = eventInfo.GetAddMethod();
                if (method != null)
                    return method.IsStatic;

                method = eventInfo.GetRemoveMethod();
                if (method != null)
                    return method.IsStatic;
            }

            return false;
        }

        public static object ToSafe(this Type type, object value)
        {
            if (value == null)
                return type.CreateDefault();

            var safeValue = value;
            try
            {
                if (!type.IsInstanceOfType(value))
                {
                    if (value is IObservableProperty)
                    {
                        safeValue = (value as IObservableProperty).Value;
                        if (!type.IsInstanceOfType(safeValue))
                        {
                            safeValue = ChangeType(safeValue, type);
                        }
                    }
                    else if (type == typeof(string))
                    {
                        safeValue = value.ToString();
                    }
#if NETFX_CORE
                    else if (type.GetTypeInfo().IsEnum)
#else
                    else if (type.IsEnum)
#endif
                    {
                        var s = value as string;
                        safeValue = s != null ? Enum.Parse(type, s, true) : Enum.ToObject(type, value);
                    }
#if NETFX_CORE
                    else if (type.GetTypeInfo().IsValueType)
#else
                    else if (type.IsValueType)
#endif
                    {
                        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
                        safeValue = underlyingType == typeof(bool) ? ConvertToBoolean(value) : ChangeType(value, underlyingType);
                    }
                    else
                    {
                        safeValue = ChangeType(value, type);
                    }
                }
            }
            catch (Exception) { }

            return safeValue;
        }

        private static bool ConvertToBoolean(object result)
        {
            if (result == null)
                return false;

            var s = result as string;
            if (s != null)
                return s.ToLower().Equals("true");

            if (result is bool)
                return (bool)result;

            var resultType = result.GetType();
#if NETFX_CORE
            if (resultType.GetTypeInfo().IsValueType)
#else
            if (resultType.IsValueType)
#endif
            {
                var underlyingType = Nullable.GetUnderlyingType(resultType) ?? resultType;
                return !result.Equals(underlyingType.CreateDefault());
            }

            return true;
        }

        private static object ChangeType(object value, Type type)
        {
            try
            {
                return Convert.ChangeType(value, type);
            }
            catch (Exception)
            {
                return value;
            }
        }
    }
}
