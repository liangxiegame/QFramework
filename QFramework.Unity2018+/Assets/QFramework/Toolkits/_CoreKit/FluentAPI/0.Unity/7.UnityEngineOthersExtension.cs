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
    [ClassAPI("00.FluentAPI.Unity", "UnityEngine.Others", 7)]
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
}