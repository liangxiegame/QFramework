using UnityEngine;

namespace QFramework
{
    public class BoxView : View
    {
        public string Text;

        public BoxView(string text)
        {
            Text = text;
            //Style = new GUIStyle(GUI.skin.box);
        }

        protected override void OnGUI()
        {
            GUILayout.Box(Text, GUI.skin.box, LayoutStyles);
        }
    }
}