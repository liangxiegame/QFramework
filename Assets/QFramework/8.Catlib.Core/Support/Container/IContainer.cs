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
    /// 依赖注入容器
    /// </summary>
    public interface IContainer
    {
        /// <summary>
        /// 获取服务的绑定数据,如果绑定不存在则返回null
        /// </summary>
        /// <param name="service">服务名或者别名</param>
        /// <returns>服务绑定数据或者null</returns>
        IBindData GetBind(string service);

        /// <summary>
        /// 是否已经绑定了服务
        /// </summary>
        /// <param name="service">服务名或者别名</param>
        /// <returns>返回一个bool值代表服务是否被绑定</returns>
        bool HasBind(string service);

        /// <summary>
        /// 是否已经实例静态化
        /// </summary>
        /// <param name="service">服务名或别名</param>
        /// <returns>是否已经静态化</returns>
        bool HasInstance(string service);

        /// <summary>
        /// 服务是否已经被解决过
        /// </summary>
        /// <param name="service">服务名或别名</param>
        /// <returns>是否已经被解决过</returns>
        bool IsResolved(string service);

        /// <summary>
        /// 是否可以生成服务
        /// </summary>
        /// <param name="service">服务名或者别名</param>
        /// <returns>是否可以生成服务</returns>
        bool CanMake(string service);

        /// <summary>
        /// 服务是否是静态化的,如果服务不存在也将返回false
        /// </summary>
        /// <param name="service">服务名或者别名</param>
        /// <returns>是否是静态化的</returns>
        bool IsStatic(string service);

        /// <summary>
        /// 是否是别名
        /// </summary>
        /// <param name="name">名字</param>
        /// <returns>是否是别名</returns>
        bool IsAlias(string name);

        /// <summary>
        /// 绑定一个服务
        /// </summary>
        /// <param name="service">服务名</param>
        /// <param name="concrete">服务实现</param>
        /// <param name="isStatic">服务是否静态化</param>
        /// <returns>服务绑定数据</returns>
        IBindData Bind(string service, Type concrete, bool isStatic);

        /// <summary>
        /// 绑定一个服务
        /// </summary>
        /// <param name="service">服务名</param>
        /// <param name="concrete">服务实体</param>
        /// <param name="isStatic">服务是否静态化</param>
        /// <returns>服务绑定数据</returns>
        IBindData Bind(string service, Func<IContainer, object[], object> concrete, bool isStatic);

        /// <summary>
        /// 如果服务不存在那么则绑定服务
        /// </summary>
        /// <param name="service">服务名</param>
        /// <param name="concrete">服务实现</param>
        /// <param name="isStatic">服务是否是静态的</param>
        /// <param name="bindData">如果绑定失败则返回历史绑定对象</param>
        /// <returns>是否成功绑定</returns>
        bool BindIf(string service, Func<IContainer, object[], object> concrete, bool isStatic, out IBindData bindData);

        /// <summary>
        /// 如果服务不存在那么则绑定服务
        /// </summary>
        /// <param name="service">服务名</param>
        /// <param name="concrete">服务实现</param>
        /// <param name="isStatic">服务是否是静态的</param>
        /// <param name="bindData">如果绑定失败则返回历史绑定对象</param>
        /// <returns>是否成功绑定</returns>
        bool BindIf(string service, Type concrete, bool isStatic, out IBindData bindData);

        /// <summary>
        /// 绑定一个方法到容器
        /// </summary>
        /// <param name="method">方法名</param>
        /// <param name="target">调用目标</param>
        /// <param name="call">调用方法</param>
        /// <returns>方法绑定数据</returns>
        IMethodBind BindMethod(string method, object target, MethodInfo call);

        /// <summary>
        /// 解除绑定的方法
        /// </summary>
        /// <param name="target">
        /// 解除目标
        /// <para>如果为字符串则作为调用方法名</para>
        /// <para>如果为<code>IMethodBind</code>则作为指定方法</para>
        /// <para>如果为其他对象则作为调用目标做全体解除</para>
        /// </param>
        void UnbindMethod(object target);

        /// <summary>
        /// 解除绑定服务
        /// </summary>
        /// <param name="service">服务名或者别名</param>
        void Unbind(string service);

        /// <summary>
        /// 为一个及以上的服务定义一个标记
        /// </summary>
        /// <param name="tag">标记名</param>
        /// <param name="service">服务名</param>
        void Tag(string tag, params string[] service);

        /// <summary>
        /// 根据标记名生成标记所对应的所有服务实例
        /// </summary>
        /// <param name="tag">标记名</param>
        /// <returns>将会返回标记所对应的所有服务实例</returns>
        object[] Tagged(string tag);

        /// <summary>
        /// 静态化一个服务
        /// </summary>
        /// <param name="service">服务名或者别名</param>
        /// <param name="instance">服务实例</param>
        object Instance(string service, object instance);

        /// <summary>
        /// 释放某个静态化实例
        /// </summary>
        /// <param name="service">服务名或别名</param>
        bool Release(string service);

        /// <summary>
        /// 清空容器的所有实例，绑定，别名，标签，解决器，方法容器
        /// </summary>
        void Flush();

        /// <summary>
        /// 调用一个已经被绑定的方法
        /// </summary>
        /// <param name="method">方法名</param>
        /// <param name="userParams">用户提供的参数</param>
        /// <returns>调用结果</returns>
        object Invoke(string method, params object[] userParams);

        /// <summary>
        /// 以依赖注入形式调用一个方法
        /// </summary>
        /// <param name="instance">方法对象</param>
        /// <param name="methodInfo">方法信息</param>
        /// <param name="userParams">用户传入的参数</param>
        /// <returns>方法返回值</returns>
        object Call(object instance, MethodInfo methodInfo, params object[] userParams);

        /// <summary>
        /// 构造服务
        /// </summary>
        /// <param name="service">服务名或别名</param>
        /// <param name="userParams">用户传入的参数</param>
        /// <returns>服务实例，如果构造失败那么返回null</returns>
        object Make(string service, params object[] userParams);

        /// <summary>
        /// 构造服务
        /// </summary>
        /// <param name="service">服务名或者别名</param>
        /// <returns>服务实例，如果构造失败那么返回null</returns>
		object this[string service] { get; }

        /// <summary>
        /// 获取一个回调，当执行回调可以生成指定的服务
        /// </summary>
        /// <param name="service">服务名或别名</param>
        /// <param name="userParams">用户传入的参数</param>
        /// <returns>回调方案</returns>
        Func<object> Factory(string service, params object[] userParams);

        /// <summary>
        /// 以全局的方式为服务设定一个别名
        /// </summary>
        /// <param name="alias">别名</param>
        /// <param name="service">映射到的服务名</param>
        /// <returns>当前容器对象</returns>
        IContainer Alias(string alias, string service);

        /// <summary>
        /// 当服务被解决时触发的事件
        /// </summary>
        /// <param name="func">回调函数</param>
        /// <returns>当前容器实例</returns>
        IContainer OnResolving(Func<IBindData, object, object> func);

        /// <summary>
        /// 当静态服务被释放时
        /// </summary>
        /// <param name="action">处理释放时的回调</param>
        IContainer OnRelease(Action<IBindData, object> action);

        /// <summary>
        /// 当查找类型无法找到时会尝试去调用开发者提供的查找类型函数
        /// </summary>
        /// <param name="func">查找类型的回调</param>
        /// <param name="priority">查询优先级(值越小越优先)</param>
        /// <returns>当前容器实例</returns>
        IContainer OnFindType(Func<string, Type> func, int priority = int.MaxValue);

        /// <summary>
        /// 当一个已经被解决的服务，发生重定义时触发
        /// </summary>
        /// <param name="service">服务名</param>
        /// <param name="callback">回调</param>
        /// <returns>服务容器</returns>
        IContainer OnRebound(string service, Action<object> callback);

        /// <summary>
        /// 关注指定的服务，当服务触发重定义时调用指定对象的指定方法
        /// <para>调用是以依赖注入的形式进行的</para>
        /// <para>服务的新建（第一次解决服务）操作并不会触发重定义</para>
        /// </summary>
        /// <param name="service">关注的服务名</param>
        /// <param name="target">当服务发生重定义时调用的目标</param>
        /// <param name="methodInfo">方法信息</param>
        void Watch(string service, object target, MethodInfo methodInfo);

        /// <summary>
        /// 在回调区间内暂时性的静态化服务实例
        /// </summary>
        /// <param name="callback">回调区间</param>
        /// <param name="services">服务映射</param>
        void Flash(Action callback, params KeyValuePair<string, object>[] services);

        /// <summary>
        /// 类型转为服务名
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>转换后的服务名</returns>
        string Type2Service(Type type);
    }
}