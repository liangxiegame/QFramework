/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using UnityEngine.UI;

namespace QFramework
{
#if UNITY_EDITOR
    [ClassAPI("00.FluentAPI.Unity", "UnityEngine.Graphic", 6)]
    [APIDescriptionCN("UnityEngine.UI.Graphic 静态扩展")]
    [APIDescriptionEN("UnityEngine.UI.Graphic extension")]
#endif
    public static class UnityEngineUIGraphicExtension
    {
#if UNITY_EDITOR
        // v1 No.153
        [MethodAPI]
        [APIDescriptionCN("设置 Graphic 的 alpha 值 ")]
        [APIDescriptionEN("set graphic's alpha value")]
        [APIExampleCode(@"
var gameObject = new GameObject();
var image = gameObject.AddComponent<Image>();
var rawImage = gameObject.AddComponent<RawImage>();


image.ColorAlpha(1.0f);
rawImage.ColorAlpha(1.0f);
")]
#endif
        public static T ColorAlpha<T>(this T selfGraphic, float alpha) where T : Graphic
        {
            var color = selfGraphic.color;
            color.a = alpha;
            selfGraphic.color = color;
            return selfGraphic;
        }

#if UNITY_EDITOR
        // v1 No.154
        [MethodAPI]
        [APIDescriptionCN("设置 image 的 fillAmount 值")]
        [APIDescriptionEN("set image's fillAmount value")]
        [APIExampleCode(@"
var gameObject = new GameObject();
var image1 = gameObject.AddComponent<Image>();

image1.FillAmount(0.0f); 
")]
#endif
        public static Image FillAmount(this Image selfImage, float fillAmount)
        {
            selfImage.fillAmount = fillAmount;
            return selfImage;
        }
    }
}