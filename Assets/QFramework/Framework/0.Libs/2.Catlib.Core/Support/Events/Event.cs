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
    /// 事件对象
    /// </summary>
    internal class Event : IEvent
    {
        /// <summary>
        /// 事件名
        /// </summary>
        public string EventName { get; private set; }

        /// <summary>
        /// 事件目标
        /// </summary>
        public object Target { get; private set; }

        /// <summary>
        /// 方法信息
        /// </summary>
        public MethodInfo Method { get; private set; }

        /// <summary>
        /// 事件调用
        /// </summary>
        private readonly Func<string, object[], object> transfer;

        /// <summary>
        /// 创建一个事件对象
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="target">调用方法目标</param>
        /// <param name="method">目标方法</param>
        /// <param name="transfer">调用方法</param>
        public Event(string eventName, object target, MethodInfo method, Func<string, object[], object> transfer)
        {
            EventName = eventName;
            Target = target;
            Method = method;
            this.transfer = transfer;
        }

        /// <summary>
        /// 调用事件
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="payloads">载荷</param>
        public object Call(string eventName, params object[] payloads)
        {
            return transfer(eventName, payloads);
        }
    }
}
