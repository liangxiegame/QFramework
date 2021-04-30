using System;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    [CustomEditor(typeof(ActionKitVisualAction),true)]
    public class ActionKitVisualActionEditor : EasyInspectorEditor
    {
        private bool mFoldOut = false;
        public Action OnDeleteAction;
        
        public override void OnInspectorGUI()
        {

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            mFoldOut = EditorGUILayout.Foldout(mFoldOut, target.GetType().Name);
            GUILayout.FlexibleSpace();
            
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