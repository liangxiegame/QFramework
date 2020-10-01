using UnityEngine;

namespace QFramework
{
    public class FlexibleSpaceView : View
    {
        protected override void OnGUI()
        {
            GUILayout.FlexibleSpace();
        }
    }
}