using UnityEditor;

namespace QFramework
{
    public class HorizontalView : EditorView
    {
        public override void OnGUI()
        {
            if (Visible) EditorGUILayout.BeginHorizontal();
            base.OnGUI();
            if (Visible) EditorGUILayout.EndHorizontal();
        }
    }
}