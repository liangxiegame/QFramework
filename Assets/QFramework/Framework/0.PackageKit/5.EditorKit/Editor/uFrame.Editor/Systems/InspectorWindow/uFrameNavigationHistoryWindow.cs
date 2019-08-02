using QF.GraphDesigner;
using UnityEditor;
using UnityEngine;

public class uFrameNavigationHistoryWindow : EditorWindow
{
    private Vector2 _scrollPosition;

    [MenuItem("Window/uFrame/Navigation History #&l")]
    internal static void ShowWindow()
    {
        var window = GetWindow<uFrameNavigationHistoryWindow>();
        window.title = "Nav History";
        Instance = window;
        window.Show();
    }

    public static uFrameNavigationHistoryWindow Instance { get; set; }

    public void OnGUI()
    {
        Instance = this;
        var rect = new Rect(0f, 0f, this.position.width, this.position.height);

        GUILayout.BeginArea(rect);
        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
        InvertApplication.SignalEvent<IDrawNavigationHistory>(_ => _.DrawNavigationHistory(rect));
        GUILayout.EndScrollView();
        GUILayout.EndArea();

    }

    public void Update()
    {
        Repaint();
    }

}