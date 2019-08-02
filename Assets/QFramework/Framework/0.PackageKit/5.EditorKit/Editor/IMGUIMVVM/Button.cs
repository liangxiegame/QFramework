
using UnityEngine;

namespace QF.Editor
{
    using System;
    
    public sealed class Button : EditorView
    {
        private readonly System.Action mOnClickAction;
        private readonly string mText;
        public Button(string text, float width, float height, System.Action onClickAction)
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
                mOnClickAction.Invoke();
            }
        }
    }
}