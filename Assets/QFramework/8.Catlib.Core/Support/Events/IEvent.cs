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

using System.Reflection;

namespace CatLib
{
    /// <summary>
    /// 事件对象
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// 事件名
        /// </summary>
        string EventName { get; }

        /// <summary>
        /// 事件目标
        /// </summary>
        object Target { get; }

        /// <summary>
        /// 方法信息
        /// </summary>
        MethodInfo Method { get; }

        /// <summary>
        /// 调用事件
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="payloads">载荷</param>
        /// <returns>事件结果</returns>
        object Call(string eventName, params object[] payloads);
    }
}
