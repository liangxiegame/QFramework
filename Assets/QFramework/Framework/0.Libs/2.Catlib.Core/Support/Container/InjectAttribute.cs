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
    /// 注入标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface |
                    AttributeTargets.Parameter |
                    AttributeTargets.Property)]
    public class InjectAttribute : Attribute
    {
        /// <summary>
        /// 注入服务的别名或者服务名
        /// </summary>
        public string Alias { get; private set; }

        /// <summary>
        /// 声明注入
        /// </summary>
        /// <param name="alias">依赖服务的别名或者服务名</param>
        public InjectAttribute(string alias)
        {
            Alias = alias;
        }

        /// <summary>
        /// 声明注入
        /// </summary>
        public InjectAttribute() { }
    }
}