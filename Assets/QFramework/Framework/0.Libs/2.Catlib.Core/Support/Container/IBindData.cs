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
    /// 服务绑定数据
    /// </summary>
    public interface IBindData : IBindable<IBindData>
    {
        /// <summary>
        /// 服务实现
        /// </summary>
        Func<IContainer, object[], object> Concrete { get; }

        /// <summary>
        /// 是否是静态服务
        /// </summary>
        bool IsStatic { get; }

        /// <summary>
        /// 为服务设定一个别名
        /// </summary>
        /// <typeparam name="T">别名</typeparam>
        /// <returns>服务绑定数据</returns>
        IBindData Alias<T>();

        /// <summary>
        /// 为服务设定一个别名
        /// </summary>
        /// <param name="alias">别名</param>
        /// <returns>服务绑定数据</returns>
        IBindData Alias(string alias);

        /// <summary>
        /// 解决服务时触发的回调
        /// </summary>
        /// <param name="func">解决事件</param>
        /// <returns>服务绑定数据</returns>
        IBindData OnResolving(Func<IBindData, object, object> func);

        /// <summary>
        /// 当服务被释放时
        /// </summary>
        /// <param name="action">处理事件</param>
        /// <returns>服务绑定数据</returns>
        IBindData OnRelease(Action<IBindData, object> action);
    }
}
