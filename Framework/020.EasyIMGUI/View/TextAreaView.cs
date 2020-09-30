using UnityEngine;

namespace QFramework
{
    public class TextAreaView : View
    {
        public TextAreaView(string content = "")
        {
            Content = new Property<string>(content);
            //Style = new GUIStyle(GUI.skin.textArea);
        }

        public Property<string> Content { get; set; }

        protected override void OnGUI()
        {
            Content.Value = CrossPlatformGUILayout.TextArea(Content.Value, GUI.skin.textArea, LayoutStyles);
        }
    }
}