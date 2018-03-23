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

namespace CatLib
{
    /// <summary>
    /// 服务提供者
    /// </summary>
    public interface IServiceProvider
    {
        /// <summary>
        /// 服务提供者初始化
        /// </summary>
        void Init();

        /// <summary>
        /// 当注册服务提供者
        /// </summary>
        void Register();
    }
}