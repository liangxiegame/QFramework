using UnityEngine;

namespace EGO.Framework
{
    public class LabelView : View
    {
        public string Content { get; set; }

        public LabelView(string content)
        {
            Content = content;
        }
        
        protected override void OnGUI()
        {
            GUILayout.Label(Content, Style, LayoutStyles);
        }
    }
}