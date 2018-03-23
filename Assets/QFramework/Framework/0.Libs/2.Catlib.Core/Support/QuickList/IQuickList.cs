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

using System.Collections.Generic;

namespace CatLib
{
    /// <summary>
    /// 快速列表
    /// </summary>
    public interface IQuickList<TElement> : IEnumerable<TElement>
    {
        /// <summary>
        /// 列表元素基数
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 快速列表中的结点数量
        /// </summary>
        int Length { get; }

        /// <summary>
        /// 将元素插入到列表尾部
        /// </summary>
        /// <param name="element">元素</param>
        void Add(TElement element);

        /// <summary>
        /// 将元素插入到列表尾部
        /// </summary>
        /// <param name="element">元素</param>
        void Push(TElement element);

        /// <summary>
        /// 将元素插入到列表头部
        /// </summary>
        /// <param name="element">元素</param>
        void UnShift(TElement element);

        /// <summary>
        /// 移除并返回列表的尾部元素
        /// </summary>
        /// <returns>元素</returns>
        TElement Pop();

        /// <summary>
        /// 移除并返回列表头部的元素
        /// </summary>
        /// <returns>元素</returns>
        TElement Shift();

        /// <summary>
        /// 对列表进行修剪，只保留下标范围内的元素
        /// </summary>
        /// <param name="start">起始下标</param>
        /// <param name="end">结束下标</param>
        /// <returns>移除的元素数量</returns>
        int Trim(int start, int end);

        /// <summary>
        /// 根据参数 <paramref name="count"/> 的值，移除列表中与参数 <paramref name="element"/> 相等的元素。
        /// <para><c>count &gt; 0</c> : 从表头开始向表尾搜索，移除与 <paramref name="element"/> 相等的元素，数量为 <paramref name="count"/>。</para>
        /// <para><c>count &lt; 0</c> : 从表尾开始向表头搜索，移除与 <paramref name="element"/> 相等的元素，数量为绝对值 <paramref name="count"/>。</para>
        /// <para><c>count = 0</c> : 移除表中所有与 <paramref name="element"/> 相等的元素。</para>
        /// </summary>
        /// <param name="element">要被移除的元素</param>
        /// <param name="count">移除的元素数量，使用正负来决定扫描起始位置，如果<paramref name="count"/>为0则全部匹配的元素，反之移除指定数量。</param>
        /// <returns>被移除元素的数量</returns>
        int Remove(TElement element, int count = 0);

        /// <summary>
        /// 获取区间内的所有元素,1个元素占1个位置，范围不允许使用负数表示
        /// </summary>
        /// <param name="start">起始位置(包含)</param>
        /// <param name="end">结束位置(包含)</param>
        /// <returns>区间内的元素列表</returns>
        TElement[] GetRange(int start, int end);

        /// <summary>
        /// 通过下标访问元素,如果传入的是一个负数下标那么从末尾开始查找
        /// </summary>
        /// <param name="index">下标，允许为负数</param>
        /// <returns>元素</returns>
        TElement this[int index] { get; set; }

        /// <summary>
        /// 在指定元素之后插入
        /// </summary>
        /// <param name="finder">查找的元素</param>
        /// <param name="insert">要插入的元素</param>
        void InsertAfter(TElement finder, TElement insert);

        /// <summary>
        /// 在指定元素之前插入
        /// </summary>
        /// <param name="finder">查找的元素</param>
        /// <param name="insert">要插入的元素</param>
        void InsertBefore(TElement finder, TElement insert);

        /// <summary>
        /// 反转遍历顺序(并不是反转整个列表)
        /// </summary>
        void ReverseIterator();

        /// <summary>
        /// 清空快速列表
        /// </summary>
        void Clear();

        /// <summary>
        /// 获取第一个元素
        /// </summary>
        /// <returns>第一个元素</returns>
        TElement First();

        /// <summary>
        /// 获取最后一个元素
        /// </summary>
        /// <returns>最后一个元素</returns>
        TElement Last();
    }
}
