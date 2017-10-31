using UnityEditor;

namespace QFramework
{
    public class SpaceView :EditorView
    {
        public override void OnGUI()
        {
            if (Visible) return;
            base.OnGUI();
            EditorGUILayout.Space();
            
        }
    }
}