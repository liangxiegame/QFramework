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

using UnityEngine.Events;

namespace BindKit.Binding.Proxy.Targets
{
    public static class UnityEventExtentions
    {
        public static void AddListener(this UnityEventBase unity, Delegate listener)
        {
            MethodInfo add = unity.GetType().GetMethod("AddListener");
            add.Invoke(unity, new object[] { listener });
        }

        public static void RemoveListener(this UnityEventBase unity, Delegate listener)
        {
            MethodInfo add = unity.GetType().GetMethod("RemoveListener");
            add.Invoke(unity, new object[] { listener });
        }

        public static Type GetListenerType(this UnityEventBase unity)
        {
            MethodInfo info = unity.GetType().GetMethod("AddListener");
            if (info == null)
                return null;

            ParameterInfo[] parameters = info.GetParameters();
            if (parameters == null || parameters.Length <= 0)
                return null;

            return parameters[0].ParameterType;
        }

        public static Type GetInvokeParameterType(this UnityEventBase unity)
        {
            MethodInfo info = unity.GetType().GetMethod("Invoke");
            if (info == null)
                return null;

            ParameterInfo[] parameters = info.GetParameters();
            if (parameters == null || parameters.Length <= 0)
                return null;

            return parameters[0].ParameterType;
        }
    }
}
