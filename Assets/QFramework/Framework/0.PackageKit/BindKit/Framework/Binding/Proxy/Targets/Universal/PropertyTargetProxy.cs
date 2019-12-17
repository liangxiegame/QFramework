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
using INotifyPropertyChanged = System.ComponentModel.INotifyPropertyChanged;
using PropertyChangedEventArgs = System.ComponentModel.PropertyChangedEventArgs;

namespace BindKit.Binding.Proxy.Targets
{
    public class PropertyTargetProxy : ValueTargetProxyBase
    {
        //private static readonly ILog log = LogManager.GetLogger(typeof(PropertyTargetProxy));

        protected readonly IProxyPropertyInfo propertyInfo;

        public PropertyTargetProxy(object target, IProxyPropertyInfo propertyInfo) : base(target)
        {
            this.propertyInfo = propertyInfo;
        }

        public override Type Type { get { return this.propertyInfo.ValueType; } }

        public override BindingMode DefaultMode { get { return BindingMode.TwoWay; } }

        public override object GetValue()
        {
            var target = this.Target;
            if (target == null)
                return null;

            return propertyInfo.GetValue(target);
        }

        public override TValue GetValue<TValue>()
        {
            var target = this.Target;
            if (target == null)
                return default(TValue);

            if (propertyInfo is IProxyPropertyInfo<TValue>)
                return ((IProxyPropertyInfo<TValue>)propertyInfo).GetValue(target);

            return (TValue)propertyInfo.GetValue(target);
        }

        public override void SetValue(object value)
        {
            var target = this.Target;
            if (target == null)
                return;

            this.propertyInfo.SetValue(target, value);
        }

        public override void SetValue<TValue>(TValue value)
        {
            var target = this.Target;
            if (target == null)
                return;

            if (propertyInfo is IProxyPropertyInfo<TValue>)
            {
                ((IProxyPropertyInfo<TValue>)propertyInfo).SetValue(target, value);
                return;
            }

            this.propertyInfo.SetValue(target, value);
        }

        protected override void DoSubscribeForValueChange(object target)
        {
            if (target is INotifyPropertyChanged)
            {
                var targetNotify = target as INotifyPropertyChanged;
                targetNotify.PropertyChanged += OnPropertyChanged;
            }
        }

        protected override void DoUnsubscribeForValueChange(object target)
        {
            if (target is INotifyPropertyChanged)
            {
                var targetNotify = target as INotifyPropertyChanged;
                targetNotify.PropertyChanged -= OnPropertyChanged;
            }
        }

        protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var name = e.PropertyName;
            if (string.IsNullOrEmpty(name) || name.Equals(this.propertyInfo.Name))
            {
                var target = this.Target;
                if (target == null)
                    return;

                this.RaiseValueChanged();
            }
        }
    }
}
