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
    [CreateAssetMenu(fileName = "Scriptable Float", menuName = "EventKit/Scriptable Float", order = 1)]
    public class ScriptableFloat : ScriptableProperty, ISerializationCallbackReceiver
    {
        //Float value
        [NonSerialized]
        public float Value;

        //When the game starts, the starting value we use (so we can reset if need be)
        [SerializeField]
        private float startingValue = 0;

        /// <summary>
        /// Set sFloat value
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(float value)
        {
            Value = value;
        }

        /// <summary>
        /// Set value to another sBool value
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(ScriptableFloat value)
        {
            Value = value.Value;
        }

        /// <summary>
        /// Add a float value to the value
        /// </summary>
        /// <param name="value"></param>
        public void AddValue(float value)
        {
            Value += value;
        }

        /// <summary>
        /// Add another sFloat value to the value
        /// </summary>
        /// <param name="value"></param>
        public void AddValue(ScriptableFloat value)
        {
            Value += value.Value;
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
    [CustomEditor(typeof(ScriptableFloat))]
    [CanEditMultipleObjects]
    public class ScriptableFloatEditor : Editor
    {
        private float floatModifyValue = 0.0f;

        public override void OnInspectorGUI()
        {
            //Draw the defualt inspector options
            DrawDefaultInspector();

            ScriptableFloat script = (ScriptableFloat)target;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("Debugging Options", EditorStyles.centeredGreyMiniLabel);

            EditorGUILayout.LabelField("Current value: " + script.Value, EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            //Display a float input field and button to add the inputted value to the current value
            floatModifyValue = EditorGUILayout.FloatField("Modify current value by: ", floatModifyValue);

            if (GUILayout.Button("Modify"))
            {
                script.AddValue(floatModifyValue);
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
    #endif
}
