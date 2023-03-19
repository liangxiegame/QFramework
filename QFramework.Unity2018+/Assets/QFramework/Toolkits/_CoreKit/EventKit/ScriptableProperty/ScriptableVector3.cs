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
    [CreateAssetMenu(fileName = "Scriptable Vector3", menuName = "EventKit/Scriptable Vector3", order = 1)]
    public class ScriptableVector3 : ScriptableProperty, ISerializationCallbackReceiver
    {
        //Float value
        [NonSerialized]
        public Vector3 Value;

        //Can the value be reset in game
        //public bool resettable;

        //When the game starts, the starting value we use (so we can reset if need be)
        [SerializeField]
        private Vector3 startingValue = Vector3.zero;

        /// <summary>
        /// Set sVector3 value
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(Vector3 value)
        {
            Value = value;
        }

        /// <summary>
        /// Set value to another sVector3 value
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(ScriptableVector3 value)
        {
            Value = value.Value;
        }

        /// <summary>
        /// Add a Vector3 value to the value
        /// </summary>
        /// <param name="value"></param>
        public void AddValue(Vector3 value)
        {
            Value += value;
        }

        /// <summary>
        /// Add another sVector3 value to the value
        /// </summary>
        /// <param name="value"></param>
        public void AddValue(ScriptableVector3 value)
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
    [CustomEditor(typeof(ScriptableVector3))]
    [CanEditMultipleObjects]
    public class ScriptableVector3Editor : Editor
    {

        public override void OnInspectorGUI()
        {
            //Draw the defualt inspector options
            DrawDefaultInspector();

            ScriptableVector3 script = (ScriptableVector3)target;

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
