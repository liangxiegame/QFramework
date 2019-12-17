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

namespace BindKit.Binding.Proxy.Targets
{
    public class ObservableTargetProxy : ValueTargetProxyBase
    {
        protected readonly IObservableProperty observableProperty;

        public ObservableTargetProxy(object target, IObservableProperty observableProperty) : base(target)
        {
            this.observableProperty = observableProperty;
        }

        public override Type Type { get { return this.observableProperty.Type; } }

        public override BindingMode DefaultMode { get { return BindingMode.TwoWay; } }

        public override object GetValue()
        {
            return observableProperty.Value;
        }

        public override TValue GetValue<TValue>()
        {
            if (this.observableProperty is IObservableProperty<TValue>)
                return ((IObservableProperty<TValue>)this.observableProperty).Value;

            return (TValue)this.observableProperty.Value;
        }

        public override void SetValue(object value)
        {
            this.observableProperty.Value = value;
        }

        public override void SetValue<TValue>(TValue value)
        {
            if (this.observableProperty is IObservableProperty<TValue>)
            {
                ((IObservableProperty<TValue>)this.observableProperty).Value = value;
                return;
            }

            this.observableProperty.Value = value;
        }

        protected override void DoSubscribeForValueChange(object target)
        {
            this.observableProperty.ValueChanged += OnValueChanged;
        }

        protected override void DoUnsubscribeForValueChange(object target)
        {
            this.observableProperty.ValueChanged -= OnValueChanged;
        }

        private void OnValueChanged(object sender, EventArgs e)
        {
            this.RaiseValueChanged();
        }
    }
}
