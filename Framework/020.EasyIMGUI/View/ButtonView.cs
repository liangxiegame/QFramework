using System;
using UnityEngine;

namespace QFramework
{
    public interface IButton : IMGUIView,
        IHasLabel<IButton>,
        ICanClick<IButton>
    {
    }

    internal class ButtonView : View, IButton
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

        public IButton Label(string labelText)
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