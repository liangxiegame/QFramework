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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace CatLib
{
    /// <summary>
    /// 快速列表
    /// </summary>
    /// <typeparam name="TElement">元素</typeparam>
    [DebuggerDisplay("Count = {Count} , Length = {Length}")]
    public sealed class QuickList<TElement> : IQuickList<TElement>
    {
        /// <summary>
        /// 合并系数
        /// </summary>
        private const float MergeCoefficient = 0.9f;

        /// <summary>
        /// 快速列表结点
        /// </summary>
        private class QuickListNode
        {
            /// <summary>
            /// 后置结点
            /// </summary>
            internal QuickListNode Backward;

            /// <summary>
            /// 前置结点
            /// </summary>
            internal QuickListNode Forward;

            /// <summary>
            /// 列表
            /// </summary>
            internal InternalList<TElement> List;
        }

        /// <summary>
        /// 快速列表迭代器
        /// </summary>
        public struct Enumerator : IEnumerator<TElement>
        {
            /// <summary>
            /// 快速列表
            /// </summary>
            private readonly QuickList<TElement> quickList;

            /// <summary>
            /// 是否是向前遍历
            /// </summary>
            private readonly bool forward;

            /// <summary>
            /// 版本
            /// </summary>
            private readonly int version;

            /// <summary>
            /// 当前元素
            /// </summary>
            private TElement current;

            /// <summary>
            /// 当前节点
            /// </summary>
            private QuickListNode node;

            /// <summary>
            /// 访问下标
            /// </summary>
            private int index;

            /// <summary>
            /// 构造一个迭代器
            /// </summary>
            /// <param name="quickList"></param>
            /// <param name="forward"></param>
            internal Enumerator(QuickList<TElement> quickList, bool forward)
            {
                this.quickList = quickList;
                this.forward = forward;
                version = quickList.version;
                current = default(TElement);
                node = forward ? quickList.header : quickList.tail;
                index = 0;
            }

            /// <summary>
            /// 移动到下一个节点
            /// </summary>
            /// <returns>下一个节点是否存在</returns>
            public bool MoveNext()
            {
                if (node == null)
                {
                    return false;
                }

                if (version != quickList.version)
                {
                    throw new InvalidOperationException("Can not modify data when iterates again.");
                }

                if (forward)
                {
                    do
                    {
                        if (index < node.List.Count)
                        {
                            current = node.List[index++];
                            return true;
                        }
                        index = 0;
                        node = node.Forward;
                    } while (node != null);
                    return false;
                }

                do
                {
                    if (index < node.List.Count)
                    {
                        current = node.List[node.List.Count - ++index];
                        return true;
                    }
                    index = 0;
                    node = node.Backward;
                } while (node != null);
                return false;
            }

            /// <summary>
            /// 获取当前元素
            /// </summary>
            public TElement Current
            {
                get
                {
                    return current;
                }
            }

            /// <summary>
            /// 获取当前元素
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    return current;
                }
            }

            /// <summary>
            /// 重置迭代器
            /// </summary>
            void IEnumerator.Reset()
            {
                current = default(TElement);
                node = forward ? quickList.header : quickList.tail;
                index = 0;
            }

            /// <summary>
            /// 释放时
            /// </summary>
            public void Dispose()
            {
            }
        }

        /// <summary>
        /// 每个快速列表结点最多的元素数量
        /// </summary>
        private readonly int fill;

        /// <summary>
        /// 列表头
        /// </summary>
        private QuickListNode header;

        /// <summary>
        /// 列表尾
        /// </summary>
        private QuickListNode tail;

        /// <summary>
        /// 是否是向前的迭代方向
        /// </summary>
        private bool forward;

        /// <summary>
        /// 版本号
        /// </summary>
        private int version;

        /// <summary>
        /// 同步锁
        /// </summary>
        private readonly object syncRoot = new object();

        /// <summary>
        /// 同步锁
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return syncRoot;
            }
        }

        /// <summary>
        /// 列表元素基数
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// 快速列表中的结点数量
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// 快速列表
        /// </summary>
        /// <param name="fill">每个结点中元素的最大数量</param>
        public QuickList(int fill = 128)
        {
            this.fill = fill;
            forward = true;
            version = 0;
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            header = null;
            tail = null;
            version = 0;
            Count = 0;
            Length = 0;
        }

        /// <summary>
        /// 获取第一个元素
        /// </summary>
        /// <returns>第一个元素</returns>
        public TElement First()
        {
            if (header != null)
            {
                return header.List[0];
            }
            throw new InvalidOperationException("QuickList is Null");
        }

        /// <summary>
        /// 获取最后一个元素
        /// </summary>
        /// <returns>最后一个元素</returns>
        public TElement Last()
        {
            if (tail != null)
            {
                return tail.List[tail.List.Count - 1];
            }
            throw new InvalidOperationException("QuickList is Null");
        }

        /// <summary>
        /// 将元素插入到列表尾部
        /// </summary>
        /// <param name="element">元素</param>
        public void Add(TElement element)
        {
            Push(element);
        }

        /// <summary>
        /// 将元素插入到列表尾部
        /// </summary>
        /// <param name="element">元素</param>
        public void Push(TElement element)
        {
            Insert(element, tail, tail != null ? tail.List.Count - 1 : 0, true);
        }

        /// <summary>
        /// 将元素插入到列表头部
        /// </summary>
        /// <param name="element">元素</param>
        public void UnShift(TElement element)
        {
            Insert(element, header, 0, false);
        }

        /// <summary>
        /// 移除并返回列表的尾部元素
        /// </summary>
        /// <returns>元素</returns>
        public TElement Pop()
        {
            if (tail != null)
            {
                return ListPop(tail, false);
            }
            throw new InvalidOperationException("QuickList is Null");
        }

        /// <summary>
        /// 移除并返回列表头部的元素
        /// </summary>
        /// <returns>元素</returns>
        public TElement Shift()
        {
            if (header != null)
            {
                return ListPop(header, true);
            }
            throw new InvalidOperationException("QuickList is Null");
        }

        /// <summary>
        /// 对列表进行修剪，只保留下标范围内的元素
        /// </summary>
        /// <param name="start">起始下标(包含)</param>
        /// <param name="end">结束下标(包含)</param>
        /// <returns>移除的元素数量</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/>大于<paramref name="end"/>或者<paramref name="start"/>小于0时引发</exception>
        public int Trim(int start, int end)
        {
            end = Math.Min(end, Count);
            Guard.Requires<ArgumentOutOfRangeException>(start <= end);
            Guard.Requires<ArgumentOutOfRangeException>(start >= 0);

            var remove = 0;
            var sumIndex = 0;
            var endStep = (end - start);
            var node = header;
            while (node != null)
            {
                if ((sumIndex + node.List.Count) <= start || endStep < 0)
                {
                    remove += node.List.Count;
                    sumIndex += node.List.Count;
                    DeleteNode(node);
                    node = node.Forward;
                    continue;
                }

                //起始的偏移量，相对于当前结点
                var offset = Math.Max(start - sumIndex, 0);

                for (var i = 0; i < node.List.Count; i++)
                {
                    //如果小于起始偏移量
                    if (i < offset)
                    {
                        node.List.RemoveRange(i, Math.Max(offset - 1, 0));
                        var num = offset - i;
                        remove += num;
                        sumIndex += num;
                        Count -= num;
                        offset -= num;
                        --i;
                    }
                    //步径用完那么就删除
                    else if (endStep < 0)
                    {
                        //从当前偏移量位置一直删除到结点最后一个元素
                        var num = node.List.Count - i;
                        node.List.RemoveRange(i, node.List.Count - 1);
                        remove += num;
                        sumIndex += num;
                        Count -= num;
                        offset -= num;
                        i += num - 1;
                    }
                    else
                    {
                        var num = node.List.Count - i;

                        //直接跳过保留的元素
                        if ((endStep + 1) >= num)
                        {
                            endStep -= num;
                            sumIndex += num;
                            break;
                        }

                        i += endStep;
                        sumIndex += endStep + 1;
                        endStep = -1;
                    }
                }
                node = node.Forward;
            }
            return remove;
        }

        /// <summary>
        /// 根据参数 <paramref name="count"/> 的值，移除列表中与参数 <paramref name="element"/> 相等的元素。
        /// <para><c>count &gt; 0</c> : 从表头开始向表尾搜索，移除与 <paramref name="element"/> 相等的元素，数量为 <paramref name="count"/>。</para>
        /// <para><c>count &lt; 0</c> : 从表尾开始向表头搜索，移除与 <paramref name="element"/> 相等的元素，数量为绝对值 <paramref name="count"/>。</para>
        /// <para><c>count = 0</c> : 移除表中所有与 <paramref name="element"/> 相等的元素。</para>
        /// </summary>
        /// <param name="element">要被移除的元素</param>
        /// <param name="count">移除的元素数量，使用正负来决定扫描起始位置，如果<paramref name="count"/>为0则全部匹配的元素，反之移除指定数量。</param>
        /// <returns>被移除元素的数量</returns>
        public int Remove(TElement element, int count = 0)
        {
            var remove = 0;
            QuickListNode node;
            int i;
            if (count >= 0)
            {
                count = Math.Abs(count);
                node = header;
                while (node != null)
                {
                    for (i = 0; i < node.List.Count; ++i)
                    {
                        if (!(node.List[i] == null && element == null) &&
                            (node.List[i] == null || !node.List[i].Equals(element)))
                        {
                            continue;
                        }

                        node.List.RemoveAt(i);
                        ++remove;
                        --Count;
                        ++version;
                        --i;
                        if (count != 0 && (--count) == 0)
                        {
                            return remove;
                        }
                    }
                    node = node.Forward;
                }
            }
            else
            {
                count = Math.Abs(count);
                node = tail;
                while (node != null)
                {
                    for (i = node.List.Count - 1; i >= 0; --i)
                    {
                        if (!(node.List[i] == null && element == null) &&
                            (node.List[i] == null || !node.List[i].Equals(element)))
                        {
                            continue;
                        }

                        node.List.RemoveAt(i);
                        ++remove;
                        --Count;
                        ++version;
                        if (count != 0 && (--count) == 0)
                        {
                            return remove;
                        }
                    }
                    node = node.Backward;
                }
            }
            return remove;
        }

        /// <summary>
        /// 获取区间内的所有元素,1个元素占1个位置
        /// </summary>
        /// <param name="start">起始位置(包含)</param>
        /// <param name="end">结束位置(包含)</param>
        /// <returns>区间内的元素列表</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/>大于<paramref name="end"/>时引发</exception>
        public TElement[] GetRange(int start, int end)
        {
            start = Math.Max(start, 0);
            end = Math.Min(end, Count);
            Guard.Requires<ArgumentOutOfRangeException>(start <= end);

            var elements = new TElement[end - start + 1];
            var sumIndex = 0;
            var node = header;
            while (node != null)
            {
                if ((sumIndex + node.List.Count) <= start)
                {
                    sumIndex += node.List.Count;
                    node = node.Forward;
                    continue;
                }

                //基础偏移量
                var offset = start - sumIndex;

                for (var i = 0; i < elements.Length; i++)
                {
                    elements[i] = node.List[offset];
                    if (++offset >= fill)
                    {
                        offset = 0;
                        node = node.Forward;
                    }
                }
                return elements;
            }
            return new TElement[] { };
        }

        /// <summary>
        /// 通过下标访问元素,如果传入的是一个负数下标那么从末尾开始查找
        /// </summary>
        /// <param name="index">下标，允许为负数</param>
        /// <returns>元素</returns>
        /// <exception cref="ArgumentOutOfRangeException">下标越界时会引发</exception>
        public TElement this[int index]
        {
            get
            {
                int offset;
                var node = FindByIndex(index, out offset);
                Guard.Requires<ArgumentOutOfRangeException>(node != null);
                return node.List[offset];
            }
            set
            {
                int offset;
                var node = FindByIndex(index, out offset);
                Guard.Requires<ArgumentOutOfRangeException>(node != null);
                node.List.ReplaceAt(value, offset);
            }
        }

        /// <summary>
        /// 在指定元素之后插入
        /// </summary>
        /// <param name="finder">查找的元素</param>
        /// <param name="insert">要插入的元素</param>
        public void InsertAfter(TElement finder, TElement insert)
        {
            int offset;
            var node = FindNode(finder, out offset);
            if (node == null)
            {
                return;
            }
            Insert(insert, node, offset, true);
        }

        /// <summary>
        /// 在指定元素之前插入
        /// </summary>
        /// <param name="finder">查找的元素</param>
        /// <param name="insert">要插入的元素</param>
        public void InsertBefore(TElement finder, TElement insert)
        {
            int offset;
            var node = FindNode(finder, out offset);
            if (node == null)
            {
                return;
            }
            Insert(insert, node, offset, false);
        }

        /// <summary>
        /// 反转遍历顺序(并不是反转整个列表)
        /// </summary>
        public void ReverseIterator()
        {
            forward = !forward;
        }
        
        /// <summary>
        /// 迭代器
        /// </summary>
        /// <returns>迭代器</returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this, forward);
        }

        /// <summary>
        /// 迭代器
        /// </summary>
        /// <returns>迭代器</returns>
        IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 迭代器
        /// </summary>
        /// <returns>迭代器</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 根据下标查找元素,如果传入的是一个负数下标那么从末尾开始查找
        /// </summary>
        /// <param name="index">下标</param>
        /// <param name="offset">偏移量</param>
        /// <returns>元素</returns>
        private QuickListNode FindByIndex(int index, out int offset)
        {
            if (index >= Count)
            {
                offset = -1;
                return null;
            }
            if (index < 0)
            {
                index = Count + index;
                Guard.Requires<ArgumentOutOfRangeException>(index >= 0);
            }

            int sumIndex;
            QuickListNode node;

            if (index < Count * 0.5)
            {
                sumIndex = 0;
                node = header;
                while (node != null)
                {
                    if ((sumIndex + node.List.Count) <= index)
                    {
                        sumIndex += node.List.Count;
                        node = node.Forward;
                        continue;
                    }

                    offset = index - sumIndex;
                    return node;
                }
            }
            else
            {
                sumIndex = Count;
                node = tail;
                while (node != null)
                {
                    if ((sumIndex - node.List.Count) > index)
                    {
                        sumIndex -= node.List.Count;
                        node = node.Backward;
                        continue;
                    }
                    offset = node.List.Count - (sumIndex - index);
                    return node;
                }
            }

            offset = -1;
            return null;
        }

        /// <summary>
        /// 插入元素
        /// </summary>
        /// <param name="insert">被插入的元素</param>
        /// <param name="after">是否在被查找的元素之后插入</param>
        /// <param name="node">需要插入的结点</param>
        /// <param name="offset">结点相对偏移量</param>
        private void Insert(TElement insert, QuickListNode node, int offset, bool after)
        {
            bool full, fullNext, fullBackward, atTail, atHead;
            full = fullNext = fullBackward = atTail = atHead = false;
            QuickListNode newNode;

            if (node == null)
            {
                newNode = MakeNode();
                newNode.List.InsertAt(insert, 0);
                InsertNode(null, newNode, after);
                ++Count;
                ++version;
                return;
            }

            //如果结点不能插入那么标记为满
            if (!AllowInsert(node))
            {
                full = true;
            }

            if (after && (offset + 1) == node.List.Count)
            {
                //标记为尾部的元素
                atTail = true;
                //同时如果后面的结点也不能插入那么标记后置结点已满
                if (!AllowInsert(node.Forward))
                {
                    fullNext = true;
                }
            }

            if (!after && (offset == 0))
            {
                //标记为头部元素
                atHead = true;
                //同时如果之前的结点也不能插入那么标记前置结点已满
                if (!AllowInsert(node.Backward))
                {
                    fullBackward = true;
                }
            }

            //如果结点没有满，且是后插式插入
            if (!full && after)
            {
                if (offset + 1 < node.List.Count)
                {
                    //如果偏移量的位置之后还存在元素
                    node.List.InsertAt(insert, offset + 1);
                }
                else
                {
                    //如果之后没有元素那么直接推入
                    node.List.Push(insert);
                }
            }
            else if (!full)
            {
                //结点没有满，且是前插式
                node.List.InsertAt(insert, offset);
            }
            else if (atTail && node.Forward != null && !fullNext && after)
            {
                //如果当前结点满了，且是后插入尾部元素，并且下一个结点存在而且不是满的
                //那么就会插入到下一个结点中的头部
                newNode = node.Forward;
                newNode.List.UnShift(insert);
            }
            else if (atHead && node.Backward != null && !fullBackward && !after)
            {
                //如果当前结点满了，且是前插入头部元素，并且上一个结点存在而且不是满的
                //那么就会插入到上一个结点中的尾部
                newNode = node.Backward;
                newNode.List.Push(insert);
            }
            else if (((atTail && node.Forward != null && fullNext && after) ||
                      (atHead && node.Backward != null && fullBackward && !after)))
            {
                //如果当前结点是满的，且前置结点和后置结点都是满的那么
                //就新建一个结点，插入在2个结点之间
                newNode = MakeNode();
                newNode.List.InsertAt(insert, 0);
                InsertNode(node, newNode, after);
            }
            else
            {
                //如果当前结点是满的，且插入的元素不处于头部或者尾部的位置
                //那么拆分数据
                newNode = SplitNode(node, offset, after);
                if (after)
                {
                    newNode.List.UnShift(insert);
                }
                else
                {
                    newNode.List.Push(insert);
                }
                InsertNode(node, newNode, after);
                AttemptMergeNode(node);
            }

            ++Count;
            ++version;
        }

        /// <summary>
        /// 尝试合并结点
        /// </summary>
        /// <param name="node">发起合并的结点</param>
        private void AttemptMergeNode(QuickListNode node)
        {
            if (node == null)
            {
                return;
            }

            QuickListNode backward, backwardBackward, forward, forwardForward;
            backward = backwardBackward = forward = forwardForward = null;

            if (node.Backward != null)
            {
                backward = node.Backward;
                if (backward.Backward != null)
                {
                    backwardBackward = backward.Backward;
                }
            }

            if (node.Forward != null)
            {
                forward = node.Forward;
                if (forward.Forward != null)
                {
                    forwardForward = forward.Forward;
                }
            }

            if (AllowMerge(backward, backwardBackward))
            {
                MergeNode(backward, backwardBackward, false);
                backward = backwardBackward = null;
            }

            if (AllowMerge(forward, forwardForward))
            {
                MergeNode(forward, forwardForward, true);
                forward = forwardForward = null;
            }

            if (AllowMerge(node, node.Backward))
            {
                MergeNode(node, node.Backward, false);
            }

            if (AllowMerge(node, node.Forward))
            {
                MergeNode(node, node.Forward, true);
            }
        }

        /// <summary>
        /// 将从结点合并进主节点
        /// </summary>
        /// <param name="master">主结点</param>
        /// <param name="slave">从结点</param>
        /// <param name="after">从结点将怎么合并</param>
        private void MergeNode(QuickListNode master, QuickListNode slave, bool after)
        {
            master.List.Merge(slave.List, after);
            DeleteNode(slave);
        }

        /// <summary>
        /// 是否允许进行合并
        /// </summary>
        /// <param name="a">结点</param>
        /// <param name="b">结点</param>
        /// <returns>是否可以合并</returns>
        private bool AllowMerge(QuickListNode a, QuickListNode b)
        {
            if (a == null || b == null)
            {
                return false;
            }

            return a.List.Count + b.List.Count < (fill * MergeCoefficient);
        }

        /// <summary>
        /// 拆分结点
        /// </summary>
        /// <param name="node">要被拆分的结点</param>
        /// <param name="offset">拆分偏移量</param>
        /// <param name="after">前拆将会将offset之前的元素作为返回结点，后拆分则会将offset之后的元素作为返回结点</param>
        /// <returns>拆分出的结点</returns>
        private QuickListNode SplitNode(QuickListNode node, int offset, bool after)
        {
            var newNode = MakeNode();
            newNode.List.Init(node.List.Split(offset, after));
            return newNode;
        }

        /// <summary>
        /// 查找元素所在结点
        /// </summary>
        /// <param name="element">元素</param>
        /// <param name="offset">偏移量</param>
        /// <returns>所在结点，如果找不到结点则返回null</returns>
        private QuickListNode FindNode(TElement element, out int offset)
        {
            var node = header;
            while (node != null)
            {
                for (var i = 0; i < node.List.Count; ++i)
                {
                    if (!(node.List[i] == null && element == null) &&
                        (node.List[i] == null || !node.List[i].Equals(element)))
                    {
                        continue;
                    }

                    offset = i;
                    return node;
                }
                node = node.Forward;
            }
            offset = -1;
            return null;
        }

        /// <summary>
        /// 列表弹出数据
        /// </summary>
        /// <param name="node">结点</param>
        /// <param name="head">是否是头部</param>
        private TElement ListPop(QuickListNode node, bool head)
        {
            var ele = head ? node.List.Shift() : node.List.Pop();
            if (node.List.Count <= 0)
            {
                DeleteNode(node);
            }
            --Count;
            ++version;
            return ele;
        }

        /// <summary>
        /// 删除结点
        /// </summary>
        /// <param name="node">结点</param>
        private void DeleteNode(QuickListNode node)
        {
            if (node.Forward != null)
            {
                node.Forward.Backward = node.Backward;
            }
            if (node.Backward != null)
            {
                node.Backward.Forward = node.Forward;
            }
            if (node == tail)
            {
                tail = node.Backward;
            }
            if (node == header)
            {
                header = node.Forward;
            }
            Count -= node.List.Count;
            --Length;
            node.List.IsDelete = true;
        }

        /// <summary>
        /// 插入结点
        /// </summary>
        /// <param name="oldNode">旧的结点</param>
        /// <param name="newNode">新的结点</param>
        /// <param name="after">在旧的结点之前还是之后</param>
        private void InsertNode(QuickListNode oldNode, QuickListNode newNode, bool after)
        {
            if (after)
            {
                newNode.Backward = oldNode;
                if (oldNode != null)
                {
                    newNode.Forward = oldNode.Forward;
                    if (oldNode.Forward != null)
                    {
                        oldNode.Forward.Backward = newNode;
                    }
                    oldNode.Forward = newNode;
                }
                if (tail == oldNode)
                {
                    tail = newNode;
                }
            }
            else
            {
                newNode.Forward = oldNode;
                if (oldNode != null)
                {
                    newNode.Backward = oldNode.Backward;
                    if (oldNode.Backward != null)
                    {
                        oldNode.Backward.Forward = newNode;
                    }
                    oldNode.Backward = newNode;
                }
                if (header == oldNode)
                {
                    header = newNode;
                }
            }
            if (Length == 0)
            {
                header = tail = newNode;
            }
            ++Length;
        }

        /// <summary>
        /// 创建结点
        /// </summary>
        /// <returns></returns>
        private QuickListNode MakeNode()
        {
            return new QuickListNode
            {
                List = new InternalList<TElement>(fill),
            };
        }

        /// <summary>
        /// 快速列表结点是否允许插入
        /// </summary>
        /// <param name="node">结点</param>
        /// <returns>是否可以插入</returns>
        private bool AllowInsert(QuickListNode node)
        {
            if (node == null)
            {
                return false;
            }
            return node.List.Count < fill;
        }
    }
}
