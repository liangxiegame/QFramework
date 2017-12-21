using UnityEngine;

namespace QFramework
{
    public sealed class FlexibaleSpaceView : GUIView
    {
        public override void OnGUI()
        {
            if (!Visible) return;
            GUILayout.FlexibleSpace();            
        }
    }
}