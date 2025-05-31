/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using UnityEngine;

namespace QFramework
{
    public interface IMGUIBox : IMGUIView, IHasText<IMGUIBox>
    {
    }

    public class IMGUIBoxView : IMGUIAbstractView, IMGUIBox
    {
        public IMGUIBoxView()
        {
            mStyle = new FluentGUIStyle(() =>
            {
                // Box 的颜色保持和文本的颜色一致
                var boxStyle = new GUIStyle(GUI.skin.box) { normal = { textColor = GUI.skin.label.normal.textColor } };
                return boxStyle;
            });
        }

        protected override void OnGUI()
        {
            GUILayout.Box(mText, mStyle.Value, LayoutStyles);
        }

        private string mText = string.Empty;

        public IMGUIBox Text(string labelText)
        {
            mText = labelText;
            return this;
        }
    }
}
#endif