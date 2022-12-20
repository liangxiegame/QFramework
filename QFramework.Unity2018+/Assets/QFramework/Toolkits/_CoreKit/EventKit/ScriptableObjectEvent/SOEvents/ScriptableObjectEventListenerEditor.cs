/****************************************************************************
 * Copyright (c) 2016 - 2022 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;


namespace QFramework.Experimental
{

    [CustomEditor(typeof(ScriptableObjectEventListener))]
    [CanEditMultipleObjects]
    public class ScriptableObjectEventListenerEditor : Editor
    {

        SerializedProperty gameEvent;
        SerializedProperty response;
        SerializedProperty responseDescription;

        private void OnEnable()
        {
            gameEvent = serializedObject.FindProperty("Event");
            response = serializedObject.FindProperty("OnEvent");
            responseDescription = serializedObject.FindProperty("responseDescription");
        }

        public override void OnInspectorGUI()
        {
            ScriptableObjectEventListener targetScript = (ScriptableObjectEventListener)target;

            serializedObject.Update();

            EditorGUILayout.PropertyField(gameEvent);

            if (targetScript.Event != null) //If the gameEvent value is not empty - get the gameEvent description
            {
                GUIStyle boldStyle = new GUIStyle();
                boldStyle.richText = true;

                EditorGUILayout.LabelField("Event Description", targetScript.Event.eventDescription, EditorStyles.helpBox);
            }

            EditorGUILayout.PropertyField(responseDescription);

            EditorGUILayout.PropertyField(response);

            serializedObject.ApplyModifiedProperties();
        }


    }
}
#endif
