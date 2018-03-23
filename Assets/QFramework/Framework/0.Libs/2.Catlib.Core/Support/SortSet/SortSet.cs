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
    /// 有序集
    /// 有序集使用分数进行排序(以小到大)
    /// </summary>
    [DebuggerDisplay("Count = {Count}")]
    public sealed class SortSet<TElement, TScore> : ISortSet<TElement, TScore>
        where TScore : IComparable<TScore>
    {
        /// <summary>
        /// 跳跃结点
        /// </summary>
        private class SkipNode
        {
            internal struct SkipNodeLevel
            {
                /// <summary>
                /// 前一个结点
                /// </summary>
                internal SkipNode Forward;

                /// <summary>
                /// 层跨越的结点数量
                /// </summary>
                internal int Span;
            }

            /// <summary>
            /// 元素
            /// </summary>
            internal TElement Element;

            /// <summary>
            /// 分数
            /// </summary>
            internal TScore Score;

            /// <summary>
            /// 是否被删除
            /// </summary>
            internal bool IsDeleted;

            /// <summary>
            /// 向后的结点
            /// </summary>
            internal SkipNode Backward;

            /// <summary>
            /// 层级
            /// </summary>
            internal SkipNodeLevel[] Level;
        }

        /// <summary>
        /// 有序集迭代器
        /// </summary>
        public struct Enumerator : IEnumerator<TElement>
        {
            /// <summary>
            /// 快速列表
            /// </summary>
            private readonly SortSet<TElement, TScore> sortSet;

            /// <summary>
            /// 是否是向前遍历
            /// </summary>
            private readonly bool forward;

            /// <summary>
            /// 当前节点
            /// </summary>
            private SkipNode current;

            /// <summary>
            /// 构造一个迭代器
            /// </summary>
            /// <param name="sortSet">有序集</param>
            /// <param name="forward">是否向前遍历</param>
            internal Enumerator(SortSet<TElement, TScore> sortSet, bool forward)
            {
                this.sortSet = sortSet;
                this.forward = forward;
                current = forward ? sortSet.header : null;
            }

            /// <summary>
            /// 移动到下一个节点
            /// </summary>
            /// <returns>下一个节点是否存在</returns>
            public bool MoveNext()
            {
                if (forward)
                {
                    do
                    {
                        current = current.Level[0].Forward;
                    } while (current != null && current.IsDeleted);
                    return current != null;
                }

                if (current == null)
                {
                    do
                    {
                        current = sortSet.tail;
                    } while (current != null && current.IsDeleted);
                    return current != null;
                }

                do
                {
                    current = current.Backward;
                } while (current != null && current.IsDeleted);
                return current != null;
            }

            /// <summary>
            /// 获取当前元素
            /// </summary>
            public TElement Current
            {
                get
                {
                    if (current != null)
                    {
                        return current.Element;
                    }
                    throw new RuntimeException("Can not get Current element");
                }
            }

            /// <summary>
            /// 获取当前元素
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    if (current != null)
                    {
                        return current.Element;
                    }
                    throw new RuntimeException("Can not get Current element");
                }
            }

            /// <summary>
            /// 重置迭代器
            /// </summary>
            void IEnumerator.Reset()
            {
                current = forward ? sortSet.header : null;
            }

            /// <summary>
            /// 释放时
            /// </summary>
            public void Dispose()
            {
            }
        }

        /// <summary>
        /// 同步锁
        /// </summary>
        private readonly object syncRoot = new object();

        /// <summary>
        /// 是否是向前的迭代方向
        /// </summary>
        private bool forward;

        /// <summary>
        /// 最大层数
        /// </summary>
        private readonly int maxLevel;

        /// <summary>
        /// 字典
        /// </summary>
        private readonly Dictionary<TElement, TScore> dict = new Dictionary<TElement, TScore>();

        /// <summary>
        /// 当前拥有的层
        /// </summary>
        private int level;

        /// <summary>
        /// 跳跃表头结点
        /// </summary>
        private readonly SkipNode header;

        /// <summary>
        /// 尾部结点
        /// </summary>
        private SkipNode tail;

        /// <summary>
        /// 可能出现层数的概率
        /// </summary>
        private readonly double probability;

        /// <summary>
        /// 随机数发生器
        /// </summary>
        private readonly System.Random random = new System.Random();

        /// <summary>
        /// 有序集的基数
        /// </summary>
        public int Count { get; private set; }

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
        /// 创建一个有序集
        /// </summary>
        /// <param name="probable">可能出现层数的概率系数(0-1之间的数)</param>
        /// <param name="maxLevel">最大层数</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="probable"/>或<paramref name="maxLevel"/>不是有效值时引发</exception>
        public SortSet(double probable = 0.25, int maxLevel = 32)
        {
            Guard.Requires<ArgumentOutOfRangeException>(maxLevel > 0);
            Guard.Requires<ArgumentOutOfRangeException>(probable < 1);
            Guard.Requires<ArgumentOutOfRangeException>(probable > 0);

            forward = true;
            probability = probable * 0xFFFF;
            this.maxLevel = maxLevel;
            level = 1;
            header = new SkipNode
            {
                Level = new SkipNode.SkipNodeLevel[maxLevel]
            };
        }

        /// <summary>
        /// 清空SortSet
        /// </summary>
        public void Clear()
        {
            for (var i = 0; i < header.Level.Length; ++i)
            {
                header.Level[i].Span = 0;
                header.Level[i].Forward = null;
            }
            tail = null;
            level = 1;
            dict.Clear();
            Count = 0;
        }

        /// <summary>
        /// 反转遍历顺序(并不是反转整个有序集)
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
            return new Enumerator(this , forward);
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
        /// 转为数组
        /// </summary>
        /// <returns>数组</returns>
        public TElement[] ToArray()
        {
            var elements = new TElement[Count];
            var node = header.Level[0];
            int i = 0;
            while (node.Forward != null)
            {
                elements[i++] = node.Forward.Element;
                node = node.Forward.Level[0];
            }
            return elements;
        }

        /// <summary>
        /// 获取第一个元素
        /// </summary>
        /// <returns>最后一个元素</returns>
        public TElement First()
        {
            if (header.Level[0].Forward != null)
            {
                return header.Level[0].Forward.Element;
            }
            throw new InvalidOperationException("SortSet is Null");
        }

        /// <summary>
        /// 获取最后一个元素
        /// </summary>
        /// <returns>元素</returns>
        public TElement Last()
        {
            if (tail != null)
            {
                return tail.Element;
            }
            throw new InvalidOperationException("SortSet is Null");
        }

        /// <summary>
        /// 移除并返回有序集头部的元素
        /// </summary>
        /// <returns>元素</returns>
        public TElement Shift()
        {
            TElement result;
            if (!Remove(header.Level[0].Forward, out result))
            {
                throw new InvalidOperationException("SortSet is Null");
            }
            return result;
        }

        /// <summary>
        /// 移除并返回有序集尾部的元素
        /// </summary>
        /// <returns>元素</returns>
        public TElement Pop()
        {
            TElement result;
            if (!Remove(tail, out result))
            {
                throw new InvalidOperationException("SortSet is Null");
            }
            return result;
        }

        /// <summary>
        /// 获取指定排名的元素(有序集成员按照Score从小到大排序)
        /// </summary>
        /// <param name="rank">排名,排名以0为底</param>
        /// <returns>指定的元素</returns>
        public TElement this[int rank]
        {
            get { return GetElementByRank(rank); }
        }

        /// <summary>
        /// 插入记录
        /// </summary>
        /// <param name="element">元素</param>
        /// <param name="score">分数</param>
        /// <exception cref="ArgumentNullException"><paramref name="element"/>或<paramref name="score"/>为<c>null</c>时引发</exception>
        public void Add(TElement element, TScore score)
        {
            Guard.Requires<ArgumentNullException>(element != null);
            Guard.Requires<ArgumentNullException>(score != null);

            //已经存在的元素先移除再添加
            TScore dictScore;
            if (dict.TryGetValue(element, out dictScore))
            {
                Remove(element, dictScore);
            }
            AddElement(element, score);
        }

        /// <summary>
        /// 是否包含某个元素
        /// </summary>
        /// <param name="element">元素</param>
        /// <exception cref="ArgumentNullException"><paramref name="element"/>为<c>null</c>时引发</exception>
        public bool Contains(TElement element)
        {
            Guard.Requires<ArgumentNullException>(element != null);
            return dict.ContainsKey(element);
        }

        /// <summary>
        /// 返回有序集的分数
        /// </summary>
        /// <param name="element">元素</param>
        /// <returns>分数，如果元素不存在则引发异常</returns>
        /// <exception cref="KeyNotFoundException"><paramref name="element"/>不存在时引发</exception>
        /// <exception cref="ArgumentNullException"><paramref name="element"/>为<c>null</c>时引发</exception>
        public TScore GetScore(TElement element)
        {
            Guard.Requires<ArgumentNullException>(element != null);
            TScore score;
            if (!dict.TryGetValue(element, out score))
            {
                throw new KeyNotFoundException();
            }
            return score;
        }

        /// <summary>
        /// 获取分数范围内的元素个数
        /// </summary>
        /// <param name="start">起始值(包含)</param>
        /// <param name="end">结束值(包含)</param>
        /// <returns>分数值在<paramref name="start"/>(包含)和<paramref name="end"/>(包含)之间的元素数量</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="start"/>和<paramref name="end"/>区间无效时引发</exception>
        public int GetRangeCount(TScore start, TScore end)
        {
            Guard.Requires<ArgumentNullException>(start != null);
            Guard.Requires<ArgumentNullException>(end != null);
            Guard.Requires<ArgumentOutOfRangeException>(start.CompareTo(end) <= 0);

            int rank = 0, bakRank = 0;
            SkipNode bakCursor = null;

            var isRight = false;
            var cursor = header;

            do
            {
                for (var i = level - 1; i >= 0; --i)
                {
                    while (cursor.Level[i].Forward != null &&
                           ((!isRight && cursor.Level[i].Forward.Score.CompareTo(start) < 0) ||
                            (isRight && cursor.Level[i].Forward.Score.CompareTo(end) <= 0)))
                    {
                        rank += cursor.Level[i].Span;
                        cursor = cursor.Level[i].Forward;
                    }
                    if (bakCursor == null)
                    {
                        bakCursor = cursor;
                        bakRank = rank;
                    }
                }

                if (!isRight)
                {
                    cursor = bakCursor;
                    rank ^= bakRank ^= rank ^= bakRank;
                }

            } while (isRight = !isRight);

            return Math.Max(0, rank - bakRank);
        }

        /// <summary>
        /// 从有序集中删除元素，如果元素不存在返回false
        /// </summary>
        /// <param name="element">元素</param>
        /// <returns>是否成功</returns>
        /// <exception cref="ArgumentNullException"><paramref name="element"/>为<c>null</c>时引发</exception>
        public bool Remove(TElement element)
        {
            Guard.Requires<ArgumentNullException>(element != null);

            TScore dictScore;
            return dict.TryGetValue(element, out dictScore) && Remove(element, dictScore);
        }

        /// <summary>
        /// 根据排名区间移除区间内的元素
        /// </summary>
        /// <param name="startRank">开始的排名(包含),排名以0为底</param>
        /// <param name="stopRank">结束的排名(包含),排名以0为底</param>
        /// <returns>被删除的元素个数</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startRank"/>和<paramref name="stopRank"/>区间无效时引发</exception>
        public int RemoveRangeByRank(int startRank, int stopRank)
        {
            startRank = Math.Max(startRank, 0);
            Guard.Requires<ArgumentOutOfRangeException>(startRank <= stopRank);

            int traversed = 0, removed = 0;
            var update = new SkipNode[maxLevel];
            var cursor = header;
            for (var i = level - 1; i >= 0; --i)
            {
                while (cursor.Level[i].Forward != null &&
                       (traversed + cursor.Level[i].Span <= startRank))
                {
                    traversed += cursor.Level[i].Span;
                    cursor = cursor.Level[i].Forward;
                }
                update[i] = cursor;
            }

            cursor = cursor.Level[0].Forward;

            while (cursor != null &&
                    traversed <= stopRank)
            {
                var next = cursor.Level[0].Forward;
                dict.Remove(cursor.Element);
                DeleteNode(cursor, update);
                ++removed;
                ++traversed;
                cursor = next;
            }

            return removed;
        }

        /// <summary>
        /// 根据分数区间移除区间内的元素
        /// </summary>
        /// <param name="startScore">开始的分数（包含）</param>
        /// <param name="stopScore">结束的分数（包含）</param>
        /// <returns>被删除的元素个数</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startScore"/>和<paramref name="stopScore"/>区间无效时引发</exception>
        public int RemoveRangeByScore(TScore startScore, TScore stopScore)
        {
            Guard.Requires<ArgumentNullException>(startScore != null);
            Guard.Requires<ArgumentNullException>(stopScore != null);
            Guard.Requires<ArgumentOutOfRangeException>(startScore.CompareTo(stopScore) <= 0);

            int removed = 0;
            var update = new SkipNode[maxLevel];
            var cursor = header;
            for (var i = level - 1; i >= 0; --i)
            {
                while (cursor.Level[i].Forward != null &&
                       cursor.Level[i].Forward.Score.CompareTo(startScore) < 0)
                {
                    cursor = cursor.Level[i].Forward;
                }
                update[i] = cursor;
            }

            cursor = cursor.Level[0].Forward;

            while (cursor != null &&
                   cursor.Score.CompareTo(stopScore) <= 0)
            {
                var next = cursor.Level[0].Forward;
                dict.Remove(cursor.Element);
                DeleteNode(cursor, update);
                ++removed;
                cursor = next;
            }

            return removed;
        }

        /// <summary>
        /// 获取排名 , 有序集成员按照Score从小到大排序
        /// </summary>
        /// <param name="element">元素</param>
        /// <returns>排名排名以0为底，为-1则表示没有找到元素</returns>
        /// <exception cref="ArgumentNullException"><paramref name="element"/>为<c>null</c>时引发</exception>
        public int GetRank(TElement element)
        {
            Guard.Requires<ArgumentNullException>(element != null);
            TScore dictScore;
            return dict.TryGetValue(element, out dictScore) ? GetRank(element, dictScore) : -1;
        }

        /// <summary>
        /// 获取排名，有序集成员按照Score从大到小排序
        /// </summary>
        /// <param name="element"></param>
        /// <returns>排名排名以0为底 , 为-1则表示没有找到元素</returns>
        /// <exception cref="ArgumentNullException"><paramref name="element"/>为<c>null</c>时引发</exception>
        public int GetRevRank(TElement element)
        {
            Guard.Requires<ArgumentNullException>(element != null);
            var rank = GetRank(element);
            return rank < 0 ? rank : Count - rank - 1;
        }

        /// <summary>
        /// 根据排名区间获取区间内的所有元素
        /// </summary>
        /// <param name="startRank">开始的排名(包含),排名以0为底</param>
        /// <param name="stopRank">结束的排名(包含),排名以0为底</param>
        /// <returns>元素列表</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startRank"/>和<paramref name="stopRank"/>区间无效时引发</exception>
        public TElement[] GetElementRangeByRank(int startRank, int stopRank)
        {
            startRank = Math.Max(startRank, 0);
            Guard.Requires<ArgumentOutOfRangeException>(startRank <= stopRank);

            int traversed = 0;
            var cursor = header;
            for (var i = level - 1; i >= 0; --i)
            {
                while (cursor.Level[i].Forward != null &&
                       (traversed + cursor.Level[i].Span <= startRank))
                {
                    traversed += cursor.Level[i].Span;
                    cursor = cursor.Level[i].Forward;
                }
            }

            cursor = cursor.Level[0].Forward;

            var result = new List<TElement>();
            while (cursor != null &&
                   traversed <= stopRank)
            {
                result.Add(cursor.Element);
                ++traversed;
                cursor = cursor.Level[0].Forward;
            }

            return result.ToArray();
        }

        /// <summary>
        /// 根据分数区间获取区间内的所有元素
        /// </summary>
        /// <param name="startScore">开始的分数（包含）</param>
        /// <param name="stopScore">结束的分数（包含）</param>
        /// <returns>元素列表</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="startScore"/>和<paramref name="stopScore"/>区间无效时引发</exception>
        public TElement[] GetElementRangeByScore(TScore startScore, TScore stopScore)
        {
            Guard.Requires<ArgumentNullException>(startScore != null);
            Guard.Requires<ArgumentNullException>(stopScore != null);
            Guard.Requires<ArgumentOutOfRangeException>(startScore.CompareTo(stopScore) <= 0);

            var cursor = header;
            for (var i = level - 1; i >= 0; --i)
            {
                while (cursor.Level[i].Forward != null &&
                       cursor.Level[i].Forward.Score.CompareTo(startScore) < 0)
                {
                    cursor = cursor.Level[i].Forward;
                }
            }

            cursor = cursor.Level[0].Forward;

            var result = new List<TElement>();
            while (cursor != null &&
                   cursor.Score.CompareTo(stopScore) <= 0)
            {
                result.Add(cursor.Element);
                cursor = cursor.Level[0].Forward;
            }

            return result.ToArray();
        }

        /// <summary>
        /// 根据排名获取元素 (有序集成员按照Score从小到大排序)
        /// </summary>
        /// <param name="rank">排名,排名以0为底</param>
        /// <returns>元素</returns>
        public TElement GetElementByRank(int rank)
        {
            rank = Math.Max(0, rank);
            rank += 1;
            int traversed = 0;
            var cursor = header;
            for (var i = level - 1; i >= 0; i--)
            {
                while (cursor.Level[i].Forward != null &&
                       (traversed + cursor.Level[i].Span) <= rank)
                {
                    traversed += cursor.Level[i].Span;
                    cursor = cursor.Level[i].Forward;
                }
                if (traversed == rank)
                {
                    return cursor.Element;
                }
            }
            if (Count > 0)
            {
                throw new ArgumentOutOfRangeException("Rank is out of range [" + rank + "]");
            }
            throw new InvalidOperationException("SortSet is Null");
        }

        /// <summary>
        /// 根据排名获取元素 (有序集成员按照Score从大到小排序)
        /// </summary>
        /// <param name="rank">排名,排名以0为底</param>
        /// <returns>元素</returns>
        public TElement GetElementByRevRank(int rank)
        {
            return GetElementByRank(Count - rank - 1);
        }

        /// <summary>
        /// 插入记录
        /// </summary>
        /// <param name="element">元素</param>
        /// <param name="score">分数</param>
        private void AddElement(TElement element, TScore score)
        {
            int i;
            dict.Add(element, score);

            var update = new SkipNode[maxLevel];
            var cursor = header;
            var rank = new int[maxLevel];
            //从跳跃层高到低的进行查找
            for (i = level - 1; i >= 0; --i)
            {
                //rank为上一级结点的跨度数作为起点
                rank[i] = i == (level - 1) ? 0 : rank[i + 1];
                while (cursor.Level[i].Forward != null &&
                        (cursor.Level[i].Forward.Score.CompareTo(score) < 0))
                {
                    rank[i] += cursor.Level[i].Span;
                    cursor = cursor.Level[i].Forward;
                }

                //将找到的最后一个结点置入需要更新的结点
                update[i] = cursor;
            }

            //随机获取层数
            var newLevel = GetRandomLevel();

            //如果随机出的层数大于现有层数，那么将新增的需要更新的结点初始化
            if (newLevel > level)
            {
                for (i = level; i < newLevel; ++i)
                {
                    rank[i] = 0;
                    update[i] = header;
                    update[i].Level[i].Span = Count;
                }
                level = newLevel;
            }

            //将游标指向为新的跳跃结点
            cursor = new SkipNode
            {
                Element = element,
                Score = score,
                Level = new SkipNode.SkipNodeLevel[newLevel]
            };

            for (i = 0; i < newLevel; ++i)
            {
                //新增结点的跳跃目标为更新结点的跳跃目标
                cursor.Level[i].Forward = update[i].Level[i].Forward;
                //老的需要被更新的结点跳跃目标为新增结点
                update[i].Level[i].Forward = cursor;
                //更新跨度
                cursor.Level[i].Span = update[i].Level[i].Span - (rank[0] - rank[i]);
                update[i].Level[i].Span = (rank[0] - rank[i]) + 1;
            }

            //将已有的层中的跨度 + 1
            for (i = newLevel; i < level; ++i)
            {
                ++update[i].Level[i].Span;
            }

            //给与上一个的结点
            cursor.Backward = (update[0] == header) ? null : update[0];

            if (cursor.Level[0].Forward != null)
            {
                cursor.Level[0].Forward.Backward = cursor;
            }
            else
            {
                tail = cursor;
            }

            ++Count;
        }

        /// <summary>
        /// 移除元素
        /// </summary>
        /// <param name="node">节点</param>
        /// <param name="element">元素</param>
        /// <returns>移除的元素</returns>
        private bool Remove(SkipNode node, out TElement element)
        {
            if (node == null)
            {
                element = default(TElement);
                return false;
            }
            var result = node.Element;
            if (!Remove(node.Element, node.Score))
            {
                element = default(TElement);
                return false;
            }
            element = result;
            return true;
        }

        /// <summary>
        /// 如果元素存在那么从有序集中删除元素
        /// </summary>
        /// <param name="element">元素</param>
        /// <param name="score">分数</param>
        /// <returns>是否成功</returns>
        private bool Remove(TElement element, TScore score)
        {
            Guard.Requires<ArgumentNullException>(element != null);
            Guard.Requires<ArgumentNullException>(score != null);

            var update = new SkipNode[maxLevel];
            var cursor = header;

            //从跳跃层高到低的进行查找
            for (var i = level - 1; i >= 0; --i)
            {
                while (cursor.Level[i].Forward != null &&
                            (cursor.Level[i].Forward.Score.CompareTo(score) <= 0 &&
                                !cursor.Level[i].Forward.Element.Equals(element)))
                {
                    cursor = cursor.Level[i].Forward;
                }

                //将找到的最后一个结点置入需要更新的结点 
                update[i] = cursor;
            }

            cursor = update[0].Level[0].Forward;

            if (cursor == null ||
                    cursor.Score.CompareTo(score) != 0 ||
                        !cursor.Element.Equals(element))
            {
                return false;
            }

            dict.Remove(element);
            DeleteNode(cursor, update);
            return true;
        }

        /// <summary>
        /// 获取元素排名
        /// </summary>
        /// <param name="element">元素</param>
        /// <param name="score">分数</param>
        /// <returns>排名，排名以0为底</returns>
        private int GetRank(TElement element, TScore score)
        {
            int rank = 0;
            var cursor = header;
            for (var i = level - 1; i >= 0; --i)
            {
                while (cursor.Level[i].Forward != null &&
                        (cursor.Level[i].Forward.Score.CompareTo(score) <= 0 &&
                            !cursor.Level[i].Forward.Equals(element)))
                {
                    rank += cursor.Level[i].Span;
                    cursor = cursor.Level[i].Forward;
                }
                if (cursor != header &&
                        cursor.Element != null &&
                            cursor.Element.Equals(element))
                {
                    return rank - 1;
                }
            }
            return -1;
        }

        /// <summary>
        /// 删除结点关系
        /// </summary>
        /// <param name="cursor">结点</param>
        /// <param name="update">更新结点列表</param>
        private void DeleteNode(SkipNode cursor, SkipNode[] update)
        {
            for (var i = 0; i < level; ++i)
            {
                if (update[i].Level[i].Forward == cursor)
                {
                    update[i].Level[i].Span += cursor.Level[i].Span - 1;
                    update[i].Level[i].Forward = cursor.Level[i].Forward;
                }
                else
                {
                    update[i].Level[i].Span -= 1;
                }
            }
            if (cursor.Level[0].Forward != null)
            {
                cursor.Level[0].Forward.Backward = cursor.Backward;
            }
            else
            {
                tail = cursor.Backward;
            }
            while (level > 1 && header.Level[level - 1].Forward == null)
            {
                --level;
            }
            cursor.IsDeleted = true;
            --Count;
        }

        /// <summary>
        /// 获取随机层
        /// </summary>
        /// <returns>随机的层数</returns>
        private int GetRandomLevel()
        {
            var newLevel = 1;
            while (random.Next(0, 0xFFFF) < probability)
            {
                ++newLevel;
            }
            return (newLevel < maxLevel) ? newLevel : maxLevel;
        }
    }
}