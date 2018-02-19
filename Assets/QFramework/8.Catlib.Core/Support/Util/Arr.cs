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
using System.Collections.Generic;

namespace CatLib
{
    /// <summary>
    /// 数组
    /// </summary>
    public static class Arr
    {
        /// <summary>
        /// 将多个规定数组合并成一个数组
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="sources">规定数组</param>
        /// <returns>合并后的数组</returns>
        public static T[] Merge<T>(params T[][] sources)
        {
            Guard.Requires<ArgumentNullException>(sources != null);
            var length = 0;
            foreach (var source in sources)
            {
                length += source.Length;
            }

            var merge = new T[length];
            var current = 0;
            foreach (var source in sources)
            {
                Array.Copy(source, 0, merge, current, source.Length);
                current += source.Length;
            }

            return merge;
        }

        /// <summary>
        /// 从规定数组中获取一个或者指定数量的随机值
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="source">规定数组</param>
        /// <param name="number">随机的数量</param>
        /// <returns>随机后的元素</returns>
        public static T[] Rand<T>(T[] source, int number = 1)
        {
            Guard.Requires<ArgumentNullException>(source != null);
            number = Math.Max(number, 1);
            source = Shuffle(source);
            var requested = new T[number];
            var i = 0;
            foreach (var result in source)
            {
                if (i >= number)
                {
                    break;
                }
                requested[i++] = result;
            }

            return requested;
        }

        /// <summary>
        /// 将规定数组中的元素打乱
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="source">规定数组</param>
        /// <param name="seed">种子</param>
        /// <returns>打乱后的数组</returns>
        public static T[] Shuffle<T>(T[] source, int? seed = null)
        {
            Guard.Requires<ArgumentNullException>(source != null);
            var requested = new T[source.Length];
            Array.Copy(source, requested, source.Length);

            var random = Util.MakeRandom(seed);
            for (var i = 0; i < requested.Length; i++)
            {
                var index = random.Next(0, requested.Length - 1);
                if (index == i)
                {
                    continue;
                }
                var temp = requested[i];
                requested[i] = requested[index];
                requested[index] = temp;
            }

            return requested;
        }

        /// <summary>
        /// 从数组中移除指定长度的元素，如果给定了<paramref name="replSource"/>参数，那么新元素从<paramref name="start"/>位置开始插入
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="source">规定数组</param>
        /// <param name="start">
        /// 删除元素的开始位置。
        /// <para>如果该值设置为正数，则从前往后开始删除</para>
        /// <para>如果该值设置为负数，则从后向前取 <paramref name="start"/> 绝对值。-2 意味着从数组的倒数第二个元素开始</para></param>
        /// <param name="length">
        /// 删除元素的个数，也是被返回数组的长度
        /// <para>如果该值设置为整数，则返回该数量的元素。</para>
        /// <para>如果该值设置为负数，则则从后向前取 <paramref name="length"/> 绝对值位置终止删除。-1 意味着从数组的倒数第一个元素前删除</para>
        /// <para>如果该值未设置，则返回从 <paramref name="start"/> 参数设置的位置开始直到数组末端的所有元素。</para>
        /// </param>
        /// <param name="replSource">在start位置插入的数组</param>
        /// <returns>被删除的数组</returns>
        public static T[] Splice<T>(ref T[] source, int start, int? length = null, T[] replSource = null)
        {
            Guard.Requires<ArgumentNullException>(source != null);

            Util.NormalizationPosition(source.Length, ref start, ref length);

            var requested = new T[length.Value];

            if (length.Value == source.Length)
            {
                // 现在移除所有旧的元素，然后用新的元素替换。
                Array.Copy(source, requested, source.Length);
                source = replSource ?? new T[] { };
                return requested;
            }

            Array.Copy(source, start, requested, 0, length.Value);

            if (replSource == null || replSource.Length == 0)
            {
                var newSource = new T[source.Length - length.Value];
                // 现在只删除不插入
                if (start > 0)
                {
                    Array.Copy(source, 0, newSource, 0, start);
                }
                Array.Copy(source, start + length.Value, newSource, start, source.Length - (start + length.Value));
                source = newSource;
            }
            else
            {
                var newSource = new T[source.Length - length.Value + replSource.Length];
                // 删除并且插入
                if (start > 0)
                {
                    Array.Copy(source, 0, newSource, 0, start);
                }
                Array.Copy(replSource, 0, newSource, start, replSource.Length);
                Array.Copy(source, start + length.Value, newSource, start + replSource.Length,
                    source.Length - (start + length.Value));
                source = newSource;
            }

            return requested;
        }

