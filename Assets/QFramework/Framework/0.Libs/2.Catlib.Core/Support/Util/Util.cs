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
using System.Reflection;

namespace CatLib
{
    /// <summary>
    /// 通用支持
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// 获取优先级
        /// </summary>
        /// <param name="type">识别的类型</param>
        /// <param name="method">识别的方法</param>
        /// <returns>优先级</returns>
        public static int GetPriority(Type type, string method = null)
        {
            Guard.Requires<ArgumentNullException>(type != null);
            var priority = typeof(PriorityAttribute);
            var currentPriority = int.MaxValue;

            MethodInfo methodInfo;
            if (method != null &&
                (methodInfo = type.GetMethod(method)) != null &&
                methodInfo.IsDefined(priority, false))
            {
                currentPriority = ((PriorityAttribute)methodInfo.GetCustomAttributes(priority, false)[0]).Priorities;
            }
            else if (type.IsDefined(priority, false))
            {
                currentPriority = ((PriorityAttribute)type.GetCustomAttributes(priority, false)[0]).Priorities;
            }

            return currentPriority;
        }

        /// <summary>
        /// 构建一个随机生成器
        /// </summary>
        /// <param name="seed">种子</param>
        /// <returns>随机生成器</returns>
        public static System.Random MakeRandom(int? seed = null)
        {
            return new System.Random(seed.GetValueOrDefault(MakeSeed()));
        }

        /// <summary>
        /// 生成种子
        /// </summary>
        /// <returns>种子</returns>
        public static int MakeSeed()
        {
            return Environment.TickCount ^ Guid.NewGuid().GetHashCode();
        }

        /// <summary>
        /// 标准化位置
        /// </summary>
        /// <param name="sourceLength">源长度</param>
        /// <param name="start">起始位置</param>
        /// <param name="length">作用长度</param>
        internal static void NormalizationPosition(int sourceLength, ref int start, ref int? length)
        {
            start = (start >= 0) ? Math.Min(start, sourceLength) : Math.Max(sourceLength + start, 0);

            length = (length == null)
                ? Math.Max(sourceLength - start, 0)
                : (length >= 0)
                    ? Math.Min(length.Value, sourceLength - start)
                    : Math.Max(sourceLength + length.Value - start, 0);
        }
    }
}
