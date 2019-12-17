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

namespace BindKit.Binding.Proxy.Targets
{
    public class FieldTargetProxy : ValueTargetProxyBase
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(FieldTargetProxy));

        protected readonly IProxyFieldInfo fieldInfo;

        public FieldTargetProxy(object target, IProxyFieldInfo fieldInfo) : base(target)
        {
            this.fieldInfo = fieldInfo;
        }

        public override Type Type { get { return this.fieldInfo.ValueType; } }

        public override BindingMode DefaultMode { get { return BindingMode.OneWay; } }

        public override object GetValue()
        {
            var target = this.Target;
            if (target == null)
                return null;

            return fieldInfo.GetValue(target);
        }

        public override TValue GetValue<TValue>()
        {
            var target = this.Target;
            if (target == null)
                return default(TValue);

            if (fieldInfo is IProxyFieldInfo<TValue>)
                return ((IProxyFieldInfo<TValue>)fieldInfo).GetValue(target);

            return (TValue)fieldInfo.GetValue(target);
        }

        public override void SetValue(object value)
        {
            var target = this.Target;
            if (target == null)
                return;

            this.fieldInfo.SetValue(target, value);
        }

        public override void SetValue<TValue>(TValue value)
        {
            var target = this.Target;
            if (target == null)
                return;

            if (fieldInfo is IProxyFieldInfo<TValue>)
            {
                ((IProxyFieldInfo<TValue>)fieldInfo).SetValue(target, value);
                return;
            }

            this.fieldInfo.SetValue(target, value);
        }
    }
}
