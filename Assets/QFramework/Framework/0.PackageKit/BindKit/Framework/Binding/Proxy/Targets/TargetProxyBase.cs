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
using UnityEngine.EventSystems;

namespace BindKit.Binding.Proxy.Targets
{
    public abstract class TargetProxyBase : BindingProxyBase, ITargetProxy
    {
        private readonly WeakReference target;
        protected readonly string targetName;
        public TargetProxyBase(object target)
        {
            if (target != null)
            {
                this.target = new WeakReference(target, true);
                this.targetName = target.ToString();
            }
        }

        public abstract Type Type { get; }

        public virtual object Target
        {
            get
            {
                var target = this.target != null ? this.target.Target : null;
                return IsAlive(target) ? target : null;
            }
        }
        private bool IsAlive(object target)
        {
            try
            {
                if (target is UIBehaviour)
                {
                    if (((UIBehaviour)target).IsDestroyed())
                        return false;
                    return true;
                }

                if (target is UnityEngine.Object)
                {
                    //Check if the object is valid because it may have been destroyed.
                    //Unmanaged objects,the weak caches do not accurately track the validity of objects.
                    var name = ((UnityEngine.Object)target).name;
                    return true;
                }

                return target != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public virtual BindingMode DefaultMode { get { return BindingMode.OneWay; } }
    }

    public abstract class ValueTargetProxyBase : TargetProxyBase, IModifiable, IObtainable, INotifiable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ValueTargetProxyBase));

        private bool disposed = false;
        private bool subscribed = false;

        protected readonly object _lock = new object();
        protected EventHandler valueChanged;

        public ValueTargetProxyBase(object target) : base(target)
        {
        }

        public event EventHandler ValueChanged
        {
            add
            {
                lock (_lock)
                {
                    this.valueChanged += value;

                    if (this.valueChanged != null && !this.subscribed)
                        this.Subscribe();
                }
            }

            remove
            {
                lock (_lock)
                {
                    this.valueChanged -= value;

                    if (this.valueChanged == null && this.subscribed)
                        this.Unsubscribe();
                }
            }
        }

        protected void Subscribe()
        {
            try
            {
                if (subscribed)
                    return;

                var target = this.Target;
                if (target == null)
                    return;

                this.subscribed = true;
                this.DoSubscribeForValueChange(target);
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("{0} Subscribe Exception:{1}", this.targetName, e);
            }
        }

        protected virtual void DoSubscribeForValueChange(object target)
        {
        }

        protected void Unsubscribe()
        {
            try
            {
                if (!subscribed)
                    return;

                var target = this.Target;
                if (target == null)
                    return;

                this.subscribed = false;
                this.DoUnsubscribeForValueChange(target);
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("{0} Unsubscribe Exception:{1}", this.targetName, e);
            }
        }
        protected virtual void DoUnsubscribeForValueChange(object target)
        {
        }

        public abstract object GetValue();

        public abstract TValue GetValue<TValue>();

        public abstract void SetValue<TValue>(TValue value);

        public abstract void SetValue(object value);

        protected void RaiseValueChanged()
        {
            try
            {
                var handler = this.valueChanged;
                if (handler != null)
                    handler(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                log.WarnFormat("{0}", e);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;
                lock (_lock)
                {
                    this.Unsubscribe();
                }
                base.Dispose(disposing);
            }
        }
    }

    public abstract class EventTargetProxyBase : TargetProxyBase, IModifiable
    {
        public EventTargetProxyBase(object target) : base(target)
        {
        }

        public abstract void SetValue(object value);

        public abstract void SetValue<TValue>(TValue value);
    }
}
