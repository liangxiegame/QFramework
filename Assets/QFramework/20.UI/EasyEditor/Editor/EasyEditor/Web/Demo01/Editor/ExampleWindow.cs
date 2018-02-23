using QFramework;
using UnityEngine;
using UnityEditor;

public class ExampleWindow : CustomWebViewEditorWindow
{

    [MenuItem("Window/Example01")]
    private static void Open()
    {
//        CreateWebViewEditorWindow<ExampleWindow>(
//            "Example",
//            Application.dataPath + "/Web/HTML/index.html", 200, 530, 800, 600);
//        var path = Application.dataPath.Replace("Assets", string.Empty).CombinePath("WebApp/LearnVue/index.html");
        var path = Application.dataPath.Replace("Assets","WebApp/LearnVue") + "/index.html";

        CreateWebViewEditorWindow<ExampleWindow>(
            "Example",
            path, 200, 530,
            800, 600);

        QFramework.Log.I("path:{0}", path);
    }

    public void Play()
    {
        EditorApplication.isPlaying = !EditorApplication.isPlaying;
    }

    public void Pause()
    {
        EditorApplication.isPaused = !EditorApplication.isPaused;
    }

    public void Step()
    {
        EditorApplication.Step();
    }
}