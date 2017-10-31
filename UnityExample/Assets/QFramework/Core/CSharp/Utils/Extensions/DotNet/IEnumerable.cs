/****************************************************************************
 * Copyright (c) 2017 liangxie
 * 
 * http://liangxiegame.com
 * https://github.com/liangxiegame/QFramework
 * https://github.com/liangxiegame/QSingleton
 * https://github.com/liangxiegame/QChain
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

namespace QFramework
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    /// <summary>
    /// IE numerable extension.
    /// </summary>
    public static class IEnumerableExtension
    {

        #region Array Extension

        /// <summary>
        /// Fors the each.
        /// </summary>
        /// <returns>The each.</returns>
        /// <param name="selfArray">Self array.</param>
        /// <param name="action">Action.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T[] ForEach<T>(this T[] selfArray, Action<T> action)
        {
            Array.ForEach(selfArray, action);
            return selfArray;
        }

        /// <summary>
        /// Fors the each.
        /// </summary>
        /// <returns>The each.</returns>
        /// <param name="selfArray">Self array.</param>
        /// <param name="action">Action.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> selfArray, Action<T> action)
        {
            if (action == null) throw new ArgumentException();
            foreach (var item in selfArray)
            {
                action(item);
            }

            return selfArray;
        }

        /// <summary>
        /// Has the specified selfList and match.
        /// </summary>
        /// <returns>The has.</returns>
        /// <param name="selfList">Self list.</param>
        /// <param name="match">Match.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static bool Has<T>(this List<T> selfList, Predicate<T> match)
        {
            return selfList.Find(match) != null;
        }

        #endregion

        #region List Extension

        /// <summary>
        /// Fors the each reverse.
        /// </summary>
        /// <returns>The each reverse.</returns>
        /// <param name="selfList">Self list.</param>
        /// <param name="action">Action.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static List<T> ForEachReverse<T>(this List<T> selfList, Action<T> action)
        {
            if (action == null) throw new ArgumentException();

            for (int i = selfList.Count - 1; i >= 0; --i)
                action(selfList[i]);

            return selfList;
        }

        #endregion


        #region Dictionary Extension

        /// <summary>
        /// Merge the specified dictionary and dictionaries.
        /// </summary>
        /// <returns>The merge.</returns>
        /// <param name="dictionary">Dictionary.</param>
        /// <param name="dictionaries">Dictionaries.</param>
        /// <typeparam name="TKey">The 1st type parameter.</typeparam>
        /// <typeparam name="TValue">The 2nd type parameter.</typeparam>
        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this Dictionary<TKey, TValue> dictionary,
            params Dictionary<TKey, TValue>[] dictionaries)
        {
            var merged = dictionary;
            foreach (var dict in dictionaries)
            {
                merged = merged.Union(dict).ToDictionary(kv => kv.Key, kv => kv.Value);
            }

            return merged;
        }
        #endregion
    }
}