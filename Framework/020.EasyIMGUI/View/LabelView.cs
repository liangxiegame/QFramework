using UnityEngine;

namespace QFramework
{
    public class LabelView : View
    {
        public string Content { get; set; }

        public LabelView(string content)
        {
            Content = content;

            mStyleProperty = new GUIStyleProperty(() => new GUIStyle(GUI.skin.label));
        }

        protected override void OnGUI()
        {
            GUILayout.Label(Content, Style.Value, LayoutStyles);
        }
    }
}