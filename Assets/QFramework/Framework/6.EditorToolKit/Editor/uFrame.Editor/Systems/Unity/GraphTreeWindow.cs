using System.Globalization;
using System.Linq;
using Invert.Common;
using QF.GraphDesigner;
using UnityEditor;
using UnityEngine;
using Invert.uFrame.Editor;
namespace Assets.UnderConstruction.Editor
{
    public class GraphTreeWindow : EditorWindow
    {


        private Vector2 _scrollPosition;

        [MenuItem("Window/uFrame/Graph Explorer %T")]

        internal static void ShowWindow()
        {
            var window = GetWindow<GraphTreeWindow>();
            window.title = "Graph Explorer";
            window.Show();
            window.Repaint();
            window.Focus();
        }

        public static GraphTreeWindow Instance { get; set; }

        public void OnGUI()
        {
            Instance = this;
            var rect = new Rect(0f, 0f, this.position.width, this.position.height);

            GUILayout.BeginArea(rect);
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            InvertApplication.SignalEvent<IDrawGraphExplorer>(_ => _.DrawGraphExplorer(rect));
            GUILayout.EndScrollView();
            GUILayout.EndArea();

        }

        public void Update()
        {
            Repaint();
        }


    }
}