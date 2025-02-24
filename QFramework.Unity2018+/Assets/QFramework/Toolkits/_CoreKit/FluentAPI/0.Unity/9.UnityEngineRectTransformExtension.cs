/****************************************************************************
 * Copyright (c) 2015 - 2024 liangxiegame UNDER MIT License
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using UnityEngine;

namespace QFramework
{
#if UNITY_EDITOR
    [ClassAPI("00.FluentAPI.Unity", "UnityEngine.RectTransform", 9)]
    [APIDescriptionCN("针对 RectTransform 封装的函数")]
    [APIDescriptionEN("wrapper function for RectTransform")]
#endif
    public static class UnityEngineRectTransformExtension 
    {
#if UNITY_EDITOR
        // v1.0.167
        [MethodAPI]
        [APIDescriptionCN("设置 rectTransform.anchoredPosition.y 值")]
        [APIDescriptionEN("set rectTransform.anchoredPosition.y value")]
        [APIExampleCode(@"
text.rectTransform.AnchoredPositionY(5);
")]
#endif
        public static RectTransform AnchoredPositionY(this RectTransform selfRectTrans, float anchoredPositionY)
        {
            var anchorPos = selfRectTrans.anchoredPosition;
            anchorPos.y = anchoredPositionY;
            selfRectTrans.anchoredPosition = anchorPos;
            return selfRectTrans;
        }
    }
}