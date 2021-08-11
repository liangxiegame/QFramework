using System;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    [CustomEditor(typeof(ActionKitVisualAction),true)]
    public class ActionKitVisualActionEditor : EasyInspectorEditor
    {
        private bool mFoldOut = true;
        public Action OnDeleteAction;
        public Action OnUpButtonDraw;
        public Action OnDownButtonDraw;

        public override void OnInspectorGUI()
        {

            EditorGUILayout.BeginHorizontal("box");
            GUILayout.Space(10);
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            mFoldOut = EditorGUILayout.Foldout(mFoldOut, target.GetType().Name);
            GUILayout.FlexibleSpace();
            
            OnUpButtonDraw.InvokeGracefully();
            OnDownButtonDraw.InvokeGracefully();
            
            if (GUILayout.Button("-"))
            {
                OnDeleteAction.InvokeGracefully();
            }
            
            EditorGUILayout.EndHorizontal();

            if (mFoldOut)
            {
                base.OnInspectorGUI();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
    }
}