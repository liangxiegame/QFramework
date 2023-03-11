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
    [CustomEditor(typeof(SOInt))]
    [CanEditMultipleObjects]
    public class SOIntEditor : Editor
    {
        private int intModifyValue = 0;

        public override void OnInspectorGUI()
        {
            //Draw the defualt inspector options
            DrawDefaultInspector();

            SOInt script = (SOInt)target;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("Debugging Options", EditorStyles.centeredGreyMiniLabel);

            EditorGUILayout.LabelField("Current value: " + script.value, EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            //Display a int input field and button to add the inputted value to the current value
            intModifyValue = EditorGUILayout.IntField("Modify current value by: ", intModifyValue);

            if (GUILayout.Button("Modify"))
            {
                script.AddValue(intModifyValue);
            }

            EditorGUILayout.EndHorizontal();

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