        /// <summary>
        /// 将数组分为新的数组块
        /// <para>其中每个数组的单元数目由 <paramref name="size"/> 参数决定。最后一个数组的单元数目可能会少几个。</para>
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="source">规定数组</param>
        /// <param name="size">每个分块的大小</param>
        /// <returns></returns>
        public static T[][] Chunk<T>(T[] source, int size)
        {
            Guard.Requires<ArgumentNullException>(source != null);
            size = Math.Max(1, size);
            var requested = new T[source.Length / size + (source.Length % size == 0 ? 0 : 1)][];

            T[] chunk = null;
            for (var i = 0; i < source.Length; i++)
            {
                var pos = i / size;
                if (i % size == 0)
                {
                    if (chunk != null)
                    {
                        requested[pos - 1] = chunk;
                    }
                    chunk = new T[(i + size) <= source.Length ? size : source.Length - i];
                }
                chunk[i - (pos * size)] = source[i];
            }
            requested[requested.Length - 1] = chunk;

            return requested;
        }

        /// <summary>
        /// 对数组进行填充，如果传入了规定数组，那么会在规定数组的基础上进行填充
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="start">起始下标</param>
        /// <param name="length">填充长度</param>
        /// <param name="value">填充的值</param>
        /// <param name="source">规定数组</param>
        /// <returns>填充后的数组</returns>
        public static T[] Fill<T>(int start, int length, T value, T[] source = null)
        {
            Guard.Requires<ArgumentOutOfRangeException>(start >= 0);
            Guard.Requires<ArgumentOutOfRangeException>(length > 0);
            var count = start + length;
            var requested = new T[source == null ? count : source.Length + count];

            if (start > 0 && source != null)
            {
                Array.Copy(source, requested, Math.Min(source.Length, start));
            }

            for (var i = start; i < count; i++)
            {
                requested[i] = value;
            }

            if (source != null && start < source.Length)
            {
                Array.Copy(source, start, requested, count, source.Length - start);
            }

            return requested;
        }

        /// <summary>
        /// 将数组每个值传给回调函数，如果回调函数返回 true，则移除数组中对应的元素，并返回被移除的元素
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="source">规定数组</param>
        /// <param name="predicate">回调函数</param>
        /// <returns>被移除的数组</returns>
        public static T[] Remove<T>(ref T[] source, Predicate<T> predicate)
        {
            Guard.Requires<ArgumentNullException>(source != null);
            Guard.Requires<ArgumentNullException>(predicate != null);

            if (source.Length <= 0)
            {
                return new T[] { };
            }

            var results = new List<T>();

            for (var i = source.Length - 1; i >= 0; i--)
            {
                if (!predicate.Invoke(source[i]))
                {
                    continue;
                }
                results.Add(source[i]);
                RemoveAt(ref source, i);
            }

            return Reverse(results.ToArray());
        }

        /// <summary>
        /// 输入数组中的每个值传给回调函数,如果回调函数返回 true，则把输入数组中的当前值加入结果数组中
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="source">规定数组</param>
        /// <param name="predicate">回调函数</param>
        /// <returns>需求数组</returns>
        public static T[] Filter<T>(T[] source, Predicate<T> predicate)
        {
            Guard.Requires<ArgumentNullException>(source != null);
            Guard.Requires<ArgumentNullException>(predicate != null);
            var elements = new T[source.Length];

            var i = 0;
            foreach (var result in source)
            {
                if (predicate.Invoke(result))
                {
                    elements[i++] = result;
                }
            }

            Array.Resize(ref elements, i);
            return elements;
        }

        /// <summary>
        /// 将数组值传入用户自定义函数，自定义函数返回的值作为新的数组值
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="source">规定数组</param>
        /// <param name="callback">自定义函数</param>
        /// <returns>处理后的数组</returns>
        public static T[] Map<T>(T[] source, Func<T, T> callback)
        {
            Guard.Requires<ArgumentNullException>(source != null);
            Guard.Requires<ArgumentNullException>(callback != null);

            var requested = new T[source.Length];
            Array.Copy(source, requested, source.Length);

            for (var i = 0; i < source.Length; i++)
            {
                requested[i] = callback.Invoke(source[i]);
            }

            return requested;
        }

