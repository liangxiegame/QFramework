using QF.GraphDesigner;
using Invert.uFrame.Editor;
using UnityEditor;
using UnityEngine;

public class uFrameVersionProcessor : AssetPostprocessor
{
   
    private const string VERSION_KEY = "uFrame.InstalledVersion";

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        var lastVersion = EditorPrefs.GetString(VERSION_KEY, "");
        if (lastVersion != InvertGraphEditor.CURRENT_VERSION)
        {
            EditorApplication.delayCall += ShowChangeLog;
            EditorPrefs.SetString(VERSION_KEY, InvertGraphEditor.CURRENT_VERSION);
        }
    }

    private static void ShowChangeLog()
    {
        EditorApplication.delayCall -= ShowChangeLog;
        EditorApplication.ExecuteMenuItem("Window/uFrame/Welcome Screen");
        
    }
}