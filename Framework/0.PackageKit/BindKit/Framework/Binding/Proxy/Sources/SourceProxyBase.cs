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

using Loxodon.Log;
using System;

namespace BindKit.Binding.Proxy.Sources
{
    public abstract class SourceProxyBase : BindingProxyBase, ISourceProxy
    {
        protected readonly object source;

        public SourceProxyBase(object source)
        {
            this.source = source;
        }

        public abstract Type Type { get; }

        public virtual object Source { get { return this.source; } }
    }

    public abstract class NotifiableSourceProxyBase : SourceProxyBase, INotifiable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(NotifiableSourceProxyBase));

        protected readonly object _lock = new object();
        protected EventHandler valueChanged;

        public NotifiableSourceProxyBase(object source) : base(source)
        {
        }

        public virtual event EventHandler ValueChanged
        {
            add
            {
                lock (_lock) { this.valueChanged += value; }
            }

            remove
            {
                lock (_lock) { this.valueChanged -= value; }
            }
        }

        protected virtual void RaiseValueChanged()
        {
            try
            {
                if (this.valueChanged != null)
                    this.valueChanged(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.Warn(e);
            }
        }
    }
}
