using System;

using System.Linq.Expressions;
using UnityEngine;
using Action = System.Action;

namespace UnityEditorUI
{
    /// <summary>
    /// Clickable push button widget.
    /// </summary>
    public interface IButton : IWidget
    {
        /// <summary>
        /// Text to be displayed on the button.
        /// </summary>
        IButton Text(string text);

        /// <summary>
        /// Tooltip displayed on mouse hover.
        /// </summary>
        IButton Tooltip(string tooptip);

        /// <summary>
        /// Width of the widget in pixels. Default uses auto-layout.
        /// </summary>
        IButton Width(int width);

        /// <summary>
        /// Height of the widget in pixels. Default uses auto-layout.
        /// </summary>
        IButton Height(int height);


        /// <summary>
        /// Height of the widget in pixels. Default uses auto-layout.
        /// </summary>
        IButton Texture(Texture2D texture2D);

        /// <summary>
        /// Event invoked when the button is clicked.
        /// </summary>
        IButton Click(Expression<Action> methodExpression);
    }

    /// <summary>
    /// Clickable push button widget.
    /// </summary>
    internal class Button : AbstractWidget, IButton
    {
        // Private members
        //private string mText = String.Empty;
        //private string mTooltip = String.Empty;
        //private int mWidth = -1;
        //private int mHeight = -1;
        private Texture2D mTexture2D;

        private readonly EventBinding<IButton> mClickEvent;

        // Public interfaces for getting PropertyBindings
        public IButton Text(string text)
        {
            //mText = text;
            return this;
        }

        public IButton Tooltip(string toolTip)
        {
            //mTooltip = toolTip;
            return this;
        }

        public IButton Width(int width)
        {
            //mWidth = width;
            return this;
        }

        public IButton Height(int height)
        {
            //mHeight = height;
            return this;
        }

        public IButton Texture(Texture2D texture2D)
        {
            mTexture2D = texture2D;
            return this;
        }

        public IButton Click(Expression<Action> methodExpression)
        {
            mClickEvent.Bind(methodExpression);
            return this;
        }        

        internal Button(ILayout parent) : base(parent)
        {
            mClickEvent = new EventBinding<IButton>(this);
        }

        private Rect mRect = Rect.zero;
        public override void OnGUI()
        {
            mRect.width = mTexture2D.width;
            mRect.height = mTexture2D.height;

            if (UnityEngine.GUI.Button(mRect, mTexture2D, GUIStyle.none))
            {
				QF.Log.I("click");
                mClickEvent.Invoke();
            }
        }

        public override void BindViewModel(object viewModel)
        {
            mClickEvent.BindViewModel(viewModel);
        }
    }
}
