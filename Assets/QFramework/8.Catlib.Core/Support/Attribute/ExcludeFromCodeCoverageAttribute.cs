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
    /// 标记的类，函数，属性不再进行覆盖率分析
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Constructor, Inherited = false)]
    public class ExcludeFromCodeCoverageAttribute : Attribute
    {
    }
}
