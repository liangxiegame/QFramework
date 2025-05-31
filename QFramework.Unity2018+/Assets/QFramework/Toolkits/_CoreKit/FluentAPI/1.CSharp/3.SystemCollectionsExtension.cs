/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace QFramework
{
#if UNITY_EDITOR
    [ClassAPI("01.FluentAPI.CSharp", "System.Collections", 3)]
    [APIDescriptionCN("针对 System.Collections 提供的链式扩展，理论上任何集合都可以使用")]
    [APIDescriptionEN("The chain extension provided by System.Collections can theoretically be used by any collection")]
#endif
    public static class CollectionsExtension
    {
#if UNITY_EDITOR
        // v1 No.4
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
.ForEach(keyValue => Debug.LogFormat(""key:{0},value:{1}"", keyValue.Key, keyValue.Value));
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

#if UNITY_EDITOR
        // v1 No.5
        [MethodAPI]
        [APIDescriptionCN("List 倒序遍历")]
        [APIDescriptionEN("Reverse ForEach for List")]
        [APIExampleCode(@"
var testList = new List<int> { 1, 2, 3 };
testList.ForEachReverse(number => number.LogInfo());
// 3 2 1
")]
#endif
        public static List<T> ForEachReverse<T>(this List<T> selfList, Action<T> action)
        {
            for (var i = selfList.Count - 1; i >= 0; --i)
                action(selfList[i]);

            return selfList;
        }
        
        
#if UNITY_EDITOR
        // v1 No.6
        [MethodAPI]
        [APIDescriptionCN("遍历 List (可获得索引）")]
        [APIDescriptionEN("foreach List (can get index)")]
        [APIExampleCode(@"
var testList = new List<string> {""a"", ""b"", ""c"" };
testList.Foreach((c,index)=>Debug.Log(index)); 
// 1, 2, 3,
")]
#endif
        public static void ForEach<T>(this List<T> list, Action<int, T> action)
        {
            for (var i = 0; i < list.Count; i++)
            {
                action(i, list[i]);
            }
        }
        
        
#if UNITY_EDITOR
        // v1 No.7
        [MethodAPI]
        [APIDescriptionCN("遍历字典")]
        [APIDescriptionEN("ForEach Dictionary")]
        [APIExampleCode(@"
var infos = new Dictionary<string,string> {{""name"",""liangxie""},{""age"",""18""}};
infos.ForEach((key,value)=> Debug.LogFormat(""{0}:{1}"",key,value);
// name:liangxie    
// age:18
")]
#endif
        public static void ForEach<K, V>(this Dictionary<K, V> dict, Action<K, V> action)
        {
            var dictE = dict.GetEnumerator();

            while (dictE.MoveNext())
            {
                var current = dictE.Current;
                action(current.Key, current.Value);
            }

            dictE.Dispose();
        }
        
        
#if UNITY_EDITOR
        // v1 No.8
        [MethodAPI]
        [APIDescriptionCN("合并字典")]
        [APIDescriptionEN("Merge Dictionaries")]
        [APIExampleCode(@"
var dictionary1 = new Dictionary<string, string> { { ""1"", ""2"" } };
var dictionary2 = new Dictionary<string, string> { { ""3"", ""4"" } };
var dictionary3 = dictionary1.Merge(dictionary2);
dictionary3.ForEach(pair => Debug.LogFormat(""{0}:{1}"", pair.Key, pair.Value));
// 1:2
// 3:4

// notice: duplicate keys are not supported.
// 注意：不支持重复的 key。
")]
#endif
        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this Dictionary<TKey, TValue> dictionary,
            params Dictionary<TKey, TValue>[] dictionaries)
        {
            return dictionaries.Aggregate(dictionary,
                (current, dict) => current.Union(dict).ToDictionary(kv => kv.Key, kv => kv.Value));
        }

#if UNITY_EDITOR
        // v1 No.9
        [MethodAPI]
        [APIDescriptionCN("字典添加新的字典")]
        [APIDescriptionEN("Dictionary Adds a new dictionary")]
        [APIExampleCode(@"
var dictionary1 = new Dictionary<string, string> { { ""1"", ""2"" } };
var dictionary2 = new Dictionary<string, string> { { ""1"", ""4"" } };
var dictionary3 = dictionary1.AddRange(dictionary2,true); // true means override
dictionary3.ForEach(pair => Debug.LogFormat(""{0}:{1}"", pair.Key, pair.Value));
// 1:2
// 3:4

// notice: duplicate keys are  supported.
// 注意：支持重复的 key。
")]
#endif
        public static void AddRange<K, V>(this Dictionary<K, V> dict, Dictionary<K, V> addInDict,
            bool isOverride = false)
        {
            var enumerator = addInDict.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                if (dict.ContainsKey(current.Key))
                {
                    if (isOverride)
                        dict[current.Key] = current.Value;
                    continue;
                }

                dict.Add(current.Key, current.Value);
            }

            enumerator.Dispose();
        }
        
        
        // TODO:
        public static bool IsNullOrEmpty<T>(this T[] collection) => collection == null || collection.Length == 0;
        // TODO:
        public static bool IsNullOrEmpty<T>(this IList<T> collection) => collection == null || collection.Count == 0;
        // TODO:
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection) => collection == null || !collection.Any();
        // TODO:
        public static bool IsNotNullAndEmpty<T>(this T[] collection) => !IsNullOrEmpty(collection);
        // TODO:
        public static bool IsNotNullAndEmpty<T>(this IList<T> collection) => !IsNullOrEmpty(collection);
        // TODO:
        public static bool IsNotNullAndEmpty<T>(this IEnumerable<T> collection) => !IsNullOrEmpty(collection);
    }
}