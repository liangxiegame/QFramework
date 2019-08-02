using UnityEditor;

namespace QF
{
    public class HorizontalView : GUIView
    {
        public override void OnGUI()
        {
            if (Visible) EditorGUILayout.BeginHorizontal();
            base.OnGUI();
            if (Visible) EditorGUILayout.EndHorizontal();
        }
    }
}