using System.IO;
using UnityEngine;

namespace QFramework
{
    public class FrameworkModule : ConsoleModule
    {
        public override string Title { get; set; } = "Framework";

        public override void DrawGUI()
        {
            base.DrawGUI();

            if (GUILayout.Button("Clear All Data & Quit"))
            {
                PlayerPrefs.DeleteAll();
                Directory.Delete(Application.persistentDataPath, true);
                Quit();
            }
        }

        public static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit ();
#endif
        }
    }
}