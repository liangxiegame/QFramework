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
using BindKit.Binding.Reflection;

#if NETFX_CORE
using System.Reflection;
#endif

namespace BindKit.Binding.Proxy.Sources.Object
{
    public class FieldNodeProxy : SourceProxyBase, IObtainable, IModifiable
    {
        protected IProxyFieldInfo fieldInfo;

        public FieldNodeProxy(IProxyFieldInfo fieldInfo) : this(null, fieldInfo)
        {
        }

        public FieldNodeProxy(object source, IProxyFieldInfo fieldInfo) : base(source)
        {
            this.fieldInfo = fieldInfo;
        }

        public override Type Type { get { return fieldInfo.ValueType; } }

        public virtual object GetValue()
        {
            return fieldInfo.GetValue(source);
        }

        public virtual TValue GetValue<TValue>()
        {
            var proxy = fieldInfo as IProxyFieldInfo<TValue>;
            if (proxy != null)
                return proxy.GetValue(source);

            return (TValue)this.fieldInfo.GetValue(source);
        }

        public virtual void SetValue(object value)
        {
            fieldInfo.SetValue(source, value);
        }

        public virtual void SetValue<TValue>(TValue value)
        {
            var proxy = fieldInfo as IProxyFieldInfo<TValue>;
            if (proxy != null)
            {
                proxy.SetValue(source, value);
                return;
            }

            this.fieldInfo.SetValue(source, value);
        }
    }
}
