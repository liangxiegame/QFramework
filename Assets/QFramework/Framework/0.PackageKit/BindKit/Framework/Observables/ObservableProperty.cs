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

namespace BindKit.Observables
{
    [Serializable]
    public class ObservableProperty : IObservableProperty
    {
        private readonly object _lock = new object();
        private EventHandler valueChanged;
        private object _value;

        public event EventHandler ValueChanged
        {
            add { lock (_lock) { this.valueChanged += value; } }
            remove { lock (_lock) { this.valueChanged -= value; } }
        }

        public ObservableProperty() : this(null)
        {
        }
        public ObservableProperty(object value)
        {
            this._value = value;
        }

        public virtual Type Type { get { return this._value != null ? this._value.GetType() : typeof(object); } }

        public virtual object Value
        {
            get { return this._value; }
            set
            {
                if (object.Equals(this._value, value))
                    return;

                this._value = value;
                this.RaiseValueChanged();
            }
        }

        protected void RaiseValueChanged()
        {
            var handler = this.valueChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        public override string ToString()
        {
            var v = this.Value;
            if (v == null)
                return "";

            return v.ToString();
        }
    }

    public class ObservableProperty<T> : ObservableProperty, IObservableProperty<T>
    {
        public ObservableProperty() : this(default(T))
        {
        }
        public ObservableProperty(T value) : base(value)
        {
        }

        public override Type Type { get { return typeof(T); } }

        public new virtual T Value
        {
            get { return (T)base.Value; }
            set { base.Value = value; }
        }

        public static implicit operator T(ObservableProperty<T> data)
        {
            return data.Value;
        }

        public static implicit operator ObservableProperty<T>(T data)
        {
            return new ObservableProperty<T>(data);
        }
    }

}
