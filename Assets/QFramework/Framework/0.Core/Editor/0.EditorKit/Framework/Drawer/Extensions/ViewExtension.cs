using UnityEngine;

namespace EGO.Framework
{
    public static class ViewExtension
    {
        public static T Width<T>(this T view, float width) where T : IView
        {
            view.AddLayoutOption(GUILayout.Width(width));
            return view;
        }

        public static T Height<T>(this T view, float height) where T : IView
        {
            view.AddLayoutOption(GUILayout.Height(height));
            return view;
        }
        
        public static T MaxHeight<T>(this T view, float height) where T : IView
        {
            view.AddLayoutOption(GUILayout.MaxHeight(height));
            return view;
        }
        
        public static T MinHeight<T>(this T view, float height) where T : IView
        {
            view.AddLayoutOption(GUILayout.MinHeight(height));
            return view;
        }
        
        public static T ExpandHeight<T>(this T view) where T : IView
        {
            view.AddLayoutOption(GUILayout.ExpandHeight(true));
            return view;
        }
        
        
        public static T TextMiddleLeft<T>(this T view) where T : IView
        {
            view.Style.alignment = TextAnchor.MiddleLeft;
            return view;
        }
        
        public static T TextMiddleRight<T>(this T view) where T : IView
        {
            view.Style.alignment = TextAnchor.MiddleRight;
            return view;
        }
        
        public static T TextLowerRight<T>(this T view) where T : IView
        {
            view.Style.alignment = TextAnchor.LowerRight;
            return view;
        }
        
        public static T TextMiddleCenter<T>(this T view) where T : IView
        {
            view.Style.alignment = TextAnchor.MiddleCenter;
            return view;
        }
        
        public static T TextLowerCenter<T>(this T view) where T : IView
        {
            view.Style.alignment = TextAnchor.LowerCenter;
            return view;
        }
        
        public static T Color<T>(this T view,Color color) where T : IView
        {
            view.BackgroundColor = color;
            return view;
        }

        public static T FontColor<T>(this T view, Color color) where T : IView
        {
            view.Style.normal.textColor = color;
            return view;
        }
        
        public static T FontBold<T>(this T view) where T : IView
        {
            view.Style.fontStyle = FontStyle.Bold; 
            return view;
        }
        
        public static T FontNormal<T>(this T view) where T : IView
        {
            view.Style.fontStyle = FontStyle.Normal; 
            return view;
        }
        
        public static T FontSize<T>(this T view, int fontSize) where T : IView
        {
            view.Style.fontSize = fontSize;
            return view;
        }

    }
}