
using QF.Extensions;
using UnityEngine;

namespace QF
{
    using System;
    
    public sealed class GUIButton : GUIView
    {
        private readonly System.Action mOnClickAction;
        private readonly string mText;
        private readonly Rect mRect;
        
        public GUIButton(string text,Rect rect, System.Action onClickAction)
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