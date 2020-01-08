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

namespace BindKit.Binding.Reflection
{
    public interface IProxyObject:IDisposable
    {
        event EventHandler ValueChanged;

        Type Type { get; }

        bool IsRoot { get; }

        object Value { get; set; }

        T GetProperty<T>(string name);

        void SetProperty<T>(string name, T value);

        T GetField<T>(string name);

        void SetField<T>(string name, T value);

        object Invoke(string name, params object[] args);

        object Invoke(string name,Type[] parameterTypes, params object[] args);

        IProxyObject GetPropertyProxy(string name);

        IProxyInvoker GetMethodInvoker(string name);

        IProxyInvoker GetMethodInvoker(string name, Type[] parameterTypes);
    }

    public interface IProxyObject<T> : IProxyObject
    {
        new T Value { get; set; }

        IProxyObject<E> GetPropertyProxy<E>(string name);

        TResult Invoke<TResult>(string name, params object[] args);

        TResult Invoke<TResult>(string name, Type[] parameterTypes, params object[] args);
    }

    public interface IProxyCollection : IProxyObject
    {
        object this[object key] { get; set; }

        IProxyObject GetItemProxy(object key);
    }

    public interface IProxyCollection<T> : IProxyObject<T>, IProxyCollection
    {       
        new T this[object key] { get; set; }

        new IProxyObject<T> GetItemProxy(object key);
    }
}
