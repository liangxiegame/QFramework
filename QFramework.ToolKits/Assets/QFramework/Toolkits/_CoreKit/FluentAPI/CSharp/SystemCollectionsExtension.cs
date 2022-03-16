/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections.Generic;

namespace QFramework
{
#if UNITY_EDITOR
    [ClassAPI("FluentAPI.CSharp", "System.Collections", 1)]
    [APIDescriptionCN("针对 System.Collections 提供的链式扩展，理论上任何集合都可以使用")]
    [APIDescriptionEN("The chain extension provided by System.Collections can theoretically be used by any collection")]
#endif
    public static class CollectionsExtension
    {
#if UNITY_EDITOR
        [MethodAPI]
        [APIDescriptionCN("遍历 IEnumerable")]
        [APIDescriptionEN("ForEach for IEnumerable")]
        [APIExampleCode(@"
IEnumerable<int> testIEnumerable = new List<int> { 1, 2, 3 };
testIEnumerable.ForEach(number => Debug.Log(number));
// output 
// 1 
// 2 
// 3
new Dictionary<string, string>() 
{ 
    {""name"",""liangxie""}, 
    {""company"",""liangxiegame"" } 
}
.ForEach(keyValue => Log.I(""key:{0},value:{1}"", keyValue.Key, keyValue.Value));
// output
// key:name,value:liangxie
// key:company,value:liangxiegame")]
#endif
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> self, Action<T> action)
        {
            foreach (var item in self)
            {
                action(item);
            }

            return self;
        }

        /// <summary>
        /// 倒序遍历
        /// <code>
        /// var testList = new List<int> { 1, 2, 3 };
        /// testList.ForEachReverse(number => number.LogInfo());
        /// // 3, 2, 1
        /// </code>
        /// </summary>
        /// <returns>返回自己</returns>
        /// <param name="selfList">Self list.</param>
        /// <param name="action">Action.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static List<T> ForEachReverse<T>(this List<T> selfList, Action<T> action)
        {
            for (var i = selfList.Count - 1; i >= 0; --i)
                action(selfList[i]);

            return selfList;
        }
    }
}