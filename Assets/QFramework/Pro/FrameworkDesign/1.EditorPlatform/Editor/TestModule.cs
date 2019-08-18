using UniRx;
using UnityEngine;

namespace QF.Master
{
    public class TestModule : IEditorPlatformModule
    {
        public void OnGUI()
        {
            GUILayout.Label("这个是一个新的模块", new GUIStyle()
            {
                fontSize = 30
            });
        }
    }
}