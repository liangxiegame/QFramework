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
    [CustomEditor(typeof(SOGameObject))]
    [CanEditMultipleObjects]
    public class SOGameObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //Draw the defualt inspector options
            DrawDefaultInspector();

            SOGameObject script = (SOGameObject)target;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("Debugging Options", EditorStyles.centeredGreyMiniLabel);

            if (script.value != null)
            {
                EditorGUILayout.LabelField("Current value: " + script.value.name, EditorStyles.boldLabel);
            }
            else
            {
                EditorGUILayout.LabelField("Current value: ", EditorStyles.boldLabel);
            }
            //Display button that resets the value to the starting value
            if (GUILayout.Button("Reset Value"))
            {
                if (EditorApplication.isPlaying)
                {
                    script.ResetValue();
                }
            }
            EditorGUILayout.EndVertical();
        }

    }
}
#endif
