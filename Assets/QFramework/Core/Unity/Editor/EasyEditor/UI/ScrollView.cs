using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public class ScrollView : EditorView
    {
        public Vector2 ScrollPos = Vector2.one;

        public override void OnGUI()
        {
            if (!Visible) return;
            ScrollPos = EditorGUILayout.BeginScrollView(ScrollPos, mLayoutOptions);
            base.OnGUI();
            EditorGUILayout.EndScrollView();
        }
    }
}