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

    [CreateAssetMenu(fileName = "Scriptable Int", menuName = "EventKit/Scriptable Int", order = 1)]
    public class ScriptableInt : ScriptableProperty, ISerializationCallbackReceiver
    {
        //Float value
        [NonSerialized]
        public int Value;

        //When the game starts, the starting value we use (so we can reset if need be)
        [SerializeField]
        private int startingValue = 0;

        /// <summary>
        /// Set sInt value
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(int value)
        {
            Value = value;
        }

        /// <summary>
        /// Set value to another sInt value
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(ScriptableInt value)
        {
            Value = value.Value;
        }

        /// <summary>
        /// Add a int value to the value
        /// </summary>
        /// <param name="value"></param>
        public void AddValue(int value)
        {
            Value += value;
        }

        /// <summary>
        /// Add another sInt value to the value
        /// </summary>
        /// <param name="value"></param>
        public void AddValue(ScriptableInt value)
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
    [CustomEditor(typeof(ScriptableInt))]
    [CanEditMultipleObjects]
    public class ScriptableIntEditor : Editor
    {
        private int intModifyValue = 0;
        
        public override void OnInspectorGUI()
        {
            //Draw the defualt inspector options
            DrawDefaultInspector();

            ScriptableInt script = (ScriptableInt)target;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("Debugging Options", EditorStyles.centeredGreyMiniLabel);

            EditorGUILayout.LabelField("Current value: " + script.Value, EditorStyles.boldLabel);

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
    #endif
}
