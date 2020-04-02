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
using BindKit.Binding.Binders;
using Loxodon.Log;

namespace BindKit.Binding.Contexts
{
    public class BindingContext : IBindingContext
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BindingContext));

        private readonly string DEFAULT_KEY = "_KEY_";
        private readonly Dictionary<object, List<IBinding>> bindings = new Dictionary<object, List<IBinding>>();

        private IBinder binder;
        private object owner;
        private object dataContext;
        private readonly object _lock = new object();
        private EventHandler dataContextChanged;

        public event EventHandler DataContextChanged
        {
            add { lock (_lock) { this.dataContextChanged += value; } }
            remove { lock (_lock) { this.dataContextChanged -= value; } }
        }

        public BindingContext(IBinder binder) : this(null, binder, (object)null)
        {
        }

        public BindingContext(object owner, IBinder binder) : this(owner, binder, (object)null)
        {
        }

        public BindingContext(object owner, IBinder binder, object dataContext)
        {
            this.owner = owner;
            this.binder = binder;
            this.DataContext = dataContext;
        }

        public BindingContext(object owner, IBinder binder, IDictionary<object, IEnumerable<BindingDescription>> firstBindings) : this(owner, binder, null, firstBindings)
        {
        }

        public BindingContext(object owner, IBinder binder, object dataContext, IDictionary<object, IEnumerable<BindingDescription>> firstBindings)
        {
            this.owner = owner;
            this.binder = binder;
            this.DataContext = dataContext;

            if (firstBindings != null && firstBindings.Count > 0)
            {
                foreach (var kvp in firstBindings)
                {
                    this.Add(kvp.Key, kvp.Value);
                }
            }
        }

        protected IBinder Binder
        {
            get { return this.binder; }
        }

        public object Owner { get { return this.owner; } }

        public object DataContext
        {
            get { return this.dataContext; }
            set
            {
                if (this.dataContext == value)
                    return;

                this.dataContext = value;
                this.OnDataContextChanged();
                this.RaiseDataContextChanged();
            }
        }

        protected void RaiseDataContextChanged()
        {
            try
            {
                var handler = this.dataContextChanged;
                if (handler != null)
                    handler(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.Warn(e);
            }
        }

        protected virtual void OnDataContextChanged()
        {
            try
            {
                foreach (var kv in this.bindings)
                {
                    foreach (var binding in kv.Value)
                    {
                        binding.DataContext = this.DataContext;
                    }
                }
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.Warn(e);
            }
        }

        protected List<IBinding> GetOrCreateList(object key)
        {
            if (key == null)
                key = DEFAULT_KEY;

            List<IBinding> list;
            if (this.bindings.TryGetValue(key, out list))
                return list;

            list = new List<IBinding>();
            this.bindings.Add(key, list);
            return list;
        }


        public virtual void Add(IBinding binding, object key = null)
        {
            if (binding == null)
                return;

            List<IBinding> list = this.GetOrCreateList(key);
            binding.BindingContext = this;
            list.Add(binding);
        }

        public virtual void Add(IEnumerable<IBinding> bindings, object key = null)
        {
            if (bindings == null)
                return;

            List<IBinding> list = this.GetOrCreateList(key);
            foreach (IBinding binding in bindings)
            {
                binding.BindingContext = this;
                list.Add(binding);
            }
        }

        public virtual void Add(object target, BindingDescription description, object key = null)
        {
            IBinding binding = this.Binder.Bind(this, this.DataContext, target, description);
            this.Add(binding, key);
        }

        public virtual void Add(object target, IEnumerable<BindingDescription> descriptions, object key = null)
        {
            IEnumerable<IBinding> bindings = this.Binder.Bind(this, this.DataContext, target, descriptions);
            this.Add(bindings, key);
        }

        public virtual void Clear(object key)
        {
            if (key == null)
                return;

            List<IBinding> list = this.bindings[key];
            if (list != null && list.Count > 0)
            {
                foreach (IBinding binding in list)
                {
                    binding.Dispose();
                }
            }
            this.bindings.Remove(key);
        }

        public virtual void Clear()
        {
            foreach (var kv in this.bindings)
            {
                foreach (var binding in kv.Value)
                {
                    binding.Dispose();
                }
            }
            this.bindings.Clear();
        }

        #region IDisposable Support
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    this.Clear();
                    this.owner = null;
                    this.binder = null;
                }
                disposed = true;
            }
        }

        ~BindingContext()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}