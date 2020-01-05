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
using System.Collections;
using System.Text.RegularExpressions;
using BindKit.Binding.Reflection;
#if NETFX_CORE
using System.Reflection;
#endif

using INotifyCollectionChanged = System.Collections.Specialized.INotifyCollectionChanged;
using NotifyCollectionChangedAction = System.Collections.Specialized.NotifyCollectionChangedAction;
using NotifyCollectionChangedEventArgs = System.Collections.Specialized.NotifyCollectionChangedEventArgs;

namespace BindKit.Binding.Proxy.Sources.Object
{
    public abstract class ItemNodeProxy<T> : NotifiableSourceProxyBase, IObtainable, IModifiable, INotifiable
    {
        protected T key;
        protected IProxyItemInfo itemInfo;
        protected bool isList;
        protected Regex regex;

        public ItemNodeProxy(ICollection source, T key, IProxyItemInfo itemInfo) : base(source)
        {
            this.key = key;
            this.isList = (source is IList);

            this.itemInfo = itemInfo;

            if (this.source != null && this.source is INotifyCollectionChanged)
            {
                var sourceCollection = this.source as INotifyCollectionChanged;
                sourceCollection.CollectionChanged += OnCollectionChanged;
            }

            if (!this.isList)
            {
                this.regex = new Regex(@"\[" + this.key + ",", RegexOptions.IgnorePatternWhitespace);
            }
        }

        public override Type Type { get { return this.itemInfo.ValueType; } }

        protected abstract void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e);


        public virtual object GetValue()
        {
            return this.itemInfo.GetValue(source, key);
        }

        public abstract TValue GetValue<TValue>();

        public virtual void SetValue(object value)
        {
            this.itemInfo.SetValue(source, key, value);
        }

        public abstract void SetValue<TValue>(TValue value);

        #region IDisposable Support    
        private bool disposedValue = false;

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (this.source != null && this.source is INotifyCollectionChanged)
                {
                    var sourceCollection = this.source as INotifyCollectionChanged;
                    sourceCollection.CollectionChanged -= OnCollectionChanged;
                }
                disposedValue = true;
                base.Dispose(disposing);
            }
        }
        #endregion
    }

    public class IntItemNodeProxy : ItemNodeProxy<int>
    {
        public IntItemNodeProxy(ICollection source, int key, IProxyItemInfo itemInfo) : base(source, key, itemInfo)
        {
        }

        protected override void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (isList)
            {
                //IList or Array
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Reset:
                        this.RaiseValueChanged();
                        break;
                    case NotifyCollectionChangedAction.Remove:
                    case NotifyCollectionChangedAction.Replace:
                        if (this.key == e.OldStartingIndex || this.key == e.NewStartingIndex)
                            this.RaiseValueChanged();
                        break;
                    case NotifyCollectionChangedAction.Move:
                        if (this.key == e.OldStartingIndex || this.key == e.NewStartingIndex)
                            this.RaiseValueChanged();
                        break;
                    case NotifyCollectionChangedAction.Add:
                        int endIndex = e.NewItems != null ? e.NewStartingIndex + e.NewItems.Count : e.NewStartingIndex + 1;
                        if (this.key >= e.NewStartingIndex && this.key < endIndex)
                            this.RaiseValueChanged();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                //IDictionary
                if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    this.RaiseValueChanged();
                    return;
                }

                if (e.NewItems != null && e.NewItems.Count > 0)
                {
                    foreach (var item in e.NewItems)
                    {
                        if (regex.IsMatch(item.ToString()))
                        {
                            this.RaiseValueChanged();
                            return;
                        }
                    }
                }

                if (e.OldItems != null && e.OldItems.Count > 0)
                {
                    foreach (var item in e.OldItems)
                    {
                        if (regex.IsMatch(item.ToString()))
                        {
                            this.RaiseValueChanged();
                            return;
                        }
                    }
                }
            }
        }

        public override TValue GetValue<TValue>()
        {
            if (!typeof(TValue).IsAssignableFrom(this.itemInfo.ValueType))
                throw new MemberAccessException();

            var proxy = itemInfo as IProxyItemInfo<int, TValue>;
            if (proxy != null)
                return proxy.GetValue(source, key);

            return (TValue)this.itemInfo.GetValue(source, key);
        }

        public override void SetValue<TValue>(TValue value)
        {
            var proxy = itemInfo as IProxyItemInfo<int, TValue>;
            if (proxy != null)
            {
                proxy.SetValue(source, key, value);
                return;
            }

            this.itemInfo.SetValue(source, key, value);
        }
    }

    public class StringItemNodeProxy : ItemNodeProxy<string>
    {
        public StringItemNodeProxy(ICollection source, string key, IProxyItemInfo itemInfo) : base(source, key, itemInfo)
        {
        }

        protected override void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //IDictionary
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                this.RaiseValueChanged();
                return;
            }

            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (var item in e.NewItems)
                {
                    if (regex.IsMatch(item.ToString()))
                    {
                        this.RaiseValueChanged();
                        return;
                    }
                }
            }

            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (var item in e.OldItems)
                {
                    if (regex.IsMatch(item.ToString()))
                    {
                        this.RaiseValueChanged();
                        return;
                    }
                }
            }
        }

        public override TValue GetValue<TValue>()
        {
            if (!typeof(TValue).IsAssignableFrom(this.itemInfo.ValueType))
                throw new InvalidCastException();

            var proxy = itemInfo as IProxyItemInfo<string, TValue>;
            if (proxy != null)
                return proxy.GetValue(source, key);

            return (TValue)this.itemInfo.GetValue(source, key);
        }

        public override void SetValue<TValue>(TValue value)
        {
            var proxy = itemInfo as IProxyItemInfo<string, TValue>;
            if (proxy != null)
            {
                proxy.SetValue(source, key, value);
                return;
            }

            this.itemInfo.SetValue(source, key, value);
        }
    }
}
