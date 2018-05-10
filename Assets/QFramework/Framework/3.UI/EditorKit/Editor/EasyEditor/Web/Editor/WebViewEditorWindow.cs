using UnityEditor;
using System;
using System.Reflection;

/// <summary>
/// Open a webview within Unity editor. (similar as AssetStore window)
/// </summary>
public class WebViewEditorWindow : EditorWindow//ScriptableObject
{
    static string Url = "http://google.com";

    [MenuItem("Window/WebViewWindow")]
    static void Open()
    {
        // "UnityEditor.Web.WebViewEditorWindow" does not work with Unity 5.5.x. Obsolete?
        string typeName = "UnityEditor.Web.WebViewEditorWindowTabs";

        // With Unity 5.5.x, calling UnityEngine.Types.GetType cause the following error:
        // error CS0619 : 'UnityEngine.Types.GetType (string, string)'is obsolete :
        //                `This was an internal method which is no longer used'
        Type type = Assembly.Load("UnityEditor.dll").GetType(typeName);

        BindingFlags Flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;
        var methodInfo = type.GetMethod("Create", Flags);
        methodInfo = methodInfo.MakeGenericMethod(type);

        methodInfo.Invoke(null, new object[] { "WebView", Url, 200, 530, 800, 600 });
    }
}