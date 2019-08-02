using UnityEditor;
using UnityEngine;

namespace QF
{
    public class ScrollView : GUIView
    {
        public Vector2 ScrollPos = Vector2.one;

        public override void OnGUI()
        {
            if (!Visible) return;
            ScrollPos = EditorGUILayout.BeginScrollView(ScrollPos, mLayoutOptions.ToArray());
            base.OnGUI();
            EditorGUILayout.EndScrollView();
        }
    }
}