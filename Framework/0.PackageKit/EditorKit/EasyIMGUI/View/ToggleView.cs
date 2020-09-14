using UnityEngine;

namespace QFramework
{
    public class ToggleView : View
    {
        public string Text { get; private set; }

        public ToggleView(string text, bool initValue = false)
        {
            Text = text;
            Toggle = new Property<bool>(initValue);

            Style = new GUIStyleProperty(() => GUI.skin.toggle);
        }

        public Property<bool> Toggle { get; private set; }

        protected override void OnGUI()
        {
            // Toggle.Value = GUILayout.Toggle(Toggle.Value, Text, Style, LayoutStyles);
            Toggle.Value = GUILayout.Toggle(Toggle.Value, Text, LayoutStyles);
        }
    }

}