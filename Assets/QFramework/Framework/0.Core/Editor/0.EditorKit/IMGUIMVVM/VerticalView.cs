using UnityEditor;

namespace QFramework
{
    public class VerticalView : GUIView
    {
        private string mStyle = null;
        
        public VerticalView(string style = null)
        {
            mStyle = style;
        }
        
        public override void OnGUI()
        {
            if (!Visible) return;
            EditorGUILayout.BeginVertical(mStyle);
            base.OnGUI();
            EditorGUILayout.EndVertical();
        }
    }
}