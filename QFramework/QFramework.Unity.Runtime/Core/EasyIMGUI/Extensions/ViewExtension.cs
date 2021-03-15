/****************************************************************************
 * Copyright (c) 2018 ~ 2020.12 liangxie
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
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

using System;
using UnityEngine;

namespace QFramework
{
    public static class ViewExtension
    {
        [Obsolete("请使用 Self",Deprecated.REMOVED_IN_V0_11_00)]
        public static TView Do<TView>(this TView self, Action<TView> onDo) where TView : IMGUIView
        {
            onDo(self);
            return self;
        }
        
        public static TView Self<TView>(this TView self, Action<TView> onDo) where TView : IMGUIView
        {
            onDo(self);
            return self;
        }

        public static T Width<T>(this T view, float width) where T : IMGUIView
        {
            view.AddLayoutOption(GUILayout.Width(width));
            return view;
        }

        public static T Height<T>(this T view, float height) where T : IMGUIView
        {
            view.AddLayoutOption(GUILayout.Height(height));
            return view;
        }

        public static T MaxHeight<T>(this T view, float height) where T : IMGUIView
        {
            view.AddLayoutOption(GUILayout.MaxHeight(height));
            return view;
        }

        public static T MinHeight<T>(this T view, float height) where T : IMGUIView
        {
            view.AddLayoutOption(GUILayout.MinHeight(height));
            return view;
        }

        public static T ExpandHeight<T>(this T view) where T : IMGUIView
        {
            view.AddLayoutOption(GUILayout.ExpandHeight(true));
            return view;
        }


        public static T TextMiddleLeft<T>(this T view) where T : IMGUIView
        {
            view.Style.Set(style => style.alignment = TextAnchor.MiddleLeft);
            return view;
        }

        public static T TextMiddleRight<T>(this T view) where T : IMGUIView
        {
            view.Style.Set(style => style.alignment = TextAnchor.MiddleRight);
            return view;
        }

        public static T TextLowerRight<T>(this T view) where T : IMGUIView
        {
            view.Style.Set(style => style.alignment = TextAnchor.LowerRight);
            return view;
        }

        public static T TextMiddleCenter<T>(this T view) where T : IMGUIView
        {
            view.Style.Set(style => style.alignment = TextAnchor.MiddleCenter);
            return view;
        }

        public static T TextLowerCenter<T>(this T view) where T : IMGUIView
        {
            view.Style.Set(style => style.alignment = TextAnchor.LowerCenter);
            return view;
        }

        public static T Color<T>(this T view, Color color) where T : IMGUIView
        {
            view.BackgroundColor = color;
            return view;
        }

        public static T FontColor<T>(this T view, Color color) where T : IMGUIView
        {
            view.Style.Set(style => style.normal.textColor = color);
            return view;
        }

        public static T FontBold<T>(this T view) where T : IMGUIView
        {
            view.Style.Set(style => style.fontStyle = FontStyle.Bold);
            return view;
        }

        public static T FontNormal<T>(this T view) where T : IMGUIView
        {
            view.Style.Set(style => style.fontStyle = FontStyle.Normal);
            return view;
        }

        public static T FontSize<T>(this T view, int fontSize) where T : IMGUIView
        {
            view.Style.Set(style => style.fontSize = fontSize);
            return view;
        }

        public static T Visible<T>(this T view, bool visible) where T : IMGUIView
        {
            view.Visible = visible;
            return view;
        }

        public static T WithVisibleCondition<T>(this T view, Func<bool> visibleCondition) where T : IMGUIView
        {
            view.VisibleCondition = visibleCondition;
            return view;
        }
    }
}