

using UnityEngine;

namespace QFramework
{
    using System;
    
    public sealed class ButtonView : EditorView
    {
        private Action mOnClickAction;
        private string mText;
        public ButtonView(string text, float width, float height, Action onClickAction)
        {
            mText = text;
            Width = width;
            Height = height;
            mOnClickAction = onClickAction;
        }

        public override void OnGUI()
        {
            if (Visible && GUILayout.Button(mText, mLayoutOptions))
            {
                mOnClickAction.InvokeGracefully();
            }
        }
    }
}