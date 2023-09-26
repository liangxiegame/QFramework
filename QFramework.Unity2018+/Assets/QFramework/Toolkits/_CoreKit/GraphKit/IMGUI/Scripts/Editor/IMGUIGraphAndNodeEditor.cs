/****************************************************************************
 * Copyright (c) 2017 Thor Brigsted UNDER MIT LICENSE  see licenses.txt 
 * Copyright (c) 2022 liangxiegame UNDER Paid MIT LICENSE  see licenses.txt
 *
 * xNode: https://github.com/Siccity/xNode
 ****************************************************************************/

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    /// <summary> Override graph inspector to show an 'Open Graph' button at the top </summary>
    [CustomEditor(typeof(GUIGraph), true)]
    public class GUIGlobalGraphInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (GUILayout.Button("Edit graph", GUILayout.Height(40)))
            {
                GUIGraphWindow.OpenWithGraph(serializedObject.targetObject as GUIGraph);
            }

            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            GUILayout.Label("Raw data", "BoldLabel");

            DrawDefaultInspector();

            serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomEditor(typeof(GUIGraphNode), true)]
    public class GlobalGUIGraphNodeInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (GUILayout.Button("Edit graph", GUILayout.Height(40)))
            {
                SerializedProperty graphProp = serializedObject.FindProperty("graph");
                GUIGraphWindow w = GUIGraphWindow.OpenWithGraph(graphProp.objectReferenceValue as GUIGraph);
                w.Home(); // Focus selected node
            }

            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            GUILayout.Label("Raw data", "BoldLabel");

            // Now draw the node itself.
            DrawDefaultInspector();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif