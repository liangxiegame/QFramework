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
    /// 绑定关系临时数据,用于支持链式调用
    /// </summary>
    public interface IGivenData<TReturn>
    {
        /// <summary>
        /// 给与什么服务
        /// </summary>
        /// <param name="service">给与的服务名或别名</param>
        /// <returns>服务绑定数据</returns>
        TReturn Given(string service);

        /// <summary>
        /// 给与什么服务
        /// </summary>
        /// <typeparam name="T">给与的服务名或别名</typeparam>
        /// <returns>服务绑定数据</returns>
        TReturn Given<T>();
    }
}