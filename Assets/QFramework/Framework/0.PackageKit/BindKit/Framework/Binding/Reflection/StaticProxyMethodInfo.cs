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
using System.Collections.Generic;
using System.Reflection;

namespace BindKit.Binding.Reflection
{
    public class StaticProxyFuncInfo<T, TResult> : ProxyMethodInfo, IStaticProxyFuncInfo<T, TResult>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(StaticProxyFuncInfo<T, TResult>));
        private Func<TResult> function;

        public StaticProxyFuncInfo(string methodName) : this(typeof(T).GetMethod(methodName), null)
        {
        }

        public StaticProxyFuncInfo(string methodName, Type[] parameterTypes) : this(typeof(T).GetMethod(methodName, parameterTypes), null)
        {
        }

        public StaticProxyFuncInfo(MethodInfo info) : this(info, null)
        {
        }

        public StaticProxyFuncInfo(string methodName, Func<TResult> function) : this(typeof(T).GetMethod(methodName), function)
        {
        }

        public StaticProxyFuncInfo(string methodName, Type[] parameterTypes, Func<TResult> function) : this(typeof(T).GetMethod(methodName, parameterTypes), function)
        {
        }

        public StaticProxyFuncInfo(MethodInfo info, Func<TResult> function) : base(info)
        {
            if (!this.methodInfo.IsStatic)
                throw new ArgumentException("The method isn't static!");

            if (!typeof(TResult).Equals(this.methodInfo.ReturnType) || !typeof(T).Equals(this.methodInfo.DeclaringType))
                throw new ArgumentException("The method types do not match!");

            this.function = function;
            if (this.function == null)
                this.function = this.MakeFunc(this.methodInfo);
        }

        public override Type DeclaringType { get { return typeof(T); } }

        private Func<TResult> MakeFunc(MethodInfo methodInfo)
        {
            try
            {
#if NETFX_CORE
                return (Func<TResult>)methodInfo.CreateDelegate(typeof(Func<TResult>));
#elif !UNITY_IOS
                return (Func<TResult>)Delegate.CreateDelegate(typeof(Func<TResult>), methodInfo);
#endif
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("{0}", e);
            }
            return null;
        }

        public virtual TResult Invoke()
        {
            if (this.function != null)
                return this.function();

            return (TResult)this.methodInfo.Invoke(null, null);
        }

        public override object Invoke(object target, params object[] args)
        {
            return Invoke();
        }
    }

    public class StaticProxyFuncInfo<T, P1, TResult> : ProxyMethodInfo, IStaticProxyFuncInfo<T, P1, TResult>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(StaticProxyFuncInfo<T, P1, TResult>));
        private Func<P1, TResult> function;
        public StaticProxyFuncInfo(string methodName) : this(typeof(T).GetMethod(methodName), null)
        {
        }

        public StaticProxyFuncInfo(string methodName, Type[] parameterTypes) : this(typeof(T).GetMethod(methodName, parameterTypes), null)
        {
        }

        public StaticProxyFuncInfo(MethodInfo info) : this(info, null)
        {
        }

        public StaticProxyFuncInfo(string methodName, Func<P1, TResult> function) : this(typeof(T).GetMethod(methodName), function)
        {
        }

        public StaticProxyFuncInfo(string methodName, Type[] parameterTypes, Func<P1, TResult> function) : this(typeof(T).GetMethod(methodName, parameterTypes), function)
        {
        }


        public StaticProxyFuncInfo(MethodInfo info, Func<P1, TResult> function) : base(info)
        {
            if (!this.methodInfo.IsStatic)
                throw new ArgumentException("The method isn't static!");

            if (!typeof(TResult).Equals(this.methodInfo.ReturnType) || !typeof(T).Equals(this.methodInfo.DeclaringType))
                throw new ArgumentException("The method types do not match!");

            ParameterInfo[] parameters = this.methodInfo.GetParameters();
            if (parameters.Length != 1 || !typeof(P1).Equals(parameters[0].ParameterType))
                throw new ArgumentException("The method types do not match!");

            this.function = function;
            if (this.function == null)
                this.function = this.MakeFunc(this.methodInfo);

        }

        public override Type DeclaringType { get { return typeof(T); } }

        private Func<P1, TResult> MakeFunc(MethodInfo methodInfo)
        {
            try
            {
#if NETFX_CORE
                return (Func<P1, TResult>)methodInfo.CreateDelegate(typeof(Func<P1, TResult>));
#elif !UNITY_IOS
                return (Func<P1, TResult>)Delegate.CreateDelegate(typeof(Func<P1, TResult>), methodInfo);
#endif
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("{0}", e);
            }
            return null;
        }

        public virtual TResult Invoke(P1 p1)
        {
            if (this.function != null)
                return this.function(p1);

            return (TResult)this.methodInfo.Invoke(null, new object[] { p1 });
        }

        public override object Invoke(object target, params object[] args)
        {
            return Invoke((P1)args[0]);
        }
    }

    public class StaticProxyFuncInfo<T, P1, P2, TResult> : ProxyMethodInfo, IStaticProxyFuncInfo<T, P1, P2, TResult>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(StaticProxyFuncInfo<T, P1, P2, TResult>));
        private Func<P1, P2, TResult> function;

        public StaticProxyFuncInfo(string methodName) : this(typeof(T).GetMethod(methodName), null)
        {
        }

        public StaticProxyFuncInfo(string methodName, Type[] parameterTypes) : this(typeof(T).GetMethod(methodName, parameterTypes), null)
        {
        }

        public StaticProxyFuncInfo(MethodInfo info) : this(info, null)
        {
        }

        public StaticProxyFuncInfo(string methodName, Func<P1, P2, TResult> function) : this(typeof(T).GetMethod(methodName), function)
        {
        }

        public StaticProxyFuncInfo(string methodName, Type[] parameterTypes, Func<P1, P2, TResult> function) : this(typeof(T).GetMethod(methodName, parameterTypes), function)
        {
        }


        public StaticProxyFuncInfo(MethodInfo info, Func<P1, P2, TResult> function) : base(info)
        {
            if (!this.methodInfo.IsStatic)
                throw new ArgumentException("The method isn't static!");

            if (!typeof(TResult).Equals(this.methodInfo.ReturnType) || !typeof(T).Equals(this.methodInfo.DeclaringType))
                throw new ArgumentException("The method types do not match!");

            ParameterInfo[] parameters = this.methodInfo.GetParameters();
            if (parameters.Length != 2 || !typeof(P1).Equals(parameters[0].ParameterType) || !typeof(P2).Equals(parameters[1].ParameterType))
                throw new ArgumentException("The method types do not match!");

            this.function = function;
            if (this.function == null)
                this.function = this.MakeFunc(this.methodInfo);

        }

        public override Type DeclaringType { get { return typeof(T); } }

        private Func<P1, P2, TResult> MakeFunc(MethodInfo methodInfo)
        {
            try
            {
#if NETFX_CORE
                return (Func<P1, P2,TResult>)methodInfo.CreateDelegate(typeof(Func<P1,P2, TResult>));
#elif !UNITY_IOS
                return (Func<P1, P2, TResult>)Delegate.CreateDelegate(typeof(Func<P1, P2, TResult>), methodInfo);
#endif
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("{0}", e);
            }
            return null;
        }

        public virtual TResult Invoke(P1 p1, P2 p2)
        {
            if (this.function != null)
                return this.function(p1, p2);

            return (TResult)this.methodInfo.Invoke(null, new object[] { p1, p2 });
        }

        public override object Invoke(object target, params object[] args)
        {
            return Invoke((P1)args[0], (P2)args[1]);
        }
    }

    public class StaticProxyFuncInfo<T, P1, P2, P3, TResult> : ProxyMethodInfo, IStaticProxyFuncInfo<T, P1, P2, P3, TResult>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(StaticProxyFuncInfo<T, P1, P2, P3, TResult>));
        private Func<P1, P2, P3, TResult> function;

        public StaticProxyFuncInfo(string methodName) : this(typeof(T).GetMethod(methodName), null)
        {
        }
        public StaticProxyFuncInfo(string methodName, Type[] parameterTypes) : this(typeof(T).GetMethod(methodName, parameterTypes), null)
        {
        }
        public StaticProxyFuncInfo(MethodInfo info) : this(info, null)
        {
        }

        public StaticProxyFuncInfo(string methodName, Func<P1, P2, P3, TResult> function) : this(typeof(T).GetMethod(methodName), function)
        {
        }
        public StaticProxyFuncInfo(string methodName, Type[] parameterTypes, Func<P1, P2, P3, TResult> function) : this(typeof(T).GetMethod(methodName, parameterTypes), function)
        {
        }
        public StaticProxyFuncInfo(MethodInfo info, Func<P1, P2, P3, TResult> function) : base(info)
        {
            if (!this.methodInfo.IsStatic)
                throw new ArgumentException("The method isn't static!");

            if (!typeof(TResult).Equals(this.methodInfo.ReturnType) || !typeof(T).Equals(this.methodInfo.DeclaringType))
                throw new ArgumentException("The method types do not match!");

            ParameterInfo[] parameters = this.methodInfo.GetParameters();
            if (parameters.Length != 3 || !typeof(P1).Equals(parameters[0].ParameterType) || !typeof(P2).Equals(parameters[1].ParameterType) || !typeof(P3).Equals(parameters[2].ParameterType))
                throw new ArgumentException("The method types do not match!");

            this.function = function;
            if (this.function == null)
                this.function = this.MakeFunc(this.methodInfo);
        }

        public override Type DeclaringType { get { return typeof(T); } }

        private Func<P1, P2, P3, TResult> MakeFunc(MethodInfo methodInfo)
        {
            try
            {
#if NETFX_CORE
                return (Func<P1, P2,P3,TResult>)methodInfo.CreateDelegate(typeof(Func<P1,P2,P3, TResult>));
#elif !UNITY_IOS
                return (Func<P1, P2, P3, TResult>)Delegate.CreateDelegate(typeof(Func<P1, P2, P3, TResult>), methodInfo);
#endif
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("{0}", e);
            }
            return null;
        }

        public virtual TResult Invoke(P1 p1, P2 p2, P3 p3)
        {
            if (this.function != null)
                return this.function(p1, p2, p3);

            return (TResult)this.methodInfo.Invoke(null, new object[] { p1, p2, p3 });
        }

        public override object Invoke(object target, params object[] args)
        {
            return Invoke((P1)args[0], (P2)args[1], (P3)args[2]);
        }
    }

    public class StaticProxyActionInfo<T> : ProxyMethodInfo, IStaticProxyActionInfo<T>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(StaticProxyActionInfo<T>));
        private Action action;

        public StaticProxyActionInfo(string methodName) : this(typeof(T).GetMethod(methodName), null)
        {
        }
        public StaticProxyActionInfo(string methodName, Type[] parameterTypes) : this(typeof(T).GetMethod(methodName, parameterTypes), null)
        {
        }
        public StaticProxyActionInfo(MethodInfo info) : this(info, null)
        {
        }

        public StaticProxyActionInfo(string methodName, Action action) : this(typeof(T).GetMethod(methodName), action)
        {
        }
        public StaticProxyActionInfo(string methodName, Type[] parameterTypes, Action action) : this(typeof(T).GetMethod(methodName, parameterTypes), action)
        {
        }

        public StaticProxyActionInfo(MethodInfo info, Action action) : base(info)
        {
            if (!this.methodInfo.IsStatic)
                throw new ArgumentException("The method isn't static!");

            if (!typeof(void).Equals(this.methodInfo.ReturnType) || !typeof(T).Equals(this.methodInfo.DeclaringType))
                throw new ArgumentException("The method types do not match!");

            this.action = action;
            if (this.action == null)
                this.action = this.MakeAction(this.methodInfo);

        }

        public override Type DeclaringType { get { return typeof(T); } }


        private Action MakeAction(MethodInfo methodInfo)
        {
            try
            {
#if NETFX_CORE
                return  (Action)methodInfo.CreateDelegate(typeof(Action));
#elif !UNITY_IOS
                return (Action)Delegate.CreateDelegate(typeof(Action), methodInfo);
#endif
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("{0}", e);
            }
            return null;
        }

        public virtual void Invoke()
        {
            if (this.action != null)
            {
                this.action();
                return;
            }

            this.methodInfo.Invoke(null, null);
        }

        public override object Invoke(object target, params object[] args)
        {
            Invoke();
            return null;
        }
    }

    public class StaticProxyActionInfo<T, P1> : ProxyMethodInfo, IStaticProxyActionInfo<T, P1>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(StaticProxyActionInfo<T, P1>));
        private Action<P1> action;

        public StaticProxyActionInfo(string methodName) : this(typeof(T).GetMethod(methodName), null)
        {
        }
        public StaticProxyActionInfo(string methodName, Type[] parameterTypes) : this(typeof(T).GetMethod(methodName, parameterTypes), null)
        {
        }
        public StaticProxyActionInfo(MethodInfo info) : this(info, null)
        {
        }

        public StaticProxyActionInfo(string methodName, Action<P1> action) : this(typeof(T).GetMethod(methodName), action)
        {
        }
        public StaticProxyActionInfo(string methodName, Type[] parameterTypes, Action<P1> action) : this(typeof(T).GetMethod(methodName, parameterTypes), action)
        {
        }

        public StaticProxyActionInfo(MethodInfo info, Action<P1> action) : base(info)
        {
            if (!this.methodInfo.IsStatic)
                throw new ArgumentException("The method isn't static!");

            if (!typeof(void).Equals(this.methodInfo.ReturnType) || !typeof(T).Equals(this.methodInfo.DeclaringType))
                throw new ArgumentException("The method types do not match!");

            ParameterInfo[] parameters = this.methodInfo.GetParameters();
            if (parameters.Length != 1 || !typeof(P1).Equals(parameters[0].ParameterType))
                throw new ArgumentException("The method types do not match!");

            this.action = action;
            if (this.action == null)
                this.action = this.MakeAction(this.methodInfo);

        }

        public override Type DeclaringType { get { return typeof(T); } }

        private Action<P1> MakeAction(MethodInfo methodInfo)
        {
            try
            {
#if NETFX_CORE
                return  (Action<P1>)methodInfo.CreateDelegate(typeof(Action<P1>));
#elif !UNITY_IOS
                return (Action<P1>)Delegate.CreateDelegate(typeof(Action<P1>), methodInfo);
#endif
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("{0}", e);
            }
            return null;
        }

        public virtual void Invoke(P1 p1)
        {
            if (this.action != null)
            {
                this.action(p1);
                return;
            }

            this.methodInfo.Invoke(null, new object[] { p1 });
        }

        public override object Invoke(object target, params object[] args)
        {
            Invoke((P1)args[0]);
            return null;
        }
    }

    public class StaticProxyActionInfo<T, P1, P2> : ProxyMethodInfo, IStaticProxyActionInfo<T, P1, P2>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(StaticProxyActionInfo<T, P1, P2>));
        private Action<P1, P2> action;
        public StaticProxyActionInfo(string methodName) : this(typeof(T).GetMethod(methodName), null)
        {
        }
        public StaticProxyActionInfo(string methodName, Type[] parameterTypes) : this(typeof(T).GetMethod(methodName, parameterTypes), null)
        {
        }
        public StaticProxyActionInfo(MethodInfo info) : this(info, null)
        {
        }

        public StaticProxyActionInfo(string methodName, Action<P1, P2> action) : this(typeof(T).GetMethod(methodName), action)
        {
        }
        public StaticProxyActionInfo(string methodName, Type[] parameterTypes, Action<P1, P2> action) : this(typeof(T).GetMethod(methodName, parameterTypes), action)
        {
        }
        public StaticProxyActionInfo(MethodInfo info, Action<P1, P2> action) : base(info)
        {
            if (!this.methodInfo.IsStatic)
                throw new ArgumentException("The method isn't static!");

            if (!typeof(void).Equals(this.methodInfo.ReturnType) || !typeof(T).Equals(this.methodInfo.DeclaringType))
                throw new ArgumentException("The method types do not match!");

            ParameterInfo[] parameters = this.methodInfo.GetParameters();
            if (parameters.Length != 2 || !typeof(P1).Equals(parameters[0].ParameterType) || !typeof(P2).Equals(parameters[1].ParameterType))
                throw new ArgumentException("The method types do not match!");

            this.action = action;
            if (this.action == null)
                this.action = this.MakeAction(this.methodInfo);

        }

        public override Type DeclaringType { get { return typeof(T); } }

        private Action<P1, P2> MakeAction(MethodInfo methodInfo)
        {
            try
            {
#if NETFX_CORE
                return  (Action<P1,P2>)methodInfo.CreateDelegate(typeof(Action<P1,P2>));
#elif !UNITY_IOS
                return (Action<P1, P2>)Delegate.CreateDelegate(typeof(Action<P1, P2>), methodInfo);
#endif
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("{0}", e);
            }
            return null;
        }

        public virtual void Invoke(P1 p1, P2 p2)
        {
            if (this.action != null)
            {
                this.action(p1, p2);
                return;
            }

            this.methodInfo.Invoke(null, new object[] { p1, p2 });
        }

        public override object Invoke(object target, params object[] args)
        {
            Invoke((P1)args[0], (P2)args[1]);
            return null;
        }
    }

    public class StaticProxyActionInfo<T, P1, P2, P3> : ProxyMethodInfo, IStaticProxyActionInfo<T, P1, P2, P3>
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(StaticProxyActionInfo<T, P1, P2, P3>));
        private Action<P1, P2, P3> action;

        public StaticProxyActionInfo(string methodName) : this(typeof(T).GetMethod(methodName), null)
        {
        }
        public StaticProxyActionInfo(string methodName, Type[] parameterTypes) : this(typeof(T).GetMethod(methodName, parameterTypes), null)
        {
        }
        public StaticProxyActionInfo(MethodInfo info) : this(info, null)
        {
        }
        public StaticProxyActionInfo(string methodName, Action<P1, P2, P3> action) : this(typeof(T).GetMethod(methodName), action)
        {
        }
        public StaticProxyActionInfo(string methodName, Type[] parameterTypes, Action<P1, P2, P3> action) : this(typeof(T).GetMethod(methodName, parameterTypes), action)
        {
        }
        public StaticProxyActionInfo(MethodInfo info, Action<P1, P2, P3> action) : base(info)
        {
            if (!this.methodInfo.IsStatic)
                throw new ArgumentException("The method isn't static!");

            if (!typeof(void).Equals(this.methodInfo.ReturnType) || !typeof(T).Equals(this.methodInfo.DeclaringType))
                throw new ArgumentException("The method types do not match!");

            ParameterInfo[] parameters = this.methodInfo.GetParameters();
            if (parameters.Length != 3 || !typeof(P1).Equals(parameters[0].ParameterType) || !typeof(P2).Equals(parameters[1].ParameterType) || !typeof(P3).Equals(parameters[2].ParameterType))
                throw new ArgumentException("The method types do not match!");

            this.action = action;
            if (this.action == null)
                this.action = this.MakeAction(this.methodInfo);
        }

        public override Type DeclaringType { get { return typeof(T); } }


        private Action<P1, P2, P3> MakeAction(MethodInfo methodInfo)
        {
            try
            {
#if NETFX_CORE
                return  (Action<P1,P2,P3>)methodInfo.CreateDelegate(typeof(Action<P1,P2,P3>));
#elif !UNITY_IOS
                return (Action<P1, P2, P3>)Delegate.CreateDelegate(typeof(Action<P1, P2, P3>), methodInfo);
#endif
            }
            catch (Exception e)
            {
                if (log.IsWarnEnabled)
                    log.WarnFormat("{0}", e);
            }
            return null;
        }

        public virtual void Invoke(P1 p1, P2 p2, P3 p3)
        {
            if (this.action != null)
            {
                this.action(p1, p2, p3);
                return;
            }

            this.methodInfo.Invoke(null, new object[] { p1, p2, p3 });
        }

        public override object Invoke(object target, params object[] args)
        {
            Invoke((P1)args[0], (P2)args[1], (P3)args[2]);
            return null;
        }
    }
}
