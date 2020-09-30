using System;
using UnityEngine;
using UnityEngine.Events;

namespace QFramework
{
    public class TextView : View
    {
        public TextView(string content = "", Action<string> onValueChanged = null)
        {
            Content = new Property<string>(content);
            //Style = GUI.skin.textField;

            Content.Bind(_ => OnValueChanged.Invoke());

            if (onValueChanged != null)
            {
                Content.Bind(onValueChanged);
            }
        }

        public Property<string> Content;

        protected override void OnGUI()
        {
            if (mPasswordMode)
            {
                Content.Value = CrossPlatformGUILayout.PasswordField(Content.Value, GUI.skin.textField, LayoutStyles);
            }
            else
            {
                Content.Value = CrossPlatformGUILayout.TextField(Content.Value, GUI.skin.textField, LayoutStyles);
            }
        }

        public UnityEvent OnValueChanged = new UnityEvent();


        private bool mPasswordMode = false;

        public TextView PasswordMode()
        {
            mPasswordMode = true;
            return this;
        }
    }
}