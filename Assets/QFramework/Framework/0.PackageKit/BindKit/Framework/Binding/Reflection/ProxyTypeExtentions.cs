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

namespace BindKit.Binding.Reflection
{
    public static class ProxyTypeExtentions
    {
        static readonly ProxyFactory factory = ProxyFactory.Default;
        public static IProxyType AsProxy(this Type type)
        {
            return factory.Get(type);
        }

        public static IProxyEventInfo AsProxy(this EventInfo info)
        {
            IProxyType proxyType = factory.Get(info.DeclaringType);
            return proxyType.GetEvent(info.Name);
        }

        public static IProxyFieldInfo AsProxy(this FieldInfo info)
        {
            IProxyType proxyType = factory.Get(info.DeclaringType);
            if (info.IsPublic)
                return proxyType.GetField(info.Name);
            return proxyType.GetField(info.Name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
        }

        public static IProxyPropertyInfo AsProxy(this PropertyInfo info)
        {
            IProxyType proxyType = factory.Get(info.DeclaringType);
            if (info.GetGetMethod().IsPublic)
                return proxyType.GetProperty(info.Name);
            return proxyType.GetProperty(info.Name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
        }

        public static IProxyMethodInfo AsProxy(this MethodInfo info)
        {
            IProxyType proxyType = factory.Get(info.DeclaringType);
            Type[] types = info.GetParameterTypes().ToArray();
            if (info.IsPublic)
                return proxyType.GetMethod(info.Name, types);

            return proxyType.GetMethod(info.Name, types, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
        }

        public static List<Type> GetParameterTypes(this MethodInfo info)
        {
            List<Type> list = new List<Type>();
            foreach (ParameterInfo p in info.GetParameters())
            {
                list.Add(p.ParameterType);
            }
            return list;
        }
    }
}
