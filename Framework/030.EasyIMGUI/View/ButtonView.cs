using System;
using UnityEngine;
using UnityEngine.Events;

namespace QFramework
{
    public class ButtonView : View
    {
        public ButtonView(string text = null, Action onClickEvent = null)
        {
            Text = text;
            OnClickEvent = onClickEvent;
        }

        public string Text { get; set; }

        public Action OnClickEvent { get; set; }
        public UnityEvent OnClick = new UnityEvent();

        protected override void OnGUI()
        {
            if (GUILayout.Button(Text, GUI.skin.button, LayoutStyles))
            {
                if (OnClickEvent != null)
                {
                    OnClickEvent.Invoke();
                }

                OnClick.Invoke();
            }
        }
    }
}