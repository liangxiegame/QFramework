/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT License
 *
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using UnityEngine;

namespace QFramework
{
    public static class IMGUIGestureHelper
    {
        private static readonly GUIStyle SelectionRect = "SelectionRect";

        public static void LastRectSelectionCheck<T>(T current,T select,Action onSelect)
        {
            var lastRect = GUILayoutUtility.GetLastRect();

            if (Equals(current,select))
            {
                GUI.Box(lastRect, "", SelectionRect);
            }

            if (lastRect.Contains(Event.current.mousePosition) &&
                Event.current.type == EventType.MouseUp)
            {
                onSelect();
                Event.current.Use();
            }
        }
    }
}
#endif