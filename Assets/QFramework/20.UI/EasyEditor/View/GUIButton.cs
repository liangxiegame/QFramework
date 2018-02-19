
using UnityEngine;

namespace QFramework
{
    using System;
    
    public sealed class GUIButton : GUIView
    {
        private readonly Action mOnClickAction;
        private readonly string mText;
        private readonly Rect mRect;
        
        public GUIButton(string text,Rect rect, Action onClickAction)
        {
            mText = text;
            mRect = rect;
            mOnClickAction = onClickAction;
        }

        public override void OnGUI()
        {
            if (Visible && GUI.Button(mRect,mText))
            {
                mOnClickAction.InvokeGracefully();
            }
        }
    }
}