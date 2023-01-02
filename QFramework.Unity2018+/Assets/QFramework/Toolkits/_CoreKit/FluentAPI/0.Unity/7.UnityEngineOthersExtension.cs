/****************************************************************************
 * Copyright (c) 2016 - 2022 liangxiegame UNDER MIT License
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