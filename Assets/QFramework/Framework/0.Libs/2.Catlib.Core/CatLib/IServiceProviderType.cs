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
    /// 主动标志服务提供者的基础类型
    /// </summary>
    public interface IServiceProviderType
    {
        /// <summary>
        /// 提供者基础类型
        /// </summary>
        Type BaseType { get; }
    }
}
