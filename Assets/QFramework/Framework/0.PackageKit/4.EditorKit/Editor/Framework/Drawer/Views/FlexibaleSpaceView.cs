using UnityEngine;

namespace EGO.Framework
{
    public class FlexibaleSpaceView : View
    {
        protected override void OnGUI()
        {
            GUILayout.FlexibleSpace();
        }
    }
}