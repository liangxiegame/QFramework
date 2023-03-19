/****************************************************************************
 * Copyright (c) 2016 - 2022 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace QFramework.Experimental
{

    [CreateAssetMenu(fileName = "Scriptable String", menuName = "EventKit/Scriptable String", order = 1)]
    public class ScriptableString : ScriptableProperty, ISerializationCallbackReceiver
    {
        //Float value
        [NonSerialized]
        public string Value;

        //When the game starts, the starting value we use (so we can reset if need be)
        [SerializeField]
        private string startingValue = null;

        /// <summary>
        /// Set sString value
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Set value to another sString value
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(ScriptableString value)
        {
            Value = value.Value;
        }

        /// <summary>
        /// Recieve callback after unity deseriallzes the object
        /// </summary>
        public void OnAfterDeserialize()
        {
            Value = startingValue;
        }

        public void OnBeforeSerialize() { }

        /// <summary>
        /// Reset the value to it's inital value if it's resettable
        /// </summary>
        public override void ResetValue()
        {
            Value = startingValue;
        }
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(ScriptableString))]
    [CanEditMultipleObjects]
    public class ScriptableStringEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //Draw the defualt inspector options
            DrawDefaultInspector();

            ScriptableString script = (ScriptableString)target;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("Debugging Options", EditorStyles.centeredGreyMiniLabel);

            EditorGUILayout.LabelField("Current value: " + script.Value, EditorStyles.boldLabel);

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
    #endif
}