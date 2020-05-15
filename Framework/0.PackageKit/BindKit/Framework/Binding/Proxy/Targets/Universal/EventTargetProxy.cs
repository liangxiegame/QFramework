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
    public class EventTargetProxy : EventTargetProxyBase
    {
        private bool disposed = false;
        protected readonly IProxyEventInfo eventInfo;
        protected Delegate handler;

        public EventTargetProxy(object target, IProxyEventInfo eventInfo) : base(target)
        {
            this.eventInfo = eventInfo;
        }

        public override Type Type { get { return this.eventInfo.HandlerType; } }

        public override BindingMode DefaultMode { get { return BindingMode.OneWay; } }

        public override void SetValue(object value)
        {
            if (value != null && !value.GetType().Equals(this.Type))
                throw new ArgumentException("Binding delegate to event failed, mismatched delegate type", "value");

            var target = this.Target;
            if (target == null)
                return;

            Unbind(target);

            if (value == null)
                return;

            if (value.GetType().Equals(this.Type))
            {
                this.handler = (Delegate)value;
                Bind(target);
                return;
            }
        }

        public override void SetValue<TValue>(TValue value)
        {
            this.SetValue((object)value);
        }

        protected virtual void Bind(object target)
        {
            if (this.handler == null)
                return;

            if (this.eventInfo != null)
                this.eventInfo.Add(target, handler);
        }

        protected virtual void Unbind(object target)
        {
            if (this.handler == null)
                return;

            if (this.eventInfo != null)
                this.eventInfo.Remove(target, handler);

            this.handler = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                var target = this.Target;
                if (this.eventInfo.IsStatic || target != null)
                    this.Unbind(target);

                disposed = true;
                base.Dispose(disposing);
            }
        }
    }
}
