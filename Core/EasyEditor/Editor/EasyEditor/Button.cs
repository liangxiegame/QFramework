
using UnityEngine;

namespace QFramework
{
    using System;
    
    public sealed class Button : EditorView
    {
        private readonly Action mOnClickAction;
        private readonly string mText;
        public Button(string text, float width, float height, Action onClickAction)
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