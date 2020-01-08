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

using BindKit.Binding.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace BindKit.Binding.Parameters
{
    public class ParameterWrapDelegateInvoker : IInvoker
    {
        private readonly object commandParameter;
        private readonly Delegate handler;

        public ParameterWrapDelegateInvoker(Delegate handler, object commandParameter)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");
            if (commandParameter == null)
                throw new ArgumentNullException("commandParameter");

            this.handler = handler;
            this.commandParameter = commandParameter;

            if (!IsValid(handler))
                throw new ArgumentException("Bind method failed.the parameter types do not match.");
        }

        public object Invoke(params object[] args)
        {
            return this.handler.DynamicInvoke(commandParameter);
        }

        protected virtual bool IsValid(Delegate handler)
        {
#if NETFX_CORE
            MethodInfo info = handler.GetMethodInfo();
#else
            MethodInfo info = handler.Method;
#endif
            if (!info.ReturnType.Equals(typeof(void)))
                return false;

            List<Type> parameterTypes = info.GetParameterTypes();
            if (parameterTypes.Count != 1)
                return false;

            return parameterTypes[0].IsAssignableFrom(commandParameter.GetType());
        }
    }
}
