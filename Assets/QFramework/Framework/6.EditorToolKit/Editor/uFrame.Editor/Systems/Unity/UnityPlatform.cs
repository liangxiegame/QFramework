using System;
using System.Text;
using QF.GraphDesigner;
using UnityEditor;
using UnityEngine;

namespace QF.GraphDesigner.Unity
{
    public class UnityPlatform : DiagramPlugin, IPlatformOperations, IDebugLogger
    {

        //public void ShowFileDialog(string title)
        //{
        //    EditorUtility.OpenFilePanel(title,directory,)
        //}

        public void OpenScriptFile(string filePath)
        {
            var scriptAsset = AssetDatabase.LoadAssetAtPath(filePath, typeof(TextAsset));
            AssetDatabase.OpenAsset(scriptAsset);
        }

        public void OpenLink(string link)
        {
            Application.OpenURL(link);
        }

        public string GetAssetPath(object graphData)
        {
            return AssetDatabase.GetAssetPath(graphData as UnityEngine.Object);
        }
        public bool MessageBox(string title, string message, string ok)
        {
            return EditorUtility.DisplayDialog(title, message, ok);
        }
        public bool MessageBox(string title, string message, string ok, string cancel)
        {
            return EditorUtility.DisplayDialog(title, message, ok, cancel);
        }

        public void SaveAssets()
        {
            AssetDatabase.SaveAssets();
        }

        public void RefreshAssets()
        {
            AssetDatabase.Refresh();
            //AssetDatabase.Refresh();
        }

        public void Progress(float progress, string message)
        {
            try
            {
                InvertApplication.SignalEvent<ITaskProgressHandler>(_=>_.Progress(progress, message));
            }
            catch (Exception ex)
            {
                
            }
        }

        public void Log(string message)
        {
            Debug.Log(message); 
        }

        public void LogException(Exception ex)
        {
            Debug.LogException(ex);
            if (ex.InnerException != null)
            {
                Debug.LogException(ex.InnerException);
            }
        }
    }
}
