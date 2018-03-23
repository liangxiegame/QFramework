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
    /// 字典
    /// </summary>
    public static class Dict
    {
        /// <summary>
        /// 将输入字典中的每个值传给回调函数,如果回调函数返回 true，则把输入字典中的当前键值对加入结果字典中
        /// </summary>
        /// <typeparam name="TKey">字典键类型</typeparam>
        /// <typeparam name="TValue">字典值类型</typeparam>
        /// <param name="source">规定字典</param>
        /// <param name="predicate">回调函数</param>
        /// <returns>需求字典</returns>
        public static IDictionary<TKey, TValue> Filter<TKey, TValue>(IDictionary<TKey, TValue> source,
            Func<TKey, TValue, bool> predicate)
        {
            Guard.Requires<ArgumentNullException>(source != null);
            Guard.Requires<ArgumentNullException>(predicate != null);
            var elements = new Dictionary<TKey, TValue>();

            foreach (var result in source)
            {
                if (predicate.Invoke(result.Key, result.Value))
                {
                    elements[result.Key] = result.Value;
                }
            }

            return elements;
        }

        /// <summary>
        /// 将输入字典中的每个值传给回调函数，如果回调函数返回 true，则移除字典中对应的元素，并返回被移除的元素
        /// </summary>
        /// <typeparam name="TKey">字典键类型</typeparam>
        /// <typeparam name="TValue">字典值类型</typeparam>
        /// <param name="source">规定字典</param>
        /// <param name="predicate">回调函数</param>
        /// <return>被移除的元素</return>
        public static KeyValuePair<TKey,TValue>[] Remove<TKey, TValue>(IDictionary<TKey, TValue> source, Func<TKey, TValue, bool> predicate)
        {
            Guard.Requires<ArgumentNullException>(source != null);
            Guard.Requires<ArgumentNullException>(predicate != null);

            var results = new List<KeyValuePair<TKey, TValue>>();
            foreach (var result in source)
            {
                if (predicate.Invoke(result.Key, result.Value))
                {
                    results.Add(result);
                }
            }

            foreach (var result in results)
            {
                source.Remove(result.Key);
            }

            return results.ToArray();
        }

        /// <summary>
        /// 将输入字典中的每个值传给回调函数，回调函数的返回值用于修改元素的值
        /// </summary>
        /// <typeparam name="TKey">字典键类型</typeparam>
        /// <typeparam name="TValue">字典值类型</typeparam>
        /// <param name="source">规定字典</param>
        /// <param name="callback">回调函数</param>
        public static void Modify<TKey, TValue>(IDictionary<TKey, TValue> source, Func<TKey, TValue, TValue> callback)
        {
            Guard.Requires<ArgumentNullException>(source != null);
            Guard.Requires<ArgumentNullException>(callback != null);

            Dictionary<TKey, TValue> elements = null;
            foreach (var result in source)
            {
                var value = callback.Invoke(result.Key, result.Value);
                if (!result.Equals(value))
                {
                    elements = elements ?? new Dictionary<TKey, TValue>();
                    elements[result.Key] = value;
                }
            }

            foreach (var result in elements)
            {
                source[result.Key] = result.Value;
            }
        }

        /// <summary>
        /// 将元素批量添加到字典
        /// </summary>
        /// <typeparam name="TKey">字典键</typeparam>
        /// <typeparam name="TValue">字典值</typeparam>
        /// <param name="source">目标字典</param>
        /// <param name="added">增加的内容</param>
        /// <param name="replaced">遇到重复是否替换，如果不进行替换遇到重复将会抛出一个异常</param>
        public static void AddRange<TKey, TValue>(IDictionary<TKey, TValue> source, IDictionary<TKey, TValue> added, bool replaced = true)
        {
            Guard.Requires<ArgumentNullException>(source != null);
            if (added == null)
            {
                return;
            }

            foreach (var item in added)
            {
                if (replaced)
                {
                    source[item.Key] = item.Value;
                }
                else
                {
                    source.Add(item.Key, item.Value);
                }
            }
        }

        /// <summary>
        /// 将字典值传入用户自定义函数，自定义函数返回的值作为新的字典值
        /// </summary>
        /// <typeparam name="TKey">字典键类型</typeparam>
        /// <typeparam name="TValue">字典值类型</typeparam>
        /// <param name="source">规定字典</param>
        /// <param name="callback">自定义函数</param>
        /// <returns>处理后的字典</returns>
        public static IDictionary<TKey, TValue> Map<TKey, TValue>(IDictionary<TKey, TValue> source, Func<TKey, TValue, TValue> callback)
        {
            Guard.Requires<ArgumentNullException>(source != null);
            Guard.Requires<ArgumentNullException>(callback != null);

            var requested = new Dictionary<TKey, TValue>();
            foreach (var result in source)
            {
                requested[result.Key] = callback.Invoke(result.Key, result.Value);
            }

            return requested;
        }

        /// <summary>
        /// 获取字典的键数组
        /// </summary>
        /// <typeparam name="TKey">字典键类型</typeparam>
        /// <typeparam name="TValue">字典值类型</typeparam>
        /// <param name="source">规定字典</param>
        /// <returns>字典的键数组</returns>
        public static TKey[] Keys<TKey, TValue>(IDictionary<TKey, TValue> source)
        {
            Guard.Requires<ArgumentNullException>(source != null);

            var keys = new TKey[source.Count];
            var i = 0;
            foreach (var item in source)
            {
                keys[i++] = item.Key;
            }

            return keys;
        }

        /// <summary>
        /// 获取字典的值数组
        /// </summary>
        /// <typeparam name="TKey">字典键类型</typeparam>
        /// <typeparam name="TValue">字典值类型</typeparam>
        /// <param name="source">规定字典</param>
        /// <returns>字典的值数组</returns>
        public static TValue[] Values<TKey, TValue>(IDictionary<TKey, TValue> source)
        {
            Guard.Requires<ArgumentNullException>(source != null);

            var keys = new TValue[source.Count];
            var i = 0;
            foreach (var item in source)
            {
                keys[i++] = item.Value;
            }

            return keys;
        }

        /// <summary>
        /// 使用点（.）来访问深度字典
        /// </summary>
        /// <param name="dict">规定字典</param>
        /// <param name="key">键，支持使用点（.）来进行深度访问</param>
        /// <param name="def">默认值</param>
        /// <returns>字典值</returns>
        public static object Get(IDictionary<string, object> dict, string key, object def = null)
        {
            if (dict == null)
            {
                return def;
            }

            if (string.IsNullOrEmpty(key))
            {
                return dict;
            }

            var keyArr = Arr.Reverse(key.Split('.'));
            return GetValueByDepthArray(dict, ref keyArr) ?? def;
        }

        /// <summary>
        /// 使用点（.）来访问深度字典，并为其指定位置设定一个值
        /// </summary>
        /// <param name="dict">规定字典</param>
        /// <param name="key">键，支持使用点（.）来进行深度访问</param>
        /// <param name="val">设定的值</param>
        public static void Set(IDictionary<string, object> dict, string key, object val)
        {
            Guard.Requires<ArgumentNullException>(dict != null);
            Guard.Requires<ArgumentNullException>(key != null);

            var keyArr = Arr.Reverse(key.Split('.'));
            SetValueByDepthArray(dict, ref keyArr, val);
        }

        /// <summary>
        /// 使用点（.）来访问深度字典，并移除其中指定的值
        /// </summary>
        /// <param name="dict">规定字典</param>
        /// <param name="key">键，支持使用点（.）来进行深度访问</param>
        public static bool Remove(IDictionary<string, object> dict, string key)
        {
            Guard.Requires<ArgumentNullException>(dict != null);
            Guard.Requires<ArgumentNullException>(key != null);

            var keyArr = Arr.Reverse(key.Split('.'));
            return RemoveValueByDepthArray(dict, ref keyArr);
        }

        /// <summary>
        /// 通过深度数组来访问字典
        /// </summary>
        /// <param name="dict">规定字典</param>
        /// <param name="keys">深度数组（深度数组以倒序传入）</param>
        /// <returns>字典值</returns>
        private static object GetValueByDepthArray(IDictionary<string, object> dict, ref string[] keys)
        {
            while (true)
            {
                object result;
                if (!dict.TryGetValue(Arr.Pop(ref keys), out result) || keys.Length <= 0)
                {
                    return result;
                }

                dict = result as IDictionary<string, object>;
                if (dict == null)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 通过深度数组来访问字典，并为其指定位置设定一个值
        /// </summary>
        /// <param name="dict">规定字典</param>
        /// <param name="keys">深度数组（深度数组以倒序传入）</param>
        /// <param name="value">设定值</param>
        private static void SetValueByDepthArray(IDictionary<string, object> dict, ref string[] keys, object value)
        {
            while (true)
            {
                if (keys.Length <= 1)
                {
                    dict[Arr.Pop(ref keys)] = value;
                    return;
                }

                object result;
                var key = Arr.Pop(ref keys);
                if (!dict.TryGetValue(key, out result) || !(result is IDictionary<string, object>))
                {
                    dict[key] = result = new Dictionary<string, object>();
                }

                dict = (IDictionary<string, object>)result;
            }
        }

        /// <summary>
        /// 通过深度数组来移除数组中的一个值
        /// </summary>
        /// <param name="dict">规定字典</param>
        /// <param name="keys">深度数组（深度数组以倒序传入）</param>
        private static bool RemoveValueByDepthArray(IDictionary<string, object> dict, ref string[] keys)
        {
            var perv = new Stack<KeyValuePair<string, IDictionary<string, object>>>(keys.Length);
            while (true)
            {
                if (keys.Length <= 1)
                {
                    dict.Remove(Arr.Pop(ref keys));
                    while (perv.Count > 0)
                    {
                        var data = perv.Pop();
                        var tmpDict = (IDictionary<string, object>)data.Value[data.Key];
                        if (tmpDict.Count <= 0)
                        {
                            data.Value.Remove(data.Key);
                            continue;
                        }
                        break;
                    }
                    return true;
                }

                object result;
                var key = Arr.Pop(ref keys);
                if (!dict.TryGetValue(key, out result) || !(result is IDictionary<string, object>))
                {
                    return false;
                }

                perv.Push(new KeyValuePair<string, IDictionary<string, object>>(key, dict));
                dict = (IDictionary<string, object>)result;
            }
        }
    }
}
