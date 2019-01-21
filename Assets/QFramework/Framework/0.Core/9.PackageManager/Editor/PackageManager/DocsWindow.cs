using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public class DocsWindow : EditorWindow
    {
        private string mPackagePath;

        private string[] mDocPaths = { };

        private Vector2 mScrollPos;

        public static void Init(string packagePath)
        {
            var readmeWin = (DocsWindow) GetWindow(typeof(DocsWindow), true, packagePath, true);
            readmeWin.mPackagePath = packagePath;
            readmeWin.mDocPaths = Directory.GetFiles(packagePath, "*.md", SearchOption.AllDirectories);

            readmeWin.position = new Rect(Screen.width / 2, Screen.height / 2, 600, 300);
            readmeWin.Show();
        }

        public void OnGUI()
        {
            mScrollPos =
                GUILayout.BeginScrollView(mScrollPos, false, true, GUILayout.Width(580), GUILayout.Height(300));

            mDocPaths.ForEach(item =>
            {
                var docPath = item.Replace("\\", "/");
                var docName = docPath.Replace(mPackagePath, string.Empty);
                
                if (GUILayout.Button(docName))
                {
//                    DocViewerWindow.Load("file:///" + docPath);
                    DocViewerWindow.Load(
                        "https://www.notion.so/qframework/QFramework-e5a4846c79de459da0407859000bac24");
                }
            });

            GUILayout.EndScrollView();
        }
    }
}