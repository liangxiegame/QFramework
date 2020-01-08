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
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BindKit
{
    public static class WeakDelegateExtensions
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WeakDelegateExtensions));

        public static Action AsWeak(this Action action)
        {
            if (!IsCanWeaken(action))
                return action;

            Type type = action.Target.GetType();
            WeakReference targetRef = new WeakReference(action.Target);

#if NETFX_CORE
            MethodInfo method = action.GetMethodInfo();
#else
            MethodInfo method = action.Method;
#endif
            return () =>
            {
                object target = targetRef.Target;
                if (target == null)
                {
                    if (log.IsWarnEnabled)
                        log.WarnFormat("You are trying to invoke a weak reference delegate({0}.{1}), and the target object has been destroyed.", type, method);
                    return;
                }

                method.Invoke(target, null);
            };
        }

        public static Action<T> AsWeak<T>(this Action<T> action)
        {
            if (!IsCanWeaken(action))
                return action;

            Type type = action.Target.GetType();
            WeakReference targetRef = new WeakReference(action.Target);
#if NETFX_CORE
            MethodInfo method = action.GetMethodInfo();
#else
            MethodInfo method = action.Method;
#endif
            return (t) =>
            {
                object target = targetRef.Target;
                if (target == null)
                {
                    if (log.IsWarnEnabled)
                        log.WarnFormat("You are trying to invoke a weak reference delegate({0}.{1}), and the target object has been destroyed.", type, method);
                    return;
                }

                method.Invoke(target, new object[] { t });
            };
        }

        public static Action<T1, T2> AsWeak<T1, T2>(this Action<T1, T2> action)
        {
            if (!IsCanWeaken(action))
                return action;

            Type type = action.Target.GetType();
            WeakReference targetRef = new WeakReference(action.Target);
#if NETFX_CORE
            MethodInfo method = action.GetMethodInfo();
#else
            MethodInfo method = action.Method;
#endif
            return (t1, t2) =>
            {
                object target = targetRef.Target;
                if (target == null)
                {
                    if (log.IsWarnEnabled)
                        log.WarnFormat("You are trying to invoke a weak reference delegate({0}.{1}), and the target object has been destroyed.", type, method);
                    return;
                }

                method.Invoke(target, new object[] { t1, t2 });
            };
        }

        public static Action<T1, T2, T3> AsWeak<T1, T2, T3>(this Action<T1, T2, T3> action)
        {
            if (!IsCanWeaken(action))
                return action;

            Type type = action.Target.GetType();
            WeakReference targetRef = new WeakReference(action.Target);
#if NETFX_CORE
            MethodInfo method = action.GetMethodInfo();
#else
            MethodInfo method = action.Method;
#endif
            return (t1, t2, t3) =>
            {
                object target = targetRef.Target;
                if (target == null)
                {
                    if (log.IsWarnEnabled)
                        log.WarnFormat("You are trying to invoke a weak reference delegate({0}.{1}), and the target object has been destroyed.", type, method);
                    return;
                }

                method.Invoke(target, new object[] { t1, t2, t3 });
            };
        }

        public static Action<T1, T2, T3, T4> AsWeak<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> action)
        {
            if (!IsCanWeaken(action))
                return action;

            Type type = action.Target.GetType();
            WeakReference targetRef = new WeakReference(action.Target);
#if NETFX_CORE
            MethodInfo method = action.GetMethodInfo();
#else
            MethodInfo method = action.Method;
#endif
            return (t1, t2, t3, t4) =>
            {
                object target = targetRef.Target;
                if (target == null)
                {
                    if (log.IsWarnEnabled)
                        log.WarnFormat("You are trying to invoke a weak reference delegate({0}.{1}), and the target object has been destroyed.", type, method);
                    return;
                }

                method.Invoke(target, new object[] { t1, t2, t3, t4 });
            };
        }

        public static Func<TResult> AsWeak<TResult>(this Func<TResult> func)
        {
            if (!IsCanWeaken(func))
                return func;

            Type type = func.Target.GetType();
            WeakReference targetRef = new WeakReference(func.Target);
#if NETFX_CORE
            MethodInfo method = func.GetMethodInfo();
#else
            MethodInfo method = func.Method;
#endif
            return () =>
            {
                object target = targetRef.Target;
                if (target == null)
                {
                    if (log.IsWarnEnabled)
                        log.WarnFormat("You are trying to invoke a weak reference delegate({0}.{1}), and the target object has been destroyed.", type, method);

                    throw new Exception(string.Format("You are trying to invoke a weak reference delegate({0}.{1}), and the target object has been destroyed.", type, method));
                }

                return (TResult)method.Invoke(target, null);
            };
        }

        public static Func<T, TResult> AsWeak<T, TResult>(this Func<T, TResult> func)
        {
            if (!IsCanWeaken(func))
                return func;

            Type type = func.Target.GetType();
            WeakReference targetRef = new WeakReference(func.Target);
#if NETFX_CORE
            MethodInfo method = func.GetMethodInfo();
#else
            MethodInfo method = func.Method;
#endif
            return (t) =>
            {
                object target = targetRef.Target;
                if (target == null)
                {
                    if (log.IsWarnEnabled)
                        log.WarnFormat("You are trying to invoke a weak reference delegate({0}.{1}), and the target object has been destroyed.", type, method);

                    throw new Exception(string.Format("You are trying to invoke a weak reference delegate({0}.{1}), and the target object has been destroyed.", type, method));
                }

                return (TResult)method.Invoke(target, new object[] { t });
            };
        }

        public static Func<T1, T2, TResult> AsWeak<T1, T2, TResult>(this Func<T1, T2, TResult> func)
        {
            if (!IsCanWeaken(func))
                return func;

            Type type = func.Target.GetType();
            WeakReference targetRef = new WeakReference(func.Target);
#if NETFX_CORE
            MethodInfo method = func.GetMethodInfo();
#else
            MethodInfo method = func.Method;
#endif
            return (t1, t2) =>
            {
                object target = targetRef.Target;
                if (target == null)
                {
                    if (log.IsWarnEnabled)
                        log.WarnFormat("You are trying to invoke a weak reference delegate({0}.{1}), and the target object has been destroyed.", type, method);

                    throw new Exception(string.Format("You are trying to invoke a weak reference delegate({0}.{1}), and the target object has been destroyed.", type, method));
                }

                return (TResult)method.Invoke(target, new object[] { t1, t2 });
            };
        }


        public static Func<T1, T2, T3, TResult> AsWeak<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> func)
        {
            if (!IsCanWeaken(func))
                return func;

            Type type = func.Target.GetType();
            WeakReference targetRef = new WeakReference(func.Target);
#if NETFX_CORE
            MethodInfo method = func.GetMethodInfo();
#else
            MethodInfo method = func.Method;
#endif
            return (t1, t2, t3) =>
            {
                object target = targetRef.Target;
                if (target == null)
                {
                    if (log.IsWarnEnabled)
                        log.WarnFormat("You are trying to invoke a weak reference delegate({0}.{1}), and the target object has been destroyed.", type, method);

                    throw new Exception(string.Format("You are trying to invoke a weak reference delegate({0}.{1}), and the target object has been destroyed.", type, method));
                }

                return (TResult)method.Invoke(target, new object[] { t1, t2, t3 });
            };
        }

        public static Func<T1, T2, T3, T4, TResult> AsWeak<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> func)
        {
            if (!IsCanWeaken(func))
                return func;

            Type type = func.Target.GetType();
            WeakReference targetRef = new WeakReference(func.Target);
#if NETFX_CORE
            MethodInfo method = func.GetMethodInfo();
#else
            MethodInfo method = func.Method;
#endif
            return (t1, t2, t3, t4) =>
            {
                object target = targetRef.Target;
                if (target == null)
                {
                    if (log.IsWarnEnabled)
                        log.WarnFormat("You are trying to invoke a weak reference delegate({0}.{1}), and the target object has been destroyed.", type, method);

                    throw new Exception(string.Format("You are trying to invoke a weak reference delegate({0}.{1}), and the target object has been destroyed.", type, method));
                }

                return (TResult)method.Invoke(target, new object[] { t1, t2, t3, t4 });
            };
        }

        private static bool IsCanWeaken(Delegate del)
        {
#if NETFX_CORE
            if (del == null || del.GetMethodInfo().IsStatic || del.Target == null || IsClosure(del))
#else
            if (del == null || del.Method.IsStatic || del.Target == null || IsClosure(del))
#endif
                return false;

            return true;
        }

        private static bool IsClosure(Delegate del)
        {
#if NETFX_CORE
            if (del == null || del.GetMethodInfo().IsStatic || del.Target == null)
                return false;

            var type = del.Target.GetType();
            var isCompilerGenerated = type.GetTypeInfo().GetCustomAttribute(typeof(CompilerGeneratedAttribute), false) != null;
            var isNested = type.IsNested;
            return isNested && isCompilerGenerated;
#else
            if (del == null || del.Method.IsStatic || del.Target == null)
                return false;

            var type = del.Target.GetType();
            var isInvisible = !type.IsVisible;
            var isCompilerGenerated = type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Length != 0;
            var isNested = type.IsNested && type.MemberType == MemberTypes.NestedType;
            return isNested && isCompilerGenerated && isInvisible;
#endif
        }

    }
}
