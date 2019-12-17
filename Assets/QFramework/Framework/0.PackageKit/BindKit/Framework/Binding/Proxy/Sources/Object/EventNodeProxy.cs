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

namespace BindKit.Binding.Proxy.Sources.Object
{
    public class EventNodeProxy : SourceProxyBase, ISourceProxy, IModifiable
    {
        protected readonly IProxyEventInfo eventInfo;
        private bool disposed = false;
        private bool isStatic = false;
        protected Delegate handler;

        public EventNodeProxy(IProxyEventInfo eventInfo) : this(null, eventInfo)
        {
        }

        public EventNodeProxy(object source, IProxyEventInfo eventInfo) : base(source)
        {
            this.eventInfo = eventInfo;
            this.isStatic = this.eventInfo.IsStatic;
        }

        public override Type Type { get { return this.eventInfo.HandlerType; } }

        public virtual void SetValue<TValue>(TValue value)
        {
            SetValue((object)value);
        }

        public virtual void SetValue(object value)
        {
            if (value != null && !value.GetType().Equals(this.Type))
                throw new ArgumentException("Binding delegate to event failed, mismatched delegate type", "value");

            Unbind(this.Source, this.handler);
            this.handler = (Delegate)value;
            Bind(this.Source, this.handler);
        }

        protected virtual void Bind(object target, Delegate handler)
        {
            if (handler == null)
                return;

            if (this.eventInfo != null)
                this.eventInfo.Add(target, handler);
        }

        protected virtual void Unbind(object target, Delegate handler)
        {
            if (handler == null)
                return;

            if (this.eventInfo != null)
                this.eventInfo.Remove(target, handler);
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                var source = this.Source;
                if (this.isStatic || source != null)
                    this.Unbind(source, handler);

                this.handler = null;
                disposed = true;
                base.Dispose(disposing);
            }
        }
    }
}
