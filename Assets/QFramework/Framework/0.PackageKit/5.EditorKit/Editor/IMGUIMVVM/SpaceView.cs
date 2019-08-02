using UnityEditor;

namespace QF
{
    public class SpaceView :GUIView
    {
        public override void OnGUI()
        {
            if (Visible) return;
            base.OnGUI();
            EditorGUILayout.Space();   
        }

    }
}