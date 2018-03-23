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
    /// 引用计数
    /// </summary>
    public interface IReferenceCount
    {
        /// <summary>
        /// 当前引用计数
        /// </summary>
        int RefCount { get; }

        /// <summary>
        /// 加计数。
        /// </summary>
        void Retain();

        /// <summary>
        /// 减计数
        /// </summary>
        void Release();
    }
}
