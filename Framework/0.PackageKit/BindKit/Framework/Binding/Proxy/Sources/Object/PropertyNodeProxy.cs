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
using BindKit.Binding.Reflection;
#if NETFX_CORE
using System.Reflection;
#endif

using INotifyPropertyChanged = System.ComponentModel.INotifyPropertyChanged;
using PropertyChangedEventArgs = System.ComponentModel.PropertyChangedEventArgs;

namespace BindKit.Binding.Proxy.Sources.Object
{
    public class PropertyNodeProxy : NotifiableSourceProxyBase, IObtainable, IModifiable, INotifiable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PropertyNodeProxy));

        protected IProxyPropertyInfo propertyInfo;

        public PropertyNodeProxy(IProxyPropertyInfo propertyInfo) : this(null, propertyInfo)
        {
        }

        public PropertyNodeProxy(object source, IProxyPropertyInfo propertyInfo) : base(source)
        {
            this.propertyInfo = propertyInfo;

            if (this.source == null)
                return;

            if (this.source is INotifyPropertyChanged)
            {
                var sourceNotify = this.source as INotifyPropertyChanged;
                sourceNotify.PropertyChanged += OnPropertyChanged;
            }
            else
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("The type {0} does not inherit the INotifyPropertyChanged interface and does not support the PropertyChanged event.", propertyInfo.DeclaringType.Name);
            }
        }

        public override Type Type { get { return propertyInfo.ValueType; } }

        protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var name = e.PropertyName;
            if (string.IsNullOrEmpty(name) || name.Equals(propertyInfo.Name))
                this.RaiseValueChanged();
        }

        public virtual object GetValue()
        {
            return propertyInfo.GetValue(source);
        }

        public virtual TValue GetValue<TValue>()
        {
            var proxy = propertyInfo as IProxyPropertyInfo<TValue>;
            if (proxy != null)
                return proxy.GetValue(source);

            return (TValue)this.propertyInfo.GetValue(source);
        }

        public virtual void SetValue(object value)
        {
            propertyInfo.SetValue(source, value);
        }

        public virtual void SetValue<TValue>(TValue value)
        {
            var proxy = propertyInfo as IProxyPropertyInfo<TValue>;
            if (proxy != null)
            {
                proxy.SetValue(source, value);
                return;
            }

            this.propertyInfo.SetValue(source, value);
        }

        #region IDisposable Support    
        private bool disposedValue = false;

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (this.source != null && this.source is INotifyPropertyChanged)
                {
                    var sourceNotify = this.source as INotifyPropertyChanged;
                    sourceNotify.PropertyChanged -= OnPropertyChanged;
                }
                disposedValue = true;
                base.Dispose(disposing);
            }
        }
        #endregion
    }
}
