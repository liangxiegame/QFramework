/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using UnityEngine;

namespace QFramework
{
#if UNITY_EDITOR
    [ClassAPI("00.FluentAPI.Unity", "UnityEngine.Color", 5)]
    [APIDescriptionCN("UnityEngine.Color 静态扩展")]
    [APIDescriptionEN("UnityEngine.Color extension")]
#endif
    public static class UnityEngineColorExtension
    {
#if UNITY_EDITOR
        // v1 No.152
        [MethodAPI]
        [APIDescriptionCN("HTML string(#000000) 转 Color")]
        [APIDescriptionEN("HTML string(like #000000)")]
        [APIExampleCode(@"
var color = ""#C5563CFF"".HtmlStringToColor();
Debug.Log(color);"
        )]
#endif
        public static Color HtmlStringToColor(this string htmlString)
        {
            var parseSucceed = ColorUtility.TryParseHtmlString(htmlString, out var retColor);
            return parseSucceed ? retColor : Color.black;
        }
    }
}