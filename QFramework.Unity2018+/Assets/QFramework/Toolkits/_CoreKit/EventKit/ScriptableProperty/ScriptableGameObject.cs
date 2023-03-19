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
    [CreateAssetMenu(fileName = "Scriptable GameObject", menuName = "EventKit/Scriptable GameObject", order = 1)]
    public class ScriptableGameObject : ScriptableProperty, ISerializationCallbackReceiver
    {
        //Float value
        [NonSerialized]
        public GameObject Value;

        //When the game starts, the starting value we use (so we can reset if need be)
        [SerializeField]
        private GameObject startingValue = null;

        /// <summary>
        /// Set sGameObject value
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(GameObject value)
        {
            Value = value;
        }

        /// <summary>
        /// Set value to another sGameObject value
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(ScriptableGameObject value)
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
    [CustomEditor(typeof(ScriptableGameObject))]
    [CanEditMultipleObjects]
    public class ScriptableGameObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //Draw the defualt inspector options
            DrawDefaultInspector();

            ScriptableGameObject script = (ScriptableGameObject)target;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.LabelField("Debugging Options", EditorStyles.centeredGreyMiniLabel);

            if (script.Value != null)
            {
                EditorGUILayout.LabelField("Current value: " + script.Value.name, EditorStyles.boldLabel);
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
    #endif
}
