using System;
using UnityEngine;

namespace EGO.Framework
{
    public class ButtonView : View
    {
        public ButtonView(string text, Action onClickEvent)
        {
            Text = text;
            OnClickEvent = onClickEvent;
            //Style = new GUIStyle(GUI.skin.button);
        }

        public string Text { get; set; }

        public Action OnClickEvent { get; set; }

        protected override void OnGUI()
        {
            if (GUILayout.Button(Text, GUI.skin.button, LayoutStyles))
            {
                OnClickEvent.Invoke();
            }
        }
    }
}