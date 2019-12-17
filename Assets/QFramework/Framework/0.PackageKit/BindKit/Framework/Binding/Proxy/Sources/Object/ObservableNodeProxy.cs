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

using BindKit.Observables;
using System;

namespace BindKit.Binding.Proxy.Sources.Object
{
    public class ObservableNodeProxy : NotifiableSourceProxyBase, IObtainable, IModifiable, INotifiable
    {
        protected IObservableProperty property;

        public ObservableNodeProxy(IObservableProperty property) : this(null, property)
        {
        }
        public ObservableNodeProxy(object source, IObservableProperty property) : base(source)
        {
            this.property = property;
            this.property.ValueChanged += OnValueChanged;
        }

        public override Type Type { get { return property.Type; } }

        protected virtual void OnValueChanged(object sender, EventArgs e)
        {
            this.RaiseValueChanged();
        }

        public virtual object GetValue()
        {
            return property.Value;
        }

        public virtual TValue GetValue<TValue>()
        {
            var observableProperty = property as IObservableProperty<TValue>;
            if (observableProperty != null)
                return observableProperty.Value;

            return (TValue)this.property.Value;
        }

        public virtual void SetValue(object value)
        {
            property.Value = value;
        }

        public virtual void SetValue<TValue>(TValue value)
        {
            var observableProperty = property as IObservableProperty<TValue>;
            if (observableProperty != null)
            {
                observableProperty.Value = value;
                return;
            }

            this.property.Value = value;
        }

        #region IDisposable Support    
        private bool disposedValue = false;

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (this.property != null)
                    this.property.ValueChanged -= OnValueChanged;

                disposedValue = true;
                base.Dispose(disposing);
            }
        }
        #endregion
    }
}
