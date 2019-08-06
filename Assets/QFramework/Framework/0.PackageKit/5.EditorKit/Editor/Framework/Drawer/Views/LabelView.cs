using System;
using EGO.Framework;
using UnityEngine;

namespace QF.Editor
{
    public class LabelView : View
    {
        public string Content { get; set; }

        public LabelView(string content)
        {
            Content = content;
            try
            {
                Style = new GUIStyle(GUI.skin.label);
            }
            catch (Exception e)
            {
                
            }
        }


        protected override void OnGUI()
        {
            GUILayout.Label(Content, Style, LayoutStyles);
        }
    }
}