        /// <summary>
        /// 删除数组中的最后一个元素，并将删除的元素作为返回值返回
        /// </summary>
        /// <typeparam name="T">删除数组中的最后一个元素</typeparam>
        /// <param name="source">规定数组</param>
        /// <returns>被删除的元素</returns>
        public static T Pop<T>(ref T[] source)
        {
            Guard.Requires<ArgumentNullException>(source != null);
            Guard.Requires<InvalidOperationException>(source.Length > 0);

            T result = source[source.Length - 1];
            Array.Resize(ref source, source.Length - 1);
            return result;
        }

        /// <summary>
        /// 将一个或多个元素加入数组尾端
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="source">规定数组</param>
        /// <param name="elements">要加入的元素</param>
        /// <returns>数组的元素个数</returns>
        public static int Push<T>(ref T[] source, params T[] elements)
        {
            Guard.Requires<ArgumentNullException>(source != null);
            Guard.Requires<InvalidOperationException>(elements != null);

            Array.Resize(ref source, source.Length + elements.Length);
            Array.Copy(elements, 0, source, source.Length - elements.Length, elements.Length);

            return source.Length;
        }

        /// <summary>
        /// 向用户自定义函数发送数组中的值，并返回一个字符串
        /// <para>如果数组是空的且未传递<paramref name="initial"/>参数，该函数返回 null</para>
        /// <para>如果指定了<paramref name="initial"/>参数，则该参数将被当成是数组中的第一个值来处理，如果数组为空的话就作为最终返回值(string)</para>
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="source">规定数组</param>
        /// <param name="callback">自定义函数</param>
        /// <param name="initial">初始值</param>
        /// <returns></returns>
        public static string Reduce<T>(T[] source, Func<object, T, string> callback, object initial = null)
        {
            Guard.Requires<ArgumentNullException>(source != null);
            Guard.Requires<ArgumentNullException>(callback != null);

            var requested = initial;
            foreach (var segments in source)
            {
                requested = callback.Invoke(requested, segments);
            }
            return requested == null ? null : requested.ToString();
        }

        /// <summary>
        /// 在数组中根据条件取出一段值，并返回。
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="source">规定数组</param>
        /// <param name="start">
        /// 取出元素的开始位置。
        /// <para>如果该值设置为正数，则从前往后开始取</para>
        /// <para>如果该值设置为负数，则从后向前取 <paramref name="start"/> 绝对值。-2 意味着从数组的倒数第二个元素开始</para>
        /// </param>
        /// <param name="length">
        /// 被返回数组的长度
        /// <para>如果该值设置为整数，则返回该数量的元素。</para>
        /// <para>如果该值设置为负数，则则从后向前取 <paramref name="length"/> 绝对值位置终止取出。-1 意味着从数组的倒数第一个元素前终止</para>
        /// <para>如果该值未设置，则返回从 <paramref name="start"/> 参数设置的位置开始直到数组末端的所有元素。</para>
        /// </param>
        /// <returns>取出的数组</returns>
        public static T[] Slice<T>(T[] source, int start, int? length = null)
        {
            Guard.Requires<ArgumentNullException>(source != null);

            Util.NormalizationPosition(source.Length, ref start, ref length);

            var requested = new T[length.Value];
            Array.Copy(source, start, requested, 0, length.Value);

            return requested;
        }

        /// <summary>
        /// 删除数组中第一个元素，并返回被删除元素的值
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="source">规定数组</param>
        /// <returns>被删除元素的值</returns>
        public static T Shift<T>(ref T[] source)
        {
            Guard.Requires<ArgumentNullException>(source != null);
            Guard.Requires<InvalidOperationException>(source.Length > 0);

            var requested = source[0];
            var newSource = new T[source.Length - 1];

            Array.Copy(source, 1, newSource, 0, source.Length - 1);
            source = newSource;

            return requested;
        }

        /// <summary>
        /// 向数组插入新元素。新数组的值将被插入到数组的开头。
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="source">规定数组</param>
        /// <param name="elements">插入的元素</param>
        /// <returns>数组元素个数</returns>
        public static int Unshift<T>(ref T[] source, params T[] elements)
        {
            Guard.Requires<ArgumentNullException>(source != null);
            Guard.Requires<ArgumentNullException>(elements != null);

            var newSource = new T[source.Length + elements.Length];

            Array.Copy(elements, newSource, elements.Length);
            Array.Copy(source, 0, newSource, elements.Length, source.Length);

            source = newSource;

            return source.Length;
        }

