using System;
using UnityEngine;

namespace QFramework
{
    public interface IButton : IMGUIView,
        IHasText<IButton>,
        ICanClick<IButton>
    {
    }

    internal class Button : View, IButton
    {
        private string mLabelText = string.Empty;
        private Action mOnClick = () => { };

        protected override void OnGUI()
        {
            if (GUILayout.Button(mLabelText, GUI.skin.button, LayoutStyles))
            {
                mOnClick.Invoke();
            }
        }

        public IButton Text(string labelText)
        {
            mLabelText = labelText;
            return this;
        }

        public IButton OnClick(Action action)
        {
            mOnClick = action;
            return this;
        }
    }
}