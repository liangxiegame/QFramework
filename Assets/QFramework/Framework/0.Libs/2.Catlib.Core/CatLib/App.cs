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

namespace CatLib
{
    /// <summary>
    /// CatLib实例
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract class App
    {
        #region Original
        /// <summary>
        /// 当新建Application时
        /// </summary>
        public static event Action<IApplication> OnNewApplication;

        /// <summary>
        /// CatLib实例
        /// </summary>
        private static IApplication instance;

        /// <summary>
        /// CatLib实例
        /// </summary>
        public static IApplication Handler
        {
            get
            {
                if (instance == null)
                {
                    new Application();
                }
                return instance;
            }
            set
            {
                instance = value;
                if (OnNewApplication != null)
                {
                    OnNewApplication.Invoke(instance);
                }
            }
        }
        #endregion

        #region Application API
        /// <summary>
        /// 终止CatLib框架
        /// </summary>
        public static void Terminate()
        {
            Handler.Terminate();
        }

        /// <summary>
        /// 注册服务提供者
        /// </summary>
        /// <param name="provider">服务提供者</param>
        public static void Register(IServiceProvider provider)
        {
            Handler.Register(provider);
        }

        /// <summary>
        /// 服务提供者是否已经注册过
        /// </summary>
        /// <param name="provider">服务提供者</param>
        /// <returns>服务提供者是否已经注册过</returns>
        public static bool IsRegisted(IServiceProvider provider)
        {
            return Handler.IsRegisted(provider);
        }

        /// <summary>
        /// 获取运行时唯一Id
        /// </summary>
        /// <returns>运行时的唯一Id</returns>
        public static long GetRuntimeId()
        {
            return Handler.GetRuntimeId();
        }

        /// <summary>
        /// 是否是主线程
        /// </summary>
        public static bool IsMainThread
        {
            get
            {
                return Handler.IsMainThread;
            }
        }

        /// <summary>
        /// CatLib版本(遵循semver)
        /// </summary>
        public static string Version
        {
            get
            {
                return Handler.Version;
            }
        }

        /// <summary>
        /// 比较CatLib版本(遵循semver)
        /// <para>输入版本大于当前版本则返回<code>-1</code></para>
        /// <para>输入版本等于当前版本则返回<code>0</code></para>
        /// <para>输入版本小于当前版本则返回<code>1</code></para>
        /// </summary>
        /// <param name="major">主版本号</param>
        /// <param name="minor">次版本号</param>
        /// <param name="revised">修订版本号</param>
        /// <returns>比较结果</returns>
        public static int Compare(int major, int minor, int revised)
        {
            return Handler.Compare(major, minor, revised);
        }

        /// <summary>
        /// 比较CatLib版本(遵循semver)
        /// <para>输入版本大于当前版本则返回<code>-1</code></para>
        /// <para>输入版本等于当前版本则返回<code>0</code></para>
        /// <para>输入版本小于当前版本则返回<code>1</code></para>
        /// </summary>
        /// <param name="version">版本号</param>
        /// <returns>比较结果</returns>
        public static int Compare(string version)
        {
            return Handler.Compare(version);
        }

        /// <summary>
        /// 获取优先级，如果存在方法优先级定义那么优先返回方法的优先级
        /// 如果不存在优先级定义那么返回<c>int.MaxValue</c>
        /// </summary>
        /// <param name="type">获取优先级的类型</param>
        /// <param name="method">获取优先级的调用方法</param>
        /// <returns>优先级</returns>
        public static int GetPriority(Type type, string method = null)
        {
            return Handler.GetPriority(type, method);
        }

        /// <summary>
        /// 调试等级
        /// </summary>
        public static DebugLevels DebugLevel
        {
            get { return Handler.DebugLevel; }
            set { Handler.DebugLevel = value; }
        }
        #endregion

        #region Dispatcher API
        /// <summary>
        /// 判断给定事件是否存在事件监听器
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="strict">
        /// 严格模式
        /// <para>启用严格模式则不使用正则来进行匹配事件监听器</para>
        /// </param>
        /// <returns>是否存在事件监听器</returns>
        public static bool HasListeners(string eventName, bool strict = false)
        {
            return Handler.HasListeners(eventName, strict);
        }

        /// <summary>
        /// 触发一个事件,并获取事件的返回结果
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="payloads">载荷</param>
        /// <returns>事件结果</returns>
        public static object[] Trigger(string eventName, params object[] payloads)
        {
            return Handler.Trigger(eventName, payloads);
        }

        /// <summary>
        /// 触发一个事件,遇到第一个事件存在处理结果后终止,并获取事件的返回结果
        /// </summary>
        /// <param name="eventName">事件名</param>
        /// <param name="payloads">载荷</param>
        /// <returns>事件结果</returns>
        public static object TriggerHalt(string eventName, params object[] payloads)
        {
            return Handler.TriggerHalt(eventName, payloads);
        }

        /// <summary>
        /// 注册一个事件监听器
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="method">事件处理方法</param>
        /// <returns>事件对象</returns>
        public static IEvent On(string eventName, Action method)
        {
            return Handler.On(eventName, method);
        }

        /// <summary>
        /// 注册一个事件监听器
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="target">事件调用目标</param>
        /// <param name="method">事件处理方法</param>
        /// <returns>事件对象</returns>
        public static IEvent On(string eventName, object target, string method = null)
        {
            return Handler.On(eventName, target, method);
        }

        /// <summary>
        /// 注册一个事件监听器
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="method">事件处理方法</param>
        /// <returns>事件对象</returns>
        public static IEvent On<T1>(string eventName, Action<T1> method)
        {
            return Handler.On(eventName, method);
        }

        /// <summary>
        /// 注册一个事件监听器
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="method">事件处理方法</param>
        /// <returns>事件对象</returns>
        public static IEvent On<T1, T2>(string eventName, Action<T1, T2> method)
        {
            return Handler.On(eventName, method);
        }

        /// <summary>
        /// 注册一个事件监听器
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="method">事件处理方法</param>
        /// <returns>事件对象</returns>
        public static IEvent On<T1, T2, T3>(string eventName, Action<T1, T2, T3> method)
        {
            return Handler.On(eventName, method);
        }

        /// <summary>
        /// 注册一个事件监听器
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="method">事件处理方法</param>
        /// <returns>事件对象</returns>
        public static IEvent On<T1, T2, T3, T4>(string eventName, Action<T1, T2, T3, T4> method)
        {
            return Handler.On(eventName, method);
        }

        /// <summary>
        /// 注册一个事件监听器
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="method">事件处理方法</param>
        /// <returns>事件对象</returns>
        public static IEvent Listen<TResult>(string eventName, Func<TResult> method)
        {
            return Handler.Listen(eventName, method);
        }

        /// <summary>
        /// 注册一个事件监听器
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="method">事件处理方法</param>
        /// <returns>事件对象</returns>
        public static IEvent Listen<T1, TResult>(string eventName, Func<T1, TResult> method)
        {
            return Handler.Listen(eventName, method);
        }

        /// <summary>
        /// 注册一个事件监听器
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="method">事件处理方法</param>
        /// <returns>事件对象</returns>
        public static IEvent Listen<T1, T2, TResult>(string eventName, Func<T1, T2, TResult> method)
        {
            return Handler.Listen(eventName, method);
        }

        /// <summary>
        /// 注册一个事件监听器
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="method">事件处理方法</param>
        /// <returns>事件对象</returns>
        public static IEvent Listen<T1, T2, T3, TResult>(string eventName, Func<T1, T2, T3, TResult> method)
        {
            return Handler.Listen(eventName, method);
        }

        /// <summary>
        /// 注册一个事件监听器
        /// </summary>
        /// <param name="eventName">事件名称</param>
        /// <param name="method">事件处理方法</param>
        /// <returns>事件对象</returns>
        public static IEvent Listen<T1, T2, T3, T4, TResult>(string eventName, Func<T1, T2, T3, T4, TResult> method)
        {
            return Handler.Listen(eventName, method);
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
        public static void Off(object target)
        {
            Handler.Off(target);
        }
        #endregion

        #region Container API
        /// <summary>
        /// 获取服务的绑定数据,如果绑定不存在则返回null
        /// </summary>
        /// <param name="service">服务名或者别名</param>
        /// <returns>服务绑定数据或者null</returns>
        public static IBindData GetBind(string service)
        {
            return Handler.GetBind(service);
        }

        /// <summary>
        /// 是否已经绑定了服务
        /// </summary>
        /// <param name="service">服务名或者别名</param>
        /// <returns>返回一个bool值代表服务是否被绑定</returns>
        public static bool HasBind(string service)
        {
            return Handler.HasBind(service);
        }

        /// <summary>
        /// 是否已经实例静态化
        /// </summary>
        /// <typeparam name="TService">服务名</typeparam>
        /// <returns>是否已经静态化</returns>
        public static bool HasInstance<TService>()
        {
#if CATLIB_PERFORMANCE
            return Facade<TService>.HasInstance || Handler.HasInstance<TService>();
#else
            return Handler.HasInstance<TService>();
#endif
        }

        /// <summary>
        /// 服务是否已经被解决过
        /// </summary>
        /// <typeparam name="TService">服务名</typeparam>
        /// <returns>是否已经被解决过</returns>
        public static bool IsResolved<TService>()
        {
            return Handler.IsResolved<TService>();
        }

        /// <summary>
        /// 是否可以生成服务
        /// </summary>
        /// <param name="service">服务名或者别名</param>
        /// <returns>是否可以生成服务</returns>
        public static bool CanMake(string service)
        {
            return Handler.CanMake(service);
        }

        /// <summary>
        /// 服务是否是静态化的,如果服务不存在也将返回false
        /// </summary>
        /// <param name="service">服务名或者别名</param>
        /// <returns>是否是静态化的</returns>
        public static bool IsStatic(string service)
        {
            return Handler.IsStatic(service);
        }

        /// <summary>
        /// 是否是别名
        /// </summary>
        /// <param name="name">名字</param>
        /// <returns>是否是别名</returns>
        public static bool IsAlias(string name)
        {
            return Handler.IsAlias(name);
        }

        /// <summary>
        /// 绑定一个服务
        /// </summary>
        /// <param name="service">服务名</param>
        /// <param name="concrete">服务实现</param>
        /// <param name="isStatic">服务是否静态化</param>
        /// <returns>服务绑定数据</returns>
        public static IBindData Bind(string service, Type concrete, bool isStatic)
        {
            return Handler.Bind(service, concrete, isStatic);
        }

        /// <summary>
        /// 绑定一个服务
        /// </summary>
        /// <param name="service">服务名</param>
        /// <param name="concrete">服务实体</param>
        /// <param name="isStatic">服务是否静态化</param>
        /// <returns>服务绑定数据</returns>
        public static IBindData Bind(string service, Func<IContainer, object[], object> concrete, bool isStatic)
        {
            return Handler.Bind(service, concrete, isStatic);
        }

        /// <summary>
        /// 如果服务不存在那么则绑定服务
        /// </summary>
        /// <param name="service">服务名</param>
        /// <param name="concrete">服务实现</param>
        /// <param name="isStatic">服务是否是静态的</param>
        /// <param name="bindData">如果绑定失败则返回历史绑定对象</param>
        /// <returns>服务绑定数据</returns>
        public static bool BindIf(string service, Func<IContainer, object[], object> concrete, bool isStatic, out IBindData bindData)
        {
            return Handler.BindIf(service, concrete, isStatic, out bindData);
        }

        /// <summary>
        /// 如果服务不存在那么则绑定服务
        /// </summary>
        /// <param name="service">服务名</param>
        /// <param name="concrete">服务实现</param>
        /// <param name="isStatic">服务是否是静态的</param>
        /// <param name="bindData">如果绑定失败则返回历史绑定对象</param>
        /// <returns>服务绑定数据</returns>
        public static bool BindIf(string service, Type concrete, bool isStatic, out IBindData bindData)
        {
            return Handler.BindIf(service, concrete, isStatic, out bindData);
        }

        /// <summary>
        /// 绑定一个方法到容器
        /// </summary>
        /// <param name="method">方法名</param>
        /// <param name="target">调用目标</param>
        /// <param name="call">调用方法</param>
        /// <returns>方法绑定数据</returns>
        public static IMethodBind BindMethod(string method, object target, MethodInfo call)
        {
            return Handler.BindMethod(method, target, call);
        }

        /// <summary>
        /// 解除绑定的方法
        /// </summary>
        /// <param name="target">
        /// 解除目标
        /// <para>如果为字符串则作为调用方法名</para>
        /// <para>如果为<code>IMethodBind</code>则作为指定方法</para>
        /// <para>如果为其他对象则作为调用目标做全体解除</para>
        /// </param>
        public static void UnbindMethod(object target)
        {
            Handler.UnbindMethod(target);
        }

        /// <summary>
        /// 解除绑定服务
        /// </summary>
        /// <param name="service">服务名或者别名</param>
        public static void Unbind(string service)
        {
            Handler.Unbind(service);
        }

        /// <summary>
        /// 为一个及以上的服务定义一个标记
        /// </summary>
        /// <param name="tag">标记名</param>
        /// <param name="service">服务名</param>
        public static void Tag(string tag, params string[] service)
        {
            Handler.Tag(tag, service);
        }

        /// <summary>
        /// 根据标记名生成标记所对应的所有服务实例
        /// </summary>
        /// <param name="tag">标记名</param>
        /// <returns>将会返回标记所对应的所有服务实例</returns>
        public static object[] Tagged(string tag)
        {
            return Handler.Tagged(tag);
        }

        /// <summary>
        /// 静态化一个服务
        /// </summary>
        /// <param name="service">服务名或者别名</param>
        /// <param name="instance">服务实例</param>
        public static object Instance(string service, object instance)
        {
            return Handler.Instance(service, instance);
        }

        /// <summary>
        /// 释放某个静态化实例
        /// </summary>
        /// <param name="service">服务名或别名</param>
        public static bool Release(string service)
        {
            return Handler.Release(service);
        }

        /// <summary>
        /// 调用一个已经被绑定的方法
        /// </summary>
        /// <param name="method">方法名</param>
        /// <param name="userParams">用户提供的参数</param>
        /// <returns>调用结果</returns>
        public static object Invoke(string method, params object[] userParams)
        {
            return Handler.Invoke(method, userParams);
        }

        /// <summary>
        /// 以依赖注入形式调用一个方法
        /// </summary>
        /// <param name="instance">方法对象</param>
        /// <param name="methodInfo">方法信息</param>
        /// <param name="userParams">用户传入的参数</param>
        /// <returns>方法返回值</returns>
        public static object Call(object instance, MethodInfo methodInfo, params object[] userParams)
        {
            return Handler.Call(instance, methodInfo, userParams);
        }

        /// <summary>
        /// 构造服务
        /// </summary>
        /// <param name="service">服务名或别名</param>
        /// <param name="userParams">用户传入的参数</param>
        /// <returns>服务实例，如果构造失败那么返回null</returns>
        public static object Make(string service, params object[] userParams)
        {
            return Handler.Make(service, userParams);
        }

        /// <summary>
        /// 获取一个回调，当执行回调可以生成指定的服务
        /// </summary>
        /// <param name="service">服务名或别名</param>
        /// <returns>回调方案</returns>
        public static Func<object> Factory(string service)
        {
            return Handler.Factory(service);
        }

        /// <summary>
        /// 为服务设定一个别名
        /// </summary>
        /// <param name="alias">别名</param>
        /// <param name="service">映射到的服务名</param>
        /// <returns>当前容器对象</returns>
        public static IContainer Alias(string alias, string service)
        {
            return Handler.Alias(alias, service);
        }

        /// <summary>
        /// 当服务被解决时触发的事件
        /// </summary>
        /// <param name="func">回调函数</param>
        /// <returns>当前容器实例</returns>
        public static IContainer OnResolving(Func<IBindData, object, object> func)
        {
            return Handler.OnResolving(func);
        }

        /// <summary>
        /// 当静态服务被释放时
        /// </summary>
        /// <param name="action">处理释放时的回调</param>
        public static IContainer OnRelease(Action<IBindData, object> action)
        {
            return Handler.OnRelease(action);
        }

        /// <summary>
        /// 当查找类型无法找到时会尝试去调用开发者提供的查找类型函数
        /// </summary>
        /// <param name="func">查找类型的回调</param>
        /// <param name="priority">查询优先级(值越小越优先)</param>
        /// <returns>当前容器实例</returns>
        public static IContainer OnFindType(Func<string, Type> func, int priority = int.MaxValue)
        {
            return Handler.OnFindType(func, priority);
        }

        /// <summary>
        /// 当一个已经被解决的服务，发生重定义时触发
        /// </summary>
        /// <param name="service">服务名</param>
        /// <param name="callback">回调</param>
        /// <returns>服务容器</returns>
        public static IContainer OnRebound(string service, Action<object> callback)
        {
            return Handler.OnRebound(service, callback);
        }

        /// <summary>
        /// 关注指定的服务，当服务触发重定义时调用指定对象的指定方法
        /// <para>调用是以依赖注入的形式进行的</para>
        /// <para>服务的新建（第一次解决服务）操作并不会触发重定义</para>
        /// </summary>
        /// <param name="service">关注的服务名</param>
        /// <param name="target">当服务发生重定义时调用的目标</param>
        /// <param name="methodInfo">方法信息</param>
        public static void Watch(string service, object target, MethodInfo methodInfo)
        {
            Handler.Watch(service, target, methodInfo);
        }

        /// <summary>
        /// 在回调区间内暂时性的静态化服务实例
        /// </summary>
        /// <param name="callback">回调区间</param>
        /// <param name="services">服务映射</param>
        public static void Flash(Action callback, params KeyValuePair<string, object>[] services)
        {
            Handler.Flash(callback, services);
        }

        /// <summary>
        /// 类型转为服务名
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>转换后的服务名</returns>
        public static string Type2Service(Type type)
        {
            return Handler.Type2Service(type);
        }
        #endregion

        #region Container Extend API
        /// <summary>
        /// 获取服务的绑定数据,如果绑定不存在则返回null
        /// </summary>
        /// <typeparam name="TService">服务名</typeparam>
        /// <returns>服务绑定数据或者null</returns>
        public static IBindData GetBind<TService>()
        {
            return Handler.GetBind<TService>();
        }

        /// <summary>
        /// 是否已经绑定了服务
        /// </summary>
        /// <typeparam name="TService">服务名</typeparam>
        /// <returns>代表服务是否被绑定</returns>
        public static bool HasBind<TService>()
        {
            return Handler.HasBind<TService>();
        }

        /// <summary>
        /// 是否可以生成服务
        /// </summary>
        /// <typeparam name="TService">服务名</typeparam>
        /// <returns>服务是否可以被构建</returns>
        public static bool CanMake<TService>()
        {
            return Handler.CanMake<TService>();
        }

        /// <summary>
        /// 服务是否是静态化的,如果服务不存在也将返回false
        /// </summary>
        /// <typeparam name="TService">服务名</typeparam>
        /// <returns>服务是否是静态化的</returns>
        public static bool IsStatic<TService>()
        {
            return Handler.IsStatic<TService>();
        }

        /// <summary>
        /// 是否是别名
        /// </summary>
        /// <typeparam name="TService">服务名</typeparam>
        /// <returns>是否是别名</returns>
        public static bool IsAlias<TService>()
        {
            return Handler.IsAlias<TService>();
        }

        /// <summary>
        /// 常规绑定一个服务
        /// </summary>
        /// <typeparam name="TService">服务名，同时也是服务实现</typeparam>
        /// <returns>服务绑定数据</returns>
        public static IBindData Bind<TService>()
        {
            return Handler.Bind<TService>();
        }

        /// <summary>
        /// 常规绑定一个服务
        /// </summary>
        /// <typeparam name="TService">服务名</typeparam>
        /// <typeparam name="TAlias">服务别名</typeparam>
        /// <returns>服务绑定数据</returns>
        public static IBindData Bind<TService, TAlias>()
        {
            return Handler.Bind<TService, TAlias>();
        }

        /// <summary>
        /// 常规绑定一个服务
        /// </summary>
        /// <typeparam name="TService">服务名</typeparam>
        /// <param name="concrete">服务实现</param>
        /// <returns>服务绑定数据</returns>
        public static IBindData Bind<TService>(Func<IContainer, object[], object> concrete)
        {
            return Handler.Bind<TService>(concrete);
        }

        /// <summary>
        /// 常规绑定一个服务
        /// </summary>
        /// <typeparam name="TService">服务名</typeparam>
        /// <param name="concrete">服务实现</param>
        /// <returns>服务绑定数据</returns>
        public static IBindData Bind<TService>(Func<object> concrete)
        {
            return Handler.Bind<TService>(concrete);
        }

        /// <summary>
        /// 常规绑定一个服务
        /// </summary>
        /// <param name="service">服务名</param>
        /// <param name="concrete">服务实现</param>
        /// <returns>服务绑定数据</returns>
        public static IBindData Bind(string service, Func<IContainer, object[], object> concrete)
        {
            return Handler.Bind(service, concrete);
        }

        /// <summary>
        /// 如果服务不存在那么则绑定服务
        /// </summary>
        /// <typeparam name="TService">服务名</typeparam>
        /// <typeparam name="TAlias">服务别名</typeparam>
        /// <param name="bindData">如果绑定失败则返回历史绑定对象</param>
        /// <returns>是否完成绑定</returns>
        public static bool BindIf<TService, TAlias>(out IBindData bindData)
        {
            return Handler.BindIf<TService, TAlias>(out bindData);
        }

        /// <summary>
        /// 如果服务不存在那么则绑定服务
        /// </summary>
        /// <typeparam name="TService">服务名，同时也是服务实现</typeparam>
        /// <param name="bindData">如果绑定失败则返回历史绑定对象</param>
        /// <returns>是否完成绑定</returns>
        public static bool BindIf<TService>(out IBindData bindData)
        {
            return Handler.BindIf<TService>(out bindData);
        }

        /// <summary>
        /// 如果服务不存在那么则绑定服务
        /// </summary>
        /// <typeparam name="TService">服务名</typeparam>
        /// <param name="concrete">服务实现</param>
        /// <param name="bindData">如果绑定失败则返回历史绑定对象</param>
        /// <returns>是否完成绑定</returns>
        public static bool BindIf<TService>(Func<IContainer, object[], object> concrete, out IBindData bindData)
        {
            return Handler.BindIf<TService>(concrete, out bindData);
        }

        /// <summary>
        /// 如果服务不存在那么则绑定服务
        /// </summary>
        /// <typeparam name="TService">服务名</typeparam>
        /// <param name="concrete">服务实现</param>
        /// <param name="bindData">如果绑定失败则返回历史绑定对象</param>
        /// <returns>是否完成绑定</returns>
        public static bool BindIf<TService>(Func<object> concrete, out IBindData bindData)
        {
            return Handler.BindIf<TService>(concrete, out bindData);
        }

        /// <summary>
        /// 如果服务不存在那么则绑定服务
        /// </summary>
        /// <param name="service">服务名</param>
        /// <param name="concrete">服务实现</param>
        /// <param name="bindData">如果绑定失败则返回历史绑定对象</param>
        /// <returns>是否完成绑定</returns>
        public static bool BindIf(string service, Func<IContainer, object[], object> concrete, out IBindData bindData)
        {
            return Handler.BindIf(service, concrete, out bindData);
        }

        /// <summary>
        /// 以单例的形式绑定一个服务
        /// </summary>
        /// <typeparam name="TService">服务名</typeparam>
        /// <typeparam name="TAlias">服务别名</typeparam>
        /// <returns>服务绑定数据</returns>
        public static IBindData Singleton<TService, TAlias>()
        {
            return Handler.Singleton<TService, TAlias>();
        }

        /// <summary>
        /// 以单例的形式绑定一个服务
        /// </summary>
        /// <typeparam name="TService">服务名，同时也是服务实现</typeparam>
        /// <returns>服务绑定数据</returns>
        public static IBindData Singleton<TService>()
        {
            return Handler.Singleton<TService>();
        }

        /// <summary>
        /// 以单例的形式绑定一个服务
        /// </summary>
        /// <typeparam name="TService">服务名</typeparam>
        /// <param name="concrete">服务实现</param>
        /// <returns>服务绑定数据</returns>
        public static IBindData Singleton<TService>(Func<IContainer, object[], object> concrete)
        {
            return Handler.Singleton<TService>(concrete);
        }

        /// <summary>
        /// 以单例的形式绑定一个服务
        /// </summary>
        /// <typeparam name="TService">服务名</typeparam>
        /// <param name="concrete">服务实现</param>
        /// <returns>服务绑定数据</returns>
        public static IBindData Singleton<TService>(Func<object> concrete)
        {
            return Handler.Singleton<TService>(concrete);
        }

        /// <summary>
        /// 以单例的形式绑定一个服务
        /// </summary>
        /// <param name="service">服务名</param>
        /// <param name="concrete">服务实现</param>
        /// <returns>服务绑定数据</returns>
        public static IBindData Singleton(string service, Func<IContainer, object[], object> concrete)
        {
            return Handler.Singleton(service, concrete);
        }

        /// <summary>
        /// 如果服务不存在那么则绑定服务
        /// </summary>
        /// <typeparam name="TService">服务名</typeparam>
        /// <typeparam name="TAlias">服务别名</typeparam>
        /// <param name="bindData">如果绑定失败则返回历史绑定对象</param>
        /// <returns>是否完成绑定</returns>
        public static bool SingletonIf<TService, TAlias>(out IBindData bindData)
        {
            return Handler.SingletonIf<TService, TAlias>(out bindData);
        }

        /// <summary>
        /// 如果服务不存在那么则绑定服务
        /// </summary>
        /// <typeparam name="TService">服务名，同时也是服务实现</typeparam>
        /// <param name="bindData">如果绑定失败则返回历史绑定对象</param>
        /// <returns>是否完成绑定</returns>
        public static bool SingletonIf<TService>(out IBindData bindData)
        {
            return Handler.SingletonIf<TService>(out bindData);
        }

        /// <summary>
        /// 如果服务不存在那么则绑定服务
        /// </summary>
        /// <typeparam name="TService">服务名</typeparam>
        /// <param name="concrete">服务实现</param>
        /// <param name="bindData">如果绑定失败则返回历史绑定对象</param>
        /// <returns>是否完成绑定</returns>
        public static bool SingletonIf<TService>(Func<IContainer, object[], object> concrete, out IBindData bindData)
        {
            return Handler.SingletonIf<TService>(concrete, out bindData);
        }

        /// <summary>
        /// 如果服务不存在那么则绑定服务
        /// </summary>
        /// <typeparam name="TService">服务名</typeparam>
        /// <param name="concrete">服务实现</param>
        /// <param name="bindData">如果绑定失败则返回历史绑定对象</param>
        /// <returns>是否完成绑定</returns>
        public static bool SingletonIf<TService>(Func<object> concrete, out IBindData bindData)
        {
            return Handler.SingletonIf<TService>(concrete, out bindData);
        }

        /// <summary>
        /// 如果服务不存在那么则绑定服务
        /// </summary>
        /// <param name="service">服务名</param>
        /// <param name="concrete">服务实现</param>
        /// <param name="bindData">如果绑定失败则返回历史绑定对象</param>
        /// <returns>是否完成绑定</returns>
        public static bool SingletonIf(string service, Func<IContainer, object[], object> concrete, out IBindData bindData)
        {
            return Handler.SingletonIf(service, concrete, out bindData);
        }

        /// <summary>
        /// 绑定一个方法到容器
        /// </summary>
        /// <param name="method">方法名</param>
        /// <param name="target">调用目标</param>
        /// <param name="call">调用方法</param>
        public static IMethodBind BindMethod(string method, object target,
            string call = null)
        {
            return Handler.BindMethod(method, target, call);
        }

        /// <summary>
        /// 绑定一个方法到容器
        /// </summary>
        /// <param name="method">方法名</param>
        /// <param name="callback">调用方法</param>
        public static IMethodBind BindMethod(string method, Func<object> callback)
        {
            return Handler.BindMethod(method, callback);
        }

        /// <summary>
        /// 绑定一个方法到容器
        /// </summary>
        /// <param name="method">方法名</param>
        /// <param name="callback">调用方法</param>
        public static IMethodBind BindMethod<T1>(string method, Func<T1, object> callback)
        {
            return Handler.BindMethod(method, callback);
        }

        /// <summary>
        /// 绑定一个方法到容器
        /// </summary>
        /// <param name="method">方法名</param>
        /// <param name="callback">调用方法</param>
        public static IMethodBind BindMethod<T1, T2>(string method, Func<T1, T2, object> callback)
        {
            return Handler.BindMethod(method, callback);
        }

        /// <summary>
        /// 绑定一个方法到容器
        /// </summary>
        /// <param name="method">方法名</param>
        /// <param name="callback">调用方法</param>
        public static IMethodBind BindMethod<T1, T2, T3>(string method, Func<T1, T2, T3, object> callback)
        {
            return Handler.BindMethod(method, callback);
        }

        /// <summary>
        /// 绑定一个方法到容器
        /// </summary>
        /// <param name="method">方法名</param>
        /// <param name="callback">调用方法</param>
        public static IMethodBind BindMethod<T1, T2, T3, T4>(string method, Func<T1, T2, T3, T4, object> callback)
        {
            return Handler.BindMethod(method, callback);
        }

        /// <summary>
        /// 解除服务绑定
        /// </summary>
        /// <typeparam name="TService">解除绑定的服务</typeparam>
        public static void Unbind<TService>()
        {
            Handler.Unbind<TService>();
        }

        /// <summary>
        /// 为一个服务定义一个标记
        /// </summary>
        /// <typeparam name="TService">服务</typeparam>
        /// <param name="tag">标记名</param>
        public static void Tag<TService>(string tag)
        {
            Handler.Tag<TService>(tag);
        }

        /// <summary>
        /// 静态化一个服务,实例值会经过解决修饰器
        /// </summary>
        /// <typeparam name="TService">服务名</typeparam>
        /// <param name="instance">实例值</param>
        public static void Instance<TService>(object instance)
        {
            Handler.Instance<TService>(instance);
        }

        /// <summary>
        /// 释放服务
        /// </summary>
        /// <typeparam name="TService">服务名</typeparam>
        public static bool Release<TService>()
        {
            return Handler.Release<TService>();
        }

        /// <summary>
        /// 根据实例对象释放静态化实例
        /// </summary>
        /// <param name="instances">需要释放静态化实例对象</param>
        /// <returns>只要有一个没有释放成功那么返回false</returns>
        public static bool Release(params object[] instances)
        {
            return Handler.Release(instances);
        }

        /// <summary>
        /// 以依赖注入形式调用一个方法
        /// </summary>
        /// <param name="instance">方法对象</param>
        /// <param name="method">方法名</param>
        /// <param name="userParams">用户传入的参数</param>
        /// <returns>方法返回值</returns>
        public static object Call(object instance, string method, params object[] userParams)
        {
            return Handler.Call(instance, method, userParams);
        }

        /// <summary>
        /// 以依赖注入的形式调用一个方法
        /// </summary>
        /// <param name="method">方法</param>
        /// <param name="userParams">用户传入的参数</param>
        public static void Call<T1>(Action<T1> method, params object[] userParams)
        {
            Handler.Call(method, userParams);
        }

        /// <summary>
        /// 以依赖注入的形式调用一个方法
        /// </summary>
        /// <param name="method">方法</param>
        /// <param name="userParams">用户传入的参数</param>
        public static void Call<T1, T2>(Action<T1, T2> method, params object[] userParams)
        {
            Handler.Call(method, userParams);
        }

        /// <summary>
        /// 以依赖注入的形式调用一个方法
        /// </summary>
        /// <param name="method">方法</param>
        /// <param name="userParams">用户传入的参数</param>
        public static void Call<T1, T2, T3>(Action<T1, T2, T3> method, params object[] userParams)
        {
            Handler.Call(method, userParams);
        }

        /// <summary>
        /// 以依赖注入的形式调用一个方法
        /// </summary>
        /// <param name="method">方法</param>
        /// <param name="userParams">用户传入的参数</param>
        public static void Call<T1, T2, T3, T4>(Action<T1, T2, T3, T4> method, params object[] userParams)
        {
            Handler.Call(method, userParams);
        }

        /// <summary>
        /// 包装一个依赖注入形式调用的一个方法
        /// </summary>
        /// <param name="method">方法</param>
        /// <param name="userParams">用户传入的参数</param>
        /// <returns>包装方法</returns>
        public static Action Wrap<T1>(Action<T1> method, params object[] userParams)
        {
            return Handler.Wrap(method, userParams);
        }

        /// <summary>
        /// 包装一个依赖注入形式调用的一个方法
        /// </summary>
        /// <param name="method">方法</param>
        /// <param name="userParams">用户传入的参数</param>
        /// <returns>包装方法</returns>
        public static Action Wrap<T1, T2>(Action<T1, T2> method, params object[] userParams)
        {
            return Handler.Wrap(method, userParams);
        }

        /// <summary>
        /// 包装一个依赖注入形式调用的一个方法
        /// </summary>
        /// <param name="method">方法</param>
        /// <param name="userParams">用户传入的参数</param>
        /// <returns>包装方法</returns>
        public static Action Wrap<T1, T2, T3>(Action<T1, T2, T3> method, params object[] userParams)
        {
            return Handler.Wrap(method, userParams);
        }

        /// <summary>
        /// 包装一个依赖注入形式调用的一个方法
        /// </summary>
        /// <param name="method">方法</param>
        /// <param name="userParams">用户传入的参数</param>
        /// <returns>包装方法</returns>
        public static Action Wrap<T1, T2, T3, T4>(Action<T1, T2, T3, T4> method, params object[] userParams)
        {
            return Handler.Wrap(method, userParams);
        }

        /// <summary>
        /// 构造一个服务
        /// </summary>
        /// <typeparam name="TService">服务名</typeparam>
        /// <param name="userParams">用户参数</param>
        /// <returns>服务实例</returns>
        public static TService Make<TService>(params object[] userParams)
        {
#if CATLIB_PERFORMANCE
            return Facade<TService>.Make(userParams);
#else
            return Handler.Make<TService>(userParams);
#endif
        }

        /// <summary>
        /// 构造一个服务
        /// </summary>
        /// <param name="type">服务类型</param>
        /// <param name="userParams">用户提供的参数</param>
        /// <returns>服务实例</returns>
        public static object Make(Type type, params object[] userParams)
        {
            return Handler.Make(type, userParams);
        }

        /// <summary>
        /// 获取一个回调，当执行回调可以生成指定的服务
        /// </summary>
        /// <typeparam name="TService">服务名</typeparam>
        /// <returns>回调方案</returns>
        public static Func<TService> Factory<TService>(params object[] userParams)
        {
            return () => Make<TService>(userParams);
        }

        /// <summary>
        /// 当静态服务被释放时
        /// </summary>
        /// <param name="callback">处理释放时的回调</param>
        /// <returns>当前容器实例</returns>
        public static IContainer OnRelease(Action<object> callback)
        {
            return Handler.OnRelease(callback);
        }

        /// <summary>
        /// 当服务被解决时，生成的服务会经过注册的回调函数
        /// </summary>
        /// <param name="callback">回调函数</param>
        /// <returns>当前容器对象</returns>
        public static IContainer OnResolving(Action<object> callback)
        {
            return Handler.OnResolving(callback);
        }

        /// <summary>
        /// 关注指定的服务，当服务触发重定义时调用指定对象的指定方法
        /// <param>调用是以依赖注入的形式进行的</param>
        /// </summary>
        /// <param name="service">关注的服务名</param>
        /// <param name="target">当服务发生重定义时调用的目标</param>
        /// <param name="method">方法名</param>
        public static void Watch(string service, object target, string method)
        {
            Handler.Watch(service, target, method);
        }

        /// <summary>
        /// 关注指定的服务，当服务触发重定义时调用指定对象的指定方法
        /// <param>调用是以依赖注入的形式进行的</param>
        /// </summary>
        /// <typeparam name="TService">服务名</typeparam>
        /// <param name="target">当服务发生重定义时调用的目标</param>
        /// <param name="method">方法名</param>
        public static void Watch<TService>(object target, string method)
        {
            Handler.Watch<TService>(target, method);
        }

        /// <summary>
        /// 关注指定的服务，当服务触发重定义时调用指定对象的指定方法
        /// <param>调用是以依赖注入的形式进行的</param>
        /// </summary>
        /// <typeparam name="TService">服务名</typeparam>
        /// <param name="method">回调</param>
        public static void Watch<TService>(Action method)
        {
            Handler.Watch<TService>(method);
        }

        /// <summary>
        /// 关注指定的服务，当服务触发重定义时调用指定对象的指定方法
        /// <param>调用是以依赖注入的形式进行的</param>
        /// </summary>
        /// <typeparam name="TService">服务名</typeparam>
        /// <param name="method">回调</param>
        public static void Watch<TService>(Action<TService> method)
        {
            Handler.Watch(method);
        }

        /// <summary>
        /// 在回调区间内暂时性的静态化服务实例
        /// </summary>
        /// <param name="callback">回调区间</param>
        /// <param name="service">服务名</param>
        /// <param name="instance">实例名</param>
        public static void Flash(Action callback, string service, object instance)
        {
            Handler.Flash(callback, service, instance);
        }

        /// <summary>
        /// 类型转为服务名
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <returns>服务名</returns>
        public static string Type2Service<TService>()
        {
            return Handler.Type2Service<TService>();
        }
        #endregion
    }
}