using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

[CustomEditor(typeof(TextAsset), true)]
public class TextInspectorBypass : Editor
{
    private Editor _defaultEditor;
    private bool _isDocViewable;
    private string _fullPath;

	void OnEnable ()
    {
        // as we can't inherit from the 
        _defaultEditor = CreateEditor(target, System.Type.GetType("UnityEditor.TextAssetInspector, UnityEditor"));

        _fullPath = AssetDatabase.GetAssetPath(target);
        string extension = System.IO.Path.GetExtension(_fullPath).ToLower();

        _isDocViewable =  extension == ".html" || extension == ".md"; 
    }

    public override void OnInspectorGUI()
    {
        GUI.enabled = true;

        if (_isDocViewable && GUILayout.Button("Open Doc Viewer"))
        {
            DocViewerWindow.Load("file:///"+_fullPath);
        }

        _defaultEditor.OnInspectorGUI();
    }
}
