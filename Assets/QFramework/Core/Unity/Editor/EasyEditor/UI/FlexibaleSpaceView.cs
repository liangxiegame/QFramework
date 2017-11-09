using UnityEngine;

namespace QFramework
{
    public sealed class FlexibaleSpaceView : EditorView
    {
        public override void OnGUI()
        {
            if (!Visible) return;
            GUILayout.FlexibleSpace();            
        }
    }
}