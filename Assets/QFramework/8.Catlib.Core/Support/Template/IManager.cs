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
    /// 管理器
    /// </summary>
    public interface IManager<TInterface> : IManaged<TInterface>
    {
        /// <summary>
        /// 获取解决方案
        /// </summary>
        /// <param name="name">解决方案名</param>
        /// <returns>解决方案</returns>
        TInterface Get(string name = null);

        /// <summary>
        /// 获取解决方案
        /// </summary>
        /// <param name="name">解决方案名</param>
        /// <returns>解决方案</returns>
        TInterface this[string name] { get; }
    }
}
