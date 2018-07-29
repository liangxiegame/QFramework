/****************************************************************************
 * Copyright (c) 2018.3 liangxie
 ****************************************************************************/

using UnityEngine;

namespace QFramework
{
    public class FrameworkPMView 
    {
        public FrameworkLocalVersion FrameworkLocalVersion;

        public void Init()
        {
            FrameworkLocalVersion = FrameworkLocalVersion.Get();
        }
        
        public void OnGUI()
        {
            GUILayout.Label("Framework:");
            GUILayout.BeginVertical("box");
            GUILayout.Label(string.Format("Current Framework Version:{0}", FrameworkLocalVersion.Version));
			
            GUILayout.BeginHorizontal();
			
            if (GUILayout.Button("Download Latest Version"))
            {
                EditorActionKit.ExecuteNode(new DownloadLatestFramework());
            }

            if (GUILayout.Button(string.Format("Download Demo:{0}", FrameworkLocalVersion.Version)))
            {
                EditorActionKit.ExecuteNode(new DownloadLatestDemo(FrameworkLocalVersion.Version));
            }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }
    }
}