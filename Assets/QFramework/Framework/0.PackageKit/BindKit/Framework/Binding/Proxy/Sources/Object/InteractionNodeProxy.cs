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

// using BindKit.Interactivity;
using Loxodon.Log;
using System;
using System.Collections.Generic;
using System.Reflection;
using BindKit.Binding.Reflection;

namespace BindKit.Binding.Proxy.Sources.Object
{
    public class InteractionNodeProxy : SourceProxyBase, IModifiable
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(InteractionNodeProxy));

        // private readonly IInteractionRequest request;

        private bool disposed = false;
        protected IProxyInvoker invoker;/* Method Binding */
        protected Delegate handler;/* Delegate Binding */
        protected IScriptInvoker scriptInvoker;/* Script Function Binding  */

        // public InteractionNodeProxy(IInteractionRequest request) : this(null, request)
        // {
        // }

        // public InteractionNodeProxy(object source, IInteractionRequest request) : base(source)
        // {
            // this.request = request;
            // this.BindEvent();
        // }

        public override Type Type { get { return typeof(EventHandler); } }

        public virtual void SetValue<TValue>(TValue value)
        {
            this.SetValue((object)value);
        }

        public virtual void SetValue(object value)
        {
            if (value != null && !(value is IProxyInvoker || value is Delegate || value is IScriptInvoker))
                throw new ArgumentException("Binding object to InteractionRequest failed, unsupported object type", "value");

            if (this.invoker != null)
                this.invoker = null;

            if (this.handler != null)
                this.handler = null;

            if (this.scriptInvoker != null)
                this.scriptInvoker = null;

            if (value == null)
                return;

            //Bind Method
            IProxyInvoker invoker = value as IProxyInvoker;
            if (invoker != null)
            {
                if (this.IsValid(invoker))
                {
                    this.invoker = invoker;
                    return;
                }

                throw new ArgumentException("Binding the IProxyInvoker to InteractionRequest failed, mismatched parameter type.");
            }

            //Bind Delegate
            Delegate handler = value as Delegate;
            if (handler != null)
            {
                if (this.IsValid(handler))
                {
                    this.handler = handler;
                    return;
                }

                throw new ArgumentException("Binding the Delegate to InteractionRequest failed, mismatched parameter type.");
            }

            //Bind Script Function
            IScriptInvoker scriptInvoker = value as IScriptInvoker;
            if (scriptInvoker != null)
            {
                this.scriptInvoker = scriptInvoker;
            }
        }

        protected virtual bool IsValid(Delegate handler)
        {
            // if (handler is EventHandler<InteractionEventArgs>)
                // return true;
#if NETFX_CORE
            MethodInfo info = handler.GetMethodInfo();
#else
            MethodInfo info = handler.Method;
#endif
            if (!info.ReturnType.Equals(typeof(void)))
                return false;

            List<Type> parameterTypes = info.GetParameterTypes();
            // if (parameterTypes.Count != 2)
                return false;

            // return parameterTypes[0].IsAssignableFrom(typeof(object)) && parameterTypes[1].IsAssignableFrom(typeof(InteractionEventArgs));
        }

        protected virtual bool IsValid(IProxyInvoker invoker)
        {
            IProxyMethodInfo info = invoker.ProxyMethodInfo;
            if (!info.ReturnType.Equals(typeof(void)))
                return false;

            ParameterInfo[] parameters = info.Parameters;
            // if (parameters == null || parameters.Length != 2)
                return false;

            // return parameters[0].ParameterType.IsAssignableFrom(typeof(object)) && parameters[1].ParameterType.IsAssignableFrom(typeof(InteractionEventArgs));
        }

        protected virtual void BindEvent()
        {
            // if (this.request != null)
                // this.request.Raised += OnRaised;
        }

        protected virtual void UnbindEvent()
        {
            // if (this.request != null)
                // this.request.Raised -= OnRaised;
        }

        // protected virtual void OnRaised(object sender, InteractionEventArgs args)
        // {
        //     try
        //     {
        //         if (this.invoker != null)
        //         {
        //             this.invoker.Invoke(sender, args);
        //             return;
        //         }
        //
        //         if (this.handler != null)
        //         {
        //             if (this.handler is EventHandler<InteractionEventArgs>)
        //                 (this.handler as EventHandler<InteractionEventArgs>)(sender, args);
        //             else
        //             {
        //                 this.handler.DynamicInvoke(sender, args);
        //             }
        //             return;
        //         }
        //
        //         if (this.scriptInvoker != null)
        //         {
        //             this.scriptInvoker.Invoke(sender, args);
        //             return;
        //         }
        //     }
        //     catch (Exception e)
        //     {
        //         if (log.IsWarnEnabled)
        //             log.WarnFormat("{0}", e);
        //     }
        // }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                this.UnbindEvent();
                this.handler = null;
                this.scriptInvoker = null;
                this.invoker = null;
                disposed = true;
                base.Dispose(disposing);
            }
        }

        public InteractionNodeProxy(object source) : base(source)
        {
        }
    }
}
