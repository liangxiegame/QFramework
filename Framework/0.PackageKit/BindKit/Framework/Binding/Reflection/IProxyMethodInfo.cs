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
    public interface IProxyMethodInfo : IProxyMemberInfo
    {
        Type ReturnType { get; }

        ParameterInfo[] Parameters { get; }

        ParameterInfo ReturnParameter { get; }

        object Invoke(object target, params object[] args);
    }

    public interface IProxyFuncInfo<T, TResult> : IProxyMethodInfo
    {
        TResult Invoke(T target);
    }

    public interface IProxyFuncInfo<T, P1, TResult> : IProxyMethodInfo
    {
        TResult Invoke(T target, P1 p1);
    }

    public interface IProxyFuncInfo<T, P1, P2, TResult> : IProxyMethodInfo
    {
        TResult Invoke(T target, P1 p1, P2 p2);
    }

    public interface IProxyFuncInfo<T, P1, P2, P3, TResult> : IProxyMethodInfo
    {
        TResult Invoke(T target, P1 p1, P2 p2, P3 p3);
    }

    public interface IProxyActionInfo<T> : IProxyMethodInfo
    {
        void Invoke(T target);
    }

    public interface IProxyActionInfo<T, P1> : IProxyMethodInfo
    {
        void Invoke(T target, P1 p1);
    }

    public interface IProxyActionInfo<T, P1, P2> : IProxyMethodInfo
    {
        void Invoke(T target, P1 p1, P2 p2);
    }

    public interface IProxyActionInfo<T, P1, P2, P3> : IProxyMethodInfo
    {
        void Invoke(T target, P1 p1, P2 p2, P3 p3);
    }

    public interface IStaticProxyFuncInfo<T, TResult> : IProxyMethodInfo
    {
        TResult Invoke();
    }

    public interface IStaticProxyFuncInfo<T, P1, TResult> : IProxyMethodInfo
    {
        TResult Invoke(P1 p1);
    }

    public interface IStaticProxyFuncInfo<T, P1, P2, TResult> : IProxyMethodInfo
    {
        TResult Invoke(P1 p1, P2 p2);
    }

    public interface IStaticProxyFuncInfo<T, P1, P2, P3, TResult> : IProxyMethodInfo
    {
        TResult Invoke(P1 p1, P2 p2, P3 p3);
    }

    public interface IStaticProxyActionInfo<T> : IProxyMethodInfo
    {
        void Invoke();
    }

    public interface IStaticProxyActionInfo<T, P1> : IProxyMethodInfo
    {
        void Invoke(P1 p1);
    }

    public interface IStaticProxyActionInfo<T, P1, P2> : IProxyMethodInfo
    {
        void Invoke(P1 p1, P2 p2);
    }

    public interface IStaticProxyActionInfo<T, P1, P2, P3> : IProxyMethodInfo
    {
        void Invoke(P1 p1, P2 p2, P3 p3);
    }
}
