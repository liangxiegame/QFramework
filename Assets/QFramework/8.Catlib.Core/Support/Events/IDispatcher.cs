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
using System.Reflection;

namespace CatLib
{
    /// <summary>
    /// 事件调度器
    /// </summary>
    public interface IDispatcher
    {
        /// <summary>
        /// 判断给定事件是否存在事件监听器
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="strict">
        /// 严格模式
        /// <para>启用严格模式则不使用正则来进行匹配事件监听器</para>
        /// </param>
        /// <returns>是否存在事件监听器</returns>
        bool HasListeners(string eventName, bool strict = false);

        /// <summary>
        /// 触发一个事件,并获取事件监听器的返回结果
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="payloads">载荷</param>
        /// <returns>事件结果</returns>
        object[] Trigger(string eventName, params object[] payloads);

        /// <summary>
        /// 触发一个事件,遇到第一个事件存在处理结果后终止,并获取事件监听器的返回结果
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="payloads">载荷</param>
        /// <returns>事件结果</returns>
        object TriggerHalt(string eventName, params object[] payloads);

        /// <summary>
        /// 注册一个事件监听器
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="target">调用目标</param>
        /// <param name="method">调用方法</param>
        /// <returns>事件对象</returns>
        IEvent On(string eventName, object target, MethodInfo method);

        /// <summary>
        /// 解除注册的事件监听器
        /// </summary>
        /// <param name="target">
        /// 事件解除目标
        /// <para>如果传入的是字符串(<code>string</code>)将会解除对应事件名的所有事件</para>
        /// <para>如果传入的是事件对象(<code>IEvent</code>)那么解除对应事件</para>
        /// <para>如果传入的是其他实例(<code>object</code>)会解除该实例下的所有事件</para>
        /// </param>
        void Off(object target);
    }
}
