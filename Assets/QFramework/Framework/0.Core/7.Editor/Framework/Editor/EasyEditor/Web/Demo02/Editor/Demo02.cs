using UnityEngine;
using UnityEditor;

/// <summary>
/// Demonstrate calling javascript from C# sdie.
/// </summary>
public class Example02Window : CustomWebViewEditorWindow
{
    [MenuItem("Window/Example02")]
    static void Open()
    {
        string path = Application.dataPath + "/Demo02/index.html";
        var w = CreateWebViewEditorWindow<Example02Window>("Example", path, 200, 530, 800, 600);

        // Call javascript function within of the HTML file.
        EditorApplication.update = () =>
        {
            w.InvokeJSMethod("example", "changeText", Time.realtimeSinceStartup.ToString());
        };
    }

}
