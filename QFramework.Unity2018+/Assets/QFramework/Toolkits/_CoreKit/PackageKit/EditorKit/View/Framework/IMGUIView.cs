/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using UnityEngine;

namespace QFramework
{
    public interface IMGUIView : IDisposable
    {
        string Id { get; set; }
        bool Visible { get; set; }

        Func<bool> VisibleCondition { get; set; }

        void DrawGUI();

        IMGUILayout Parent { get; set; }

        FluentGUIStyle Style { get; }

        Color BackgroundColor { get; set; }

        void RefreshNextFrame();

        void AddLayoutOption(GUILayoutOption option);

        void RemoveFromParent();

        void Refresh();

        void Hide();
        void Show();
    }
    
    
     public static class ViewExtension
    {
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

        /// <summary>
        /// SetFontColor Before Build
        /// </summary>
        /// <param name="view"></param>
        /// <param name="color"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T FontColor<T>(this T view, Color color) where T : IMGUIView
        {
            view.Style.Set(style => style.normal.textColor = color);
            return view;
        }

        /// <summary>
        /// Change Font Color After Build Any Time
        /// </summary>
        /// <param name="view"></param>
        /// <param name="color"></param>
        /// <typeparam name="T"></typeparam>
        public static void ChangeFontColor<T>(this T view, Color color) where T : IMGUIView
        {
            view.Style.Value.normal.textColor = color;
        }

        public static T FontBold<T>(this T view) where T : IMGUIView
        {
            view.Style.FontBold();
            return view;
        }

        public static T FontNormal<T>(this T view) where T : IMGUIView
        {
            view.Style.FontNormal();
            return view;
        }

        public static T FontSize<T>(this T view, int fontSize) where T : IMGUIView
        {
            view.Style.FontSize(fontSize);
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