        /// <summary>
        /// 以相反的顺序返回数组
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="source">规定数组</param>
        /// <param name="start">
        /// 起始元素的开始位置。
        /// <para>如果该值设置为正数，则从前往后开始取</para>
        /// <para>如果该值设置为负数，则从后向前取 <paramref name="start"/> 绝对值。-2 意味着从数组的倒数第二个元素开始</para></param>
        /// <param name="length">
        /// 被返回数组的长度
        /// <para>如果该值设置为整数，则返回该数量的元素。</para>
        /// <para>如果该值设置为负数，则则从后向前取 <paramref name="length"/> 绝对值位置终止取出。-1 意味着从数组的倒数第一个元素前终止</para>
        /// <para>如果该值未设置，则返回从 <paramref name="start"/> 参数设置的位置开始直到数组末端的所有元素。</para>
        /// </param>
        /// <returns>反转的数组</returns>
        public static T[] Reverse<T>(T[] source, int start = 0, int? length = null)
        {
            Guard.Requires<ArgumentNullException>(source != null);

            if (source.Length == 1)
            {
                return source;
            }

            Util.NormalizationPosition(source.Length, ref start, ref length);
            var tmpSource = new T[source.Length];
            Array.Copy(source, tmpSource, source.Length);
            Array.Reverse(tmpSource, start, length.Value);

            var resquested = new T[length.Value];
            Array.Copy(tmpSource, start, resquested, 0, length.Value);
            return resquested;
        }

        /// <summary>
        /// 从数组中检索指定的值并返回所在的下标，如果返回-1则代表没有出现
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="source">规定数组</param>
        /// <param name="match">要匹配的值，如果有多个，只有全部的匹配才算匹配</param>
        /// <returns>如果要检索的值没有出现，则该方法返回 -1</returns>
        public static int IndexOf<T>(T[] source, params T[] match)
        {
            if (match == null || match.Length <= 0
                || source == null || source.Length <= 0)
            {
                return -1;
            }

            for (int i = 0, n; i < source.Length; i++)
            {
                if (!source[i].Equals(match[0]))
                {
                    continue;
                }
                var isFinded = true;
                for (n = 0; n < match.Length; n++)
                {
                    if ((i + n) < source.Length &&
                          source[i + n].Equals(match[n]))
                    {
                        continue;
                    }
                    isFinded = false;
                    break;
                }
                if (isFinded)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// 排除掉数组中的指定值
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="source">需要过滤的数组</param>
        /// <param name="match">数组需要排除掉的值</param>
        /// <returns>过滤后的数组</returns>
        public static T[] Difference<T>(T[] source, params T[] match)
        {
            Guard.Requires<ArgumentNullException>(source != null);
            if (match == null)
            {
                return source;
            }

            return Filter(source, (val) =>
            {
                foreach (var t in match)
                {
                    if (val.Equals(t))
                    {
                        return false;
                    }
                }
                return true;
            });
        }

        /// <summary>
        /// 移除并返回指定下标的数组元素
        /// <para>如果下标传入的是负数那么将会从末尾移除</para>
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="source">规定数组</param>
        /// <param name="index">数组下标</param>
        /// <returns>被移除的元素</returns>
        public static T RemoveAt<T>(ref T[] source, int index)
        {
            Guard.Requires<ArgumentNullException>(source != null);
            Guard.Requires<ArgumentNullException>(index < source.Length);

            var result = Splice(ref source, index, 1);
            return result.Length > 0 ? result[0] : default(T);
        }

        /// <summary>
        /// 临时性的回调元素，如果遇到异常或者完成回调后会进行回滚元素回调
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="source">规定数组</param>
        /// <param name="process">顺序回调</param>
        /// <param name="completed">所有回调完成后</param>
        /// <param name="rollback">回滚回调</param>
        public static void Flash<T>(T[] source, Action<T> process, Action<T> rollback, Action completed)
        {
            Guard.Requires<ArgumentNullException>(source != null);

            if (source.Length <= 0)
            {
                completed.Invoke();
                return;
            }

            var index = 0;
            try
            {
                foreach (var result in source)
                {
                    ++index;
                    process.Invoke(result);
                }

                completed();
            }
            finally
            {
                while (--index >= 0)
                {
                    rollback.Invoke(source[index]);
                }
            }
        }
    }
}