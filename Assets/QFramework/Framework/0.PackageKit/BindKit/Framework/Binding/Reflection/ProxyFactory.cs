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

namespace BindKit.Binding.Reflection
{
    public class ProxyFactory
    {
        public static readonly ProxyFactory Default = new ProxyFactory();

        private readonly object _lock = new object();
        private readonly Dictionary<Type, ProxyType> types = new Dictionary<Type, ProxyType>();

        public IProxyType Get(Type type)
        {
            return GetType(type);
        }

        protected virtual ProxyType GetType(Type type)
        {
            ProxyType ret;
            if (this.types.TryGetValue(type, out ret) && ret != null)
                return ret;

            return Create(type);
        }

        public void Register(IProxyMemberInfo proxyMemberInfo)
        {
            if (proxyMemberInfo == null)
                return;

            ProxyType proxyType = this.GetType(proxyMemberInfo.DeclaringType);
            proxyType.Register(proxyMemberInfo);
        }

        public void Unregister(IProxyMemberInfo proxyMemberInfo)
        {
            if (proxyMemberInfo == null)
                return;

            ProxyType proxyType = this.GetType(proxyMemberInfo.DeclaringType);
            proxyType.Unregister(proxyMemberInfo);
        }

        protected ProxyType Create(Type type)
        {
            lock (_lock)
            {
                ProxyType proxyType = new ProxyType(type, this);
                this.types.Add(type, proxyType);
                return proxyType;
            }
        }
    }
}
