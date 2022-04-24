/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT License
 *
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_2018_4
using UnityEngine.Experimental.UIElements;
#else
using UnityEngine.UIElements;
#endif

#if UNITY_2018_4_OR_NEWER
namespace QFramework
{
    public static class UIElementExtensions
    {
        public static T Padding<T>(this T self, float top, float right, float bottom,
            float left) where T : VisualElement
        {
            self.style.paddingTop = top;
            self.style.paddingRight = right;
            self.style.paddingBottom = bottom;
            self.style.paddingLeft = left;
            return self;
        }

        public static T AddChild<T>(this T self, VisualElement child) where T : VisualElement
        {
            self.Add(child);
            return self;
        }

        public static T AddToParent<T>(this T self, VisualElement parent) where T : VisualElement
        {
            parent.AddChild(self);
            return self;
        }
    }
}
#endif