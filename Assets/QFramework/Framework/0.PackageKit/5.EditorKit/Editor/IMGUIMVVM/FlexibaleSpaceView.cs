using UnityEngine;

namespace QF
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