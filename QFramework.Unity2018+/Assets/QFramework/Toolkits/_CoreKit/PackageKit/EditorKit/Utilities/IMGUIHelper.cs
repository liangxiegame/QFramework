/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT License
 *
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace QFramework
{
    public static class IMGUIHelper
    {
        private static readonly FluentGUIStyle SelectionRect = new FluentGUIStyle(() => "SelectionRect");

        public static void LastRectSelectionCheck<T>(T current,T select,Action onSelect)
        {
            var lastRect = GUILayoutUtility.GetLastRect();

            if (Equals(current,select))
            {
                GUI.Box(lastRect, "", SelectionRect.Value);
            }

            if (lastRect.Contains(Event.current.mousePosition) &&
                Event.current.type == EventType.MouseUp)
            {
                onSelect();
                Event.current.Use();
            }
        }
        
#if UNITY_EDITOR
        public static void ShowEditorDialogWithErrorMsg(string content)
        {
            EditorUtility.DisplayDialog("error", content, "OK");
        }
#endif
        
        /// <summary>
        /// 设置屏幕分辨率（拉伸)
        /// SetDesignResolution(Strench)
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void SetDesignResolution(float width, float height)
        {
            var scaleX = Screen.width / width;
            var scaleY = Screen.height / height;

            var scale = Mathf.Min(scaleX, scaleY);

            GUIUtility.ScaleAroundPivot(new Vector2(scale, scale), new Vector2(0, 0));
        }
    }
}