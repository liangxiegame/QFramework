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
using System.Collections.Generic;

namespace BindKit.Binding.Proxy.Sources.Expressions
{
    public class ExpressionSourceProxy : NotifiableSourceProxyBase, IExpressionSourceProxy
    {
        private bool disposed = false;
        private readonly Type type;
        private readonly Func<object[], object> func;
        private readonly List<ISourceProxy> inners = new List<ISourceProxy>();
        private readonly object[] args;

        public ExpressionSourceProxy(object source, Func<object[], object> func, Type type, List<ISourceProxy> inners) : base(source)
        {
            this.type = type;
            this.func = func;
            this.inners = inners;

            if (source != null)
                this.args = new object[] { source };
            else
                this.args = null;

            if (this.inners == null || this.inners.Count <= 0)
                return;

            foreach (ISourceProxy proxy in this.inners)
            {
                if (proxy is INotifiable)
                    ((INotifiable)proxy).ValueChanged += OnValueChanged;
            }
        }

        public override Type Type { get { return this.type; } }

        public virtual object GetValue()
        {
            return func(this.args);
        }

        public virtual TValue GetValue<TValue>()
        {
            return (TValue)this.GetValue();
        }

        private void OnValueChanged(object sender, EventArgs e)
        {
            RaiseValueChanged();
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (this.inners != null && this.inners.Count > 0)
                    {
                        foreach (ISourceProxy proxy in this.inners)
                        {
                            if (proxy is INotifiable)
                                ((INotifiable)proxy).ValueChanged -= OnValueChanged;
                            proxy.Dispose();
                        }
                        this.inners.Clear();
                    }
                }
                disposed = true;
                base.Dispose(disposing);
            }
        }
    }

    public class ExpressionSourceProxy<T, TResult> : NotifiableSourceProxyBase, IExpressionSourceProxy
    {
        private bool disposed = false;
        private readonly Func<T, TResult> func;
        private readonly List<ISourceProxy> inners;

        public ExpressionSourceProxy(T source, Func<T, TResult> func, List<ISourceProxy> inners) : base(source)
        {
            this.func = func;
            this.inners = inners;

            if (this.inners == null || this.inners.Count <= 0)
                return;

            foreach (ISourceProxy proxy in this.inners)
            {
                if (proxy is INotifiable)
                    ((INotifiable)proxy).ValueChanged += OnValueChanged;
            }
        }

        public override Type Type { get { return typeof(TResult); } }

        public virtual object GetValue()
        {
            return func((T)this.source);
        }

        public virtual TValue GetValue<TValue>()
        {
            return (TValue)GetValue();
        }

        private void OnValueChanged(object sender, EventArgs e)
        {
            RaiseValueChanged();
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (this.inners != null && this.inners.Count > 0)
                    {
                        foreach (ISourceProxy proxy in this.inners)
                        {
                            if (proxy is INotifiable)
                                ((INotifiable)proxy).ValueChanged -= OnValueChanged;

                            proxy.Dispose();
                        }
                        this.inners.Clear();
                    }
                }
                disposed = true;
                base.Dispose(disposing);
            }
        }
    }

    public class ExpressionSourceProxy<TResult> : NotifiableSourceProxyBase, IExpressionSourceProxy
    {
        private bool disposed = false;
        private readonly Func<TResult> func;
        private readonly List<ISourceProxy> inners;

        public ExpressionSourceProxy(Func<TResult> func, List<ISourceProxy> inners) : base(null)
        {
            this.func = func;
            this.inners = inners;

            if (this.inners == null || this.inners.Count <= 0)
                return;

            foreach (ISourceProxy proxy in this.inners)
            {
                if (proxy is INotifiable)
                    ((INotifiable)proxy).ValueChanged += OnValueChanged;
            }
        }

        public override Type Type { get { return typeof(TResult); } }

        public virtual object GetValue()
        {
            return func();
        }

        public virtual TValue GetValue<TValue>()
        {
            return (TValue)GetValue();
        }

        private void OnValueChanged(object sender, EventArgs e)
        {
            RaiseValueChanged();
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (this.inners != null && this.inners.Count > 0)
                    {
                        foreach (ISourceProxy proxy in this.inners)
                        {
                            if (proxy is INotifiable)
                                ((INotifiable)proxy).ValueChanged -= OnValueChanged;

                            proxy.Dispose();
                        }
                        this.inners.Clear();
                    }
                }
                disposed = true;
                base.Dispose(disposing);
            }
        }
    }
}
