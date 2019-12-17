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

using BindKit.Commands;
using System;
using BindKit.Binding.Commands;
using BindKit.Binding.Converters;
using BindKit.Binding.Proxy;
using BindKit.Binding.Reflection;

namespace BindKit.Binding.Parameters
{
    public class ParameterWrapConverter : AbstractConverter
    {
        private readonly object commandParameter;
        public ParameterWrapConverter(object commandParameter)
        {
            if (commandParameter == null)
                throw new ArgumentNullException("commandParameter");

            this.commandParameter = commandParameter;
        }

        public override object Convert(object value)
        {
            if (value == null)
                return null;

            if (value is Delegate)
                return new ParameterWrapDelegateInvoker(value as Delegate, commandParameter);

            if (value is ICommand)
                return new ParameterWrapCommand(value as ICommand, commandParameter);

            if (value is IScriptInvoker)
                return new ParameterWrapScriptInvoker(value as IScriptInvoker, commandParameter);

            if (value is IProxyInvoker)
                return new ParameterWrapProxyInvoker(value as IProxyInvoker, commandParameter);

            throw new NotSupportedException(string.Format("Unsupported type \"{0}\".", value.GetType()));
        }

        public override object ConvertBack(object value)
        {
            throw new NotSupportedException();
        }
    }
}
