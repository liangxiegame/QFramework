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
    /// 参数名注入表
    /// </summary>
    public interface IParams
    {
        /// <summary>
        /// 获取一个参数
        /// </summary>
        /// <param name="key">参数名</param>
        /// <param name="value">参数值</param>
        /// <returns>是否成功获取</returns>
        bool TryGetValue(string key, out object value);
    }
}
