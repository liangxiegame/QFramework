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
    [CreateAssetMenu(fileName = "Scriptable Bool", menuName = "EventKit/Scriptable Bool", order = 1)]
    public class ScriptableBool : ScriptableProperty, ISerializationCallbackReceiver
    {
        [NonSerialized]
        public bool Value;

        //Can the value be reset in game
        //public bool resettable;

        //When the game starts, the starting value we use (so we can reset if need be)
        [SerializeField]
        private bool startingValue = false;

        /// <summary>
        /// Set sBool value
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(bool value)
        {
            Value = value;
        }

        /// <summary>
        /// Set value to another sBool value
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(ScriptableBool value)
        {
            Value = value.Value;
        }

        /// <summary>
        /// Swap the bool value
        /// </summary>
        public void Toggle()
        {
            Value = !Value;
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
    [CustomEditor(typeof(ScriptableBool))]
    [CanEditMultipleObjects]
    public class ScriptableBoolEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //Draw the defualt inspector options
            DrawDefaultInspector();

            ScriptableBool script = (ScriptableBool)target;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("Debugging Options", EditorStyles.centeredGreyMiniLabel);

            EditorGUILayout.LabelField("Current value: " + script.Value, EditorStyles.boldLabel);

            //Display button that toggles the bool value
            if (GUILayout.Button("Toggle Value"))
            {
                if (EditorApplication.isPlaying)
                {
                    script.Toggle();
                }
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
    #endif
}
