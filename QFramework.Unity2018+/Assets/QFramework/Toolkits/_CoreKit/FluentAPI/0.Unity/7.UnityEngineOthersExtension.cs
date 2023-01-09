/****************************************************************************
 * Copyright (c) 2016 - 2023 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
#if UNITY_EDITOR
    [ClassAPI("00.FluentAPI.Unity", "UnityEngine.Others", 8)]
    [APIDescriptionCN("其他的一些静态扩展")]
    [APIDescriptionEN("other extension")]
#endif
    public static class UnityEngineOthersExtension
    {
#if UNITY_EDITOR
        // v1 No.155
        [MethodAPI]
        [APIDescriptionCN("随机 List 中的一个元素")]
        [APIDescriptionEN("get random item in a list")]
        [APIExampleCode(@"
new List<int>(){ 1,2,3 }.GetRandomItem();
")]
#endif
        public static T GetRandomItem<T>(this List<T> list)
        {
            return list[UnityEngine.Random.Range(0, list.Count)];
        }

#if UNITY_EDITOR
        // v1
        [MethodAPI]
        [APIDescriptionCN("为 SpriteRender 设置 alpha 值")]
        [APIDescriptionEN("set SpriteRender's alpha value")]
        [APIExampleCode(@"
mySprRender.Alpha(0.5f);
")]
#endif
        public static SpriteRenderer Alpha(this SpriteRenderer self, float alpha)
        {
            var color = self.color;
            color.a = alpha;
            self.color = color;
            return self;
        }

#if UNITY_EDITOR
        // Added in v1.0.31
        [MethodAPI]
        [APIDescriptionCN("Mathf.Lerp")]
        [APIDescriptionEN("Mathf.Lerp")]
        [APIExampleCode(@"
var v = 0.5f.Lerp(0.1f,0.5f);
// v is 0.3f
")]
#endif
        public static float Lerp(this float self, float a, float b)
        {
            return Mathf.Lerp(a, b, self);
        }

#if UNITY_EDITOR
        // Added in v1.0.31
        [MethodAPI]
        [APIDescriptionCN("Mathf.Abs")]
        [APIDescriptionEN("Mathf.Abs")]
        [APIExampleCode(@"
var absValue = -1.0f.Abs();
// absValue is 1.0f
")]
#endif
        public static float Abs(this float self)
        {
            return Mathf.Abs(self);
        }

#if UNITY_EDITOR
        // Added in v1.0.31
        [MethodAPI]
        [APIDescriptionCN("Mathf.Sign")]
        [APIDescriptionEN("Mathf.Sign")]
        [APIExampleCode(@"
var sign = -5.0f.Sign();
// sign is 5.0f
")]
#endif
        public static float Sign(this float self)
        {
            return Mathf.Sign(self);
        }
    }

#if UNITY_EDITOR
    [ClassAPI("00.FluentAPI.Unity", "UnityEngine.Random", 7)]
    [APIDescriptionCN("针对随机做的一些封装")]
    [APIDescriptionEN("wrapper for random")]
#endif
    public static class RandomUtility
    {
#if UNITY_EDITOR
        // v1
        [MethodAPI]
        [APIDescriptionCN("随机选择")]
        [APIDescriptionEN("RandomChoose")]
        [APIExampleCode(@"
var result = RandomUtility.Choose(1,1,1,2,2,2,2,3,3);

if (result == 3)
{
    // todo ...
}
")]
#endif
        public static T Choose<T>(params T[] args)
        {
            return args[UnityEngine.Random.Range(0, args.Length)];
        }
    }
}