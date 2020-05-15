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

namespace BindKit.Binding.Reflection
{
    public interface IProxyType
    {
        Type Type { get; }

        IProxyMemberInfo GetMember(string name);

        IProxyMemberInfo GetMember(string name, BindingFlags flags);

        IProxyFieldInfo GetField(string name);

        IProxyFieldInfo GetField(string name, BindingFlags flags);

        IProxyPropertyInfo GetProperty(string name);

        IProxyPropertyInfo GetProperty(string name, BindingFlags flags);

        IProxyItemInfo GetItem();

        IProxyMethodInfo GetMethod(string name);

        IProxyMethodInfo GetMethod(string name, BindingFlags flags);

        IProxyMethodInfo GetMethod(string name, Type[] parameterTypes);

        IProxyMethodInfo GetMethod(string name, Type[] parameterTypes, BindingFlags flags);

        IProxyEventInfo GetEvent(string name);
    }
}
