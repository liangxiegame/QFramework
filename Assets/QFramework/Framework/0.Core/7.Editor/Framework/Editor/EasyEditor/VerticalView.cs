using UnityEditor;

namespace QFramework
{
    public class VerticalView : GUIView
    {
        public override void OnGUI()
        {
            if (!Visible) return;
            EditorGUILayout.BeginVertical();
            base.OnGUI();
            EditorGUILayout.EndVertical();
        }
    }
}