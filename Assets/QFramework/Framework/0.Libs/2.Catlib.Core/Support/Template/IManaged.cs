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
    /// 管理器模版 - 扩展内容不对外可见
    /// </summary>
    /// <typeparam name="TInterface">拓展接口</typeparam>
    public interface IManaged<TInterface>
    {
        /// <summary>
        /// 自定义解决方案
        /// </summary>
        /// <param name="resolve">解决方案实现</param>
        /// <param name="name">解决方案名</param>
        void Extend(Func<TInterface> resolve, string name = null);

        /// <summary>
        /// 释放扩展
        /// </summary>
        /// <param name="name">名字</param>
        void ReleaseExtend(string name = null);

        /// <summary>
        /// 是否包含指定拓展
        /// </summary>
        /// <param name="name">名字</param>
        bool ContainsExtend(string name = null);
    }
}
