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
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CatLib
{
    /// <summary>
    /// 事件调度器
    /// </summary>
    public class Dispatcher : IDispatcher
    {
        /// <summary>
        /// 调用方法目标 映射到 事件句柄
        /// </summary>
        private readonly Dictionary<object, List<IEvent>> targetMapping;

        /// <summary>
        /// 普通事件列表
        /// </summary>
        private readonly Dictionary<string, List<IEvent>> listeners;

        /// <summary>
        /// 通配符事件列表
        /// </summary>
        private readonly Dictionary<string, KeyValuePair<Regex, List<IEvent>>> wildcardListeners;

        /// <summary>
        /// 依赖注入容器
        /// </summary>
        private readonly IContainer container;

        /// <summary>
        /// 依赖注入容器
        /// </summary>
        protected IContainer Container
        {
            get { return container; }
        }

        /// <summary>
        /// 同步锁
        /// </summary>
        private readonly object syncRoot;

        /// <summary>
        /// 跳出标记
        /// </summary>
        protected virtual object BreakFlag
        {
            get { return false; }
        }

        /// <summary>
        /// 构建一个事件调度器
        /// </summary>
        /// <param name="container">依赖注入容器</param>
        public Dispatcher(IContainer container)
        {
            Guard.Requires<ArgumentNullException>(container != null);

            this.container = container;
            syncRoot = new object();
            targetMapping = new Dictionary<object, List<IEvent>>();
            listeners = new Dictionary<string, List<IEvent>>();
            wildcardListeners = new Dictionary<string, KeyValuePair<Regex, List<IEvent>>>();
        }

        /// <summary>
        /// 判断给定事件是否存在事件监听器
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="strict">
        /// 严格模式
        /// <para>启用严格模式则不使用正则来进行匹配事件监听器</para>
        /// </param>
        /// <returns>是否存在事件监听器</returns>
        public bool HasListeners(string eventName, bool strict = false)
        {
            eventName = FormatEventName(eventName);
            lock (syncRoot)
            {
                if (listeners.ContainsKey(eventName) 
                    || wildcardListeners.ContainsKey(eventName))
                {
                    return true;
                }

                if (strict)
                {
                    return false;
                }

                foreach (var element in wildcardListeners)
                {
                    if (element.Value.Key.IsMatch(eventName))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// 触发一个事件,并获取事件监听器的返回结果
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="payloads">载荷</param>
        /// <returns>事件结果</returns>
        public object[] Trigger(string eventName, params object[] payloads)
        {
            return Dispatch(false, eventName, payloads) as object[];
        }

        /// <summary>
        /// 触发一个事件,并获取事件监听器的返回结果
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="payloads">载荷</param>
        /// <returns>事件结果</returns>
        public object TriggerHalt(string eventName, params object[] payloads)
        {
            return Dispatch(true, eventName, payloads);
        }

        /// <summary>
        /// 注册一个事件监听器
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="target">调用目标</param>
        /// <param name="method">调用方法</param>
        /// <returns>事件对象</returns>
        public IEvent On(string eventName, object target, MethodInfo method)
        {
            Guard.NotEmptyOrNull(eventName, "eventName");
            Guard.Requires<ArgumentNullException>(method != null);
            if (!method.IsStatic)
            {
                Guard.Requires<ArgumentNullException>(target != null);
            }

            lock (syncRoot)
            {
                eventName = FormatEventName(eventName);

                var result = IsWildcard(eventName)
                    ? SetupWildcardListen(eventName, target, method)
                    : SetupListen(eventName, target, method);

                if (target == null)
                {
                    return result;
                }

                List<IEvent> listener;
                if (!targetMapping.TryGetValue(target, out listener))
                {
                    targetMapping[target] = listener = new List<IEvent>();
                }
                listener.Add(result);

                return result;
            }
        }

        /// <summary>
        /// 解除注册的事件监听器
        /// </summary>
        /// <param name="target">
        /// 事件解除目标
        /// <para>如果传入的是字符串(<code>string</code>)将会解除对应事件名的所有事件</para>
        /// <para>如果传入的是事件对象(<code>IEvent</code>)那么解除对应事件</para>
        /// <para>如果传入的是其他实例(<code>object</code>)会解除该实例下的所有事件</para>
        /// </param>
        public void Off(object target)
        {
            Guard.Requires<ArgumentNullException>(target != null);

            lock (syncRoot)
            {
                var baseEvent = target as IEvent;
                if (baseEvent != null)
                {
                    Forget(baseEvent);
                    return;
                }

                if (target is string)
                {
                    var eventName = FormatEventName(target.ToString());
                    if (IsWildcard(eventName))
                    {
                        DismissWildcardEventName(eventName);
                    }
                    else
                    {
                        DismissEventName(eventName);
                    }
                    return;
                }

                DismissTargetObject(target);
            }
        }

        /// <summary>
        /// 生成事件
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="target">目标对象</param>
        /// <param name="method">调用方法</param>
        /// <param name="isWildcard">是否是通配符事件</param>
        protected virtual IEvent MakeEvent(string eventName, object target, MethodInfo method, bool isWildcard = false)
        {
            return new Event(eventName, target, method, MakeListener(target, method, isWildcard));
        }

        /// <summary>
        /// 创建事件监听器
        /// </summary>
        /// <param name="target">调用目标</param>
        /// <param name="method">调用方法</param>
        /// <param name="isWildcard">是否是通配符方法</param>
        /// <returns>事件监听器</returns>
        protected virtual Func<string, object[], object> MakeListener(object target, MethodInfo method, bool isWildcard = false)
        {
            return (eventName, payloads) => Container.Call(target, method, isWildcard
                ? Arr.Merge(new object[] { eventName }, payloads)
                : payloads);
        }

        /// <summary>
        /// 格式化事件名
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <returns>格式化后的事件名</returns>
        protected virtual string FormatEventName(string eventName)
        {
            return eventName;
        }

        /// <summary>
        /// 调度事件
        /// </summary>
        /// <param name="halt">遇到第一个事件存在处理结果后终止</param>
        /// <param name="eventName">事件名</param>
        /// <param name="payload">载荷</param>
        /// <returns>处理结果</returns>
        private object Dispatch(bool halt, string eventName, params object[] payload)
        {
            Guard.Requires<ArgumentNullException>(eventName != null);
            eventName = FormatEventName(eventName);

            lock (syncRoot)
            {
                var outputs = new List<object>(listeners.Count);

                foreach (var listener in GetListeners(eventName))
                {
                    var response = listener.Call(eventName, payload);

                    // 如果启用了事件暂停，且得到的有效的响应那么我们终止事件调用
                    if (halt && response != null)
                    {
                        return response;
                    }

                    // 如果响应内容和终止标记相同那么我们终止事件调用
                    if (response != null && response.Equals(BreakFlag))
                    {
                        break;
                    }

                    outputs.Add(response);
                }

                return halt ? null : outputs.ToArray();
            }
        }

        /// <summary>
        /// 获取指定事件的事件列表
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <returns>事件列表</returns>
        private IEnumerable<IEvent> GetListeners(string eventName)
        {
            var outputs = new List<IEvent>();

            List<IEvent> result;
            if (listeners.TryGetValue(eventName, out result))
            {
                outputs.AddRange(result);
            }

            foreach (var element in wildcardListeners)
            {
                if (element.Value.Key.IsMatch(eventName))
                {
                    outputs.AddRange(element.Value.Value);
                }
            }

            return outputs;
        }

        /// <summary>
        /// 根据普通事件解除相关事件
        /// </summary>
        /// <param name="eventName">事件名</param>
        private void DismissEventName(string eventName)
        {
            List<IEvent> events;
            if (!listeners.TryGetValue(eventName, out events))
            {
                return;
            }

            foreach (var element in events.ToArray())
            {
                Forget(element);
            }
        }

        /// <summary>
        /// 根据通配符事件解除相关事件
        /// </summary>
        /// <param name="eventName">事件名</param>
        private void DismissWildcardEventName(string eventName)
        {
            KeyValuePair<Regex, List<IEvent>> events;
            if (!wildcardListeners.TryGetValue(eventName, out events))
            {
                return;
            }

            foreach (var element in events.Value.ToArray())
            {
                Forget(element);
            }
        }

        /// <summary>
        /// 根据Object解除事件
        /// </summary>
        /// <param name="target">事件解除目标</param>
        private void DismissTargetObject(object target)
        {
            List<IEvent> events;
            if (!targetMapping.TryGetValue(target, out events))
            {
                return;
            }

            foreach (var element in events.ToArray())
            {
                Forget(element);
            }
        }

        /// <summary>
        /// 从事件调度器中移除指定的事件监听器
        /// </summary>
        /// <param name="target">事件监听器</param>
        private void Forget(IEvent target)
        {
            lock (syncRoot)
            {
                List<IEvent> events;
                if (targetMapping.TryGetValue(target.Target, out events))
                {
                    events.Remove(target);
                    if (events.Count <= 0)
                    {
                        targetMapping.Remove(target.Target);
                    }
                }

                if (IsWildcard(target.EventName))
                {
                    ForgetWildcardListen(target);
                }
                else
                {
                    ForgetListen(target);
                }
            }
        }

        /// <summary>
        /// 销毁普通事件
        /// </summary>
        /// <param name="target">事件对象</param>
        private void ForgetListen(IEvent target)
        {
            List<IEvent> events;
            if (!listeners.TryGetValue(target.EventName, out events))
            {
                return;
            }

            events.Remove(target);
            if (events.Count <= 0)
            {
                listeners.Remove(target.EventName);
            }
        }

        /// <summary>
        /// 销毁通配符事件
        /// </summary>
        /// <param name="target">事件对象</param>
        private void ForgetWildcardListen(IEvent target)
        {
            KeyValuePair<Regex, List<IEvent>> wildcardEvents;
            if (!wildcardListeners.TryGetValue(target.EventName, out wildcardEvents))
            {
                return;
            }

            wildcardEvents.Value.Remove(target);
            if (wildcardEvents.Value.Count <= 0)
            {
                wildcardListeners.Remove(target.EventName);
            }
        }

        /// <summary>
        /// 设定普通事件
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="target">事件调用目标</param>
        /// <param name="method">事件调用方法</param>
        /// <returns>监听事件</returns>
        private IEvent SetupListen(string eventName, object target, MethodInfo method)
        {
            List<IEvent> listener;
            if (!listeners.TryGetValue(eventName, out listener))
            {
                listeners[eventName] = listener = new List<IEvent>();
            }

            var output = MakeEvent(eventName, target, method);
            listener.Add(output);
            return output;
        }

        /// <summary>
        /// 设定通配符事件
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="target">事件调用目标</param>
        /// <param name="method">事件调用方法</param>
        /// <returns>监听事件</returns>
        private IEvent SetupWildcardListen(string eventName, object target, MethodInfo method)
        {
            KeyValuePair<Regex, List<IEvent>> listener;
            if (!wildcardListeners.TryGetValue(eventName, out listener))
            {
                wildcardListeners[eventName] = listener =
                    new KeyValuePair<Regex, List<IEvent>>(new Regex(Str.AsteriskWildcard(eventName)), new List<IEvent>());
            }

            var output = MakeEvent(eventName, target, method, true);
            listener.Value.Add(output);
            return output;
        }

        /// <summary>
        /// 是否是通配符事件
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <returns>是否是通配符事件</returns>
        private bool IsWildcard(string eventName)
        {
            return eventName.IndexOf('*') != -1;
        }
    }
}
