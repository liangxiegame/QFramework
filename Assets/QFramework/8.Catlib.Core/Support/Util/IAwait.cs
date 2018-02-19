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
    /// 等待接口
    /// </summary>
    /// <typeparam name="TInterface">等待目标接口</typeparam>
    public interface IAwait<TInterface> : IAwait where TInterface : class
    {
        /// <summary>
        /// 结果
        /// </summary>
        new TInterface Result { get; }
    }

    /// <summary>
    /// 等待接口
    /// </summary>
    public interface IAwait
    {
        /// <summary>
        /// 是否准备完成
        /// </summary>
        bool IsDone { get; }

        /// <summary>
        /// 实现
        /// </summary>
        object Result { get; }
    }
}
