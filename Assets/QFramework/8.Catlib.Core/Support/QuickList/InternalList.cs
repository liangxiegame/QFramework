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
    /// 内部列表
    /// </summary>
    internal sealed class InternalList<TElement>
    {
        /// <summary>
        /// 列表元素
        /// </summary>
        private readonly TElement[] items;

        /// <summary>
        /// 实际元素个数
        /// </summary>
        internal int Count { get; private set; }

        /// <summary>
        /// 是否被删除
        /// </summary>
        internal bool IsDelete { get; set; }

        /// <summary>
        /// 新建一个内部列表
        /// </summary>
        /// <param name="capacity">容量</param>
        internal InternalList(int capacity = 128)
        {
            items = new TElement[capacity];
            Count = 0;
            IsDelete = false;
        }

        /// <summary>
        /// 获取或者设定一个元素
        /// </summary>
        /// <param name="offset">偏移量</param>
        /// <returns>元素</returns>
        internal TElement this[int offset]
        {
            get
            {
                return items[offset];
            }
        }

        /// <summary>
        /// 在指定位置插入元素
        /// </summary>
        /// <param name="element">元素</param>
        /// <param name="offset">偏移量</param>
        internal void InsertAt(TElement element, int offset)
        {
            if (offset != Count)
            {
                Array.Copy(items, offset, items, offset + 1, Count - offset);
            }
            items[offset] = element;
            ++Count;
        }

        /// <summary>
        /// 移除元素
        /// </summary>
        /// <param name="offset">偏移量</param>
        internal void RemoveAt(int offset)
        {
            Array.Copy(items, offset + 1, items, offset, Count - offset - 1);
            Array.Clear(items , Count - 1 , 1);
            --Count;
        }

        /// <summary>
        /// 移除范围内的元素
        /// </summary>
        /// <param name="start">起始范围(包含)</param>
        /// <param name="end">结束范围(包含)</param>
        internal void RemoveRange(int start, int end)
        {
            Array.Copy(items, end + 1, items, start, Count - end - 1);
            Array.Clear(items, Count - (end - start) - 1, (end - start) + 1);
            Count -= (end - start) + 1;
        }

        /// <summary>
        /// 替换指定位置的元素
        /// </summary>
        /// <param name="element">元素</param>
        /// <param name="offset">偏移量</param>
        internal void ReplaceAt(TElement element ,int offset)
        {
            items[offset] = element;
        }

        /// <summary>
        /// 合入元素
        /// </summary>
        /// <param name="elements">元素</param>
        internal void Init(TElement[] elements)
        {
            Array.Copy(elements, 0, items, 0, elements.Length);
            Count = elements.Length;
        }

        /// <summary>
        /// 合并内部列表
        /// </summary>
        /// <param name="join">需要被合并进来的内部列表</param>
        /// <param name="after">是否在当前列表元素的尾部合入</param>
        internal void Merge(InternalList<TElement> join, bool after)
        {
            if (after)
            {
                Array.Copy(join.items, 0, items, Count, join.Count);
                Count += join.Count;
                join.Count = 0;
                return;
            }
            Array.Copy(items, 0, items, join.Count, Count);
            Array.Copy(join.items, 0, items, 0, join.Count);
            Count += join.Count;
            join.Count = 0;
        }

        /// <summary>
        /// 拆分列表
        /// </summary>
        /// <param name="offset">偏移量</param>
        /// <param name="after">是否是后拆</param>
        internal TElement[] Split(int offset, bool after)
        {
            TElement[] data;
            if (after)
            {
                data = new TElement[Count - offset - 1];
                Array.Copy(items, offset + 1, data, 0, Count - offset - 1);
                Array.Clear(items, offset + 1, Count - offset - 1);

                Count -= (Count - offset - 1);
                return data;
            }

            data = new TElement[offset];
            Array.Copy(items, 0, data, 0, data.Length);
            Array.Copy(items, offset, items, 0, Count - offset);
            Array.Clear(items, Count - offset, items.Length - Count + offset);
            Count -= offset;
            return data;
        }

        /// <summary>
        /// 在尾部推入元素
        /// </summary>
        /// <param name="element">元素</param>
        internal void Push(TElement element)
        {
            items[Count] = element;
            ++Count;
        }

        /// <summary>
        /// 弹出尾部数据
        /// </summary>
        /// <returns></returns>
        internal TElement Pop()
        {
            --Count;
            var ele = items[Count];
            items[Count] = default(TElement);
            return ele;
        }

        /// <summary>
        /// 在列表头部加入元素
        /// </summary>
        /// <param name="element"></param>
        internal void UnShift(TElement element)
        {
            Array.Copy(items, 0, items, 1, Count);
            items[0] = element;
            ++Count;
        }

        /// <summary>
        /// 推出列表头部的元素
        /// </summary>
        /// <returns>元素</returns>
        internal TElement Shift()
        {
            --Count;
            var ele = items[0];
            Array.Copy(items, 1, items, 0, Count);
            return ele;
        }
    }
}
