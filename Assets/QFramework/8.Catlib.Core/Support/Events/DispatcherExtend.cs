/*
 * This file is part of the CatLib package.
 *
 * (c) Yu Bin <support@catlib.io>
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 *
 * Document: http://catlib.io/
 */

using System;

namespace CatLib
{
    /// <summary>
    /// 事件调度器扩展方法
    /// </summary>
    public static class DispatcherExtend
    {
        /// <summary>
        /// 注册一个事件监听器
        /// </summary>
        /// <param name="dispatcher">事件调度器</param>
        /// <param name="eventName">事件名称</param>
        /// <param name="target">事件调用目标</param>
        /// <param name="method">事件处理方法</param>
        /// <returns>事件对象</returns>
        public static IEvent On(this IDispatcher dispatcher, string eventName, object target, string method = null)
        {
            Guard.NotEmptyOrNull(eventName, "eventName");
            Guard.Requires<ArgumentException>(method != string.Empty);
            Guard.Requires<ArgumentNullException>(target != null);

            return dispatcher.On(eventName, target, target.GetType().GetMethod(method ?? Str.Method(eventName)));
        }

        /// <summary>
        /// 注册一个事件监听器
        /// </summary>
        /// <param name="dispatcher">事件调度器</param>
        /// <param name="eventName">事件名称</param>
        /// <param name="method">事件处理方法</param>
        /// <returns>事件对象</returns>
        public static IEvent On(this IDispatcher dispatcher, string eventName, Action method)
        {
            Guard.Requires<ArgumentNullException>(method != null);
            return dispatcher.On(eventName, method.Target, method.Method);
        }

        /// <summary>
        /// 注册一个事件监听器
        /// </summary>
        /// <param name="dispatcher">事件调度器</param>
        /// <param name="eventName">事件名称</param>
        /// <param name="method">事件处理方法</param>
        /// <returns>事件对象</returns>
        public static IEvent On<T1>(this IDispatcher dispatcher, string eventName, Action<T1> method)
        {
            Guard.Requires<ArgumentNullException>(method != null);
            return dispatcher.On(eventName, method.Target, method.Method);
        }

        /// <summary>
        /// 注册一个事件监听器
        /// </summary>
        /// <param name="dispatcher">事件调度器</param>
        /// <param name="eventName">事件名称</param>
        /// <param name="method">事件处理方法</param>
        /// <returns>事件对象</returns>
        public static IEvent On<T1, T2>(this IDispatcher dispatcher, string eventName, Action<T1, T2> method)
        {
            Guard.Requires<ArgumentNullException>(method != null);
            return dispatcher.On(eventName, method.Target, method.Method);
        }

        /// <summary>
        /// 注册一个事件监听器
        /// </summary>
        /// <param name="dispatcher">事件调度器</param>
        /// <param name="eventName">事件名称</param>
        /// <param name="method">事件处理方法</param>
        /// <returns>事件对象</returns>
        public static IEvent On<T1, T2, T3>(this IDispatcher dispatcher, string eventName, Action<T1, T2, T3> method)
        {
            Guard.Requires<ArgumentNullException>(method != null);
            return dispatcher.On(eventName, method.Target, method.Method);
        }

        /// <summary>
        /// 注册一个事件监听器
        /// </summary>
        /// <param name="dispatcher">事件调度器</param>
        /// <param name="eventName">事件名称</param>
        /// <param name="method">事件处理方法</param>
        /// <returns>事件对象</returns>
        public static IEvent On<T1, T2, T3, T4>(this IDispatcher dispatcher, string eventName, Action<T1, T2, T3, T4> method)
        {
            Guard.Requires<ArgumentNullException>(method != null);
            return dispatcher.On(eventName, method.Target, method.Method);
        }

        /// <summary>
        /// 注册一个事件监听器
        /// </summary>
        /// <param name="dispatcher">事件调度器</param>
        /// <param name="eventName">事件名称</param>
        /// <param name="method">事件处理方法</param>
        /// <returns>事件对象</returns>
        public static IEvent Listen<TResult>(this IDispatcher dispatcher, string eventName, Func<TResult> method)
        {
            Guard.Requires<ArgumentNullException>(method != null);
            return dispatcher.On(eventName, method.Target, method.Method);
        }

        /// <summary>
        /// 注册一个事件监听器
        /// </summary>
        /// <param name="dispatcher">事件调度器</param>
        /// <param name="eventName">事件名称</param>
        /// <param name="method">事件处理方法</param>
        /// <returns>事件对象</returns>
        public static IEvent Listen<T1, TResult>(this IDispatcher dispatcher, string eventName, Func<T1, TResult> method)
        {
            Guard.Requires<ArgumentNullException>(method != null);
            return dispatcher.On(eventName, method.Target, method.Method);
        }

        /// <summary>
        /// 注册一个事件监听器
        /// </summary>
        /// <param name="dispatcher">事件调度器</param>
        /// <param name="eventName">事件名称</param>
        /// <param name="method">事件处理方法</param>
        /// <returns>事件对象</returns>
        public static IEvent Listen<T1, T2, TResult>(this IDispatcher dispatcher, string eventName, Func<T1, T2, TResult> method)
        {
            Guard.Requires<ArgumentNullException>(method != null);
            return dispatcher.On(eventName, method.Target, method.Method);
        }

        /// <summary>
        /// 注册一个事件监听器
        /// </summary>
        /// <param name="dispatcher">事件调度器</param>
        /// <param name="eventName">事件名称</param>
        /// <param name="method">事件处理方法</param>
        /// <returns>事件对象</returns>
        public static IEvent Listen<T1, T2, T3, TResult>(this IDispatcher dispatcher, string eventName, Func<T1, T2, T3, TResult> method)
        {
            Guard.Requires<ArgumentNullException>(method != null);
            return dispatcher.On(eventName, method.Target, method.Method);
        }

        /// <summary>
        /// 注册一个事件监听器
        /// </summary>
        /// <param name="dispatcher">事件调度器</param>
        /// <param name="eventName">事件名称</param>
        /// <param name="method">事件处理方法</param>
        /// <returns>事件对象</returns>
        public static IEvent Listen<T1, T2, T3, T4, TResult>(this IDispatcher dispatcher, string eventName, Func<T1, T2, T3, T4, TResult> method)
        {
            Guard.Requires<ArgumentNullException>(method != null);
            return dispatcher.On(eventName, method.Target, method.Method);
        }
    }
}
