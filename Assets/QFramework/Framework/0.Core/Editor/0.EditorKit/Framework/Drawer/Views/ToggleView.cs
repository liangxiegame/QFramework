using UnityEditor.Graphs;
using UnityEngine;

namespace EGO.Framework
{
    public class ToggleView : View
    {
        public string Text { get; }

        public ToggleView(string text, bool initValue = false)
        {
            Text = text;
            Toggle = new Property<bool>(initValue);
        }

        public Property<bool> Toggle { get; private set; }

        protected override void OnGUI()
        {
            Toggle.Value = GUILayout.Toggle(Toggle.Value, Text);
        }
    }
}