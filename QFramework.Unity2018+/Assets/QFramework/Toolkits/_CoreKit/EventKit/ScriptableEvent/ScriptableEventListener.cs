/****************************************************************************
 * Copyright (c) 2016 - 2022 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace QFramework.Experimental
{
    public class ScriptableEventListener : MonoBehaviour
    {
        public ScriptableEvent Event; //Which event does this listen for
        public UnityEvent OnEvent; //Reponse to happen when the event is raised

        [TextArea]
        [Tooltip("What does this object do when the attached event is raised")]
        public string responseDescription = "[What does this object do in response to this event]";

        private void OnEnable()
        {
            //If the event is not null, register this component/gameObject
            if (Event != null)
            {
                Event.RegisterListener(this);
            }
        }

        private void OnDisable()
        {
            //If the event is not null, unregister this component/gameObject
            if (Event != null)
            {
                Event.UnregisterListener(this);
            }
        }

        /// <summary>
        /// Raise the response set to this event
        /// </summary>
        public void OnEventTrigger()
        {
            OnEvent.Invoke();
        }
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(ScriptableEventListener))]
    [CanEditMultipleObjects]
    public class ScriptableObjectEventListenerEditor : Editor
    {

        SerializedProperty mGameEvent;
        SerializedProperty mOnEvent;
        SerializedProperty mResponseDescription;

        private void OnEnable()
        {
            mGameEvent = serializedObject.FindProperty("Event");
            mOnEvent = serializedObject.FindProperty("OnEvent");
            mResponseDescription = serializedObject.FindProperty("responseDescription");
        }

        public override void OnInspectorGUI()
        {
            ScriptableEventListener targetScript = (ScriptableEventListener)target;

            serializedObject.Update();

            EditorGUILayout.PropertyField(mGameEvent);

            if (targetScript.Event != null) //If the gameEvent value is not empty - get the gameEvent description
            {
                GUIStyle boldStyle = new GUIStyle();
                boldStyle.richText = true;

                EditorGUILayout.LabelField("Event Description", targetScript.Event.eventDescription, EditorStyles.helpBox);
            }

            EditorGUILayout.PropertyField(mResponseDescription);

            EditorGUILayout.PropertyField(mOnEvent);

            serializedObject.ApplyModifiedProperties();
        }


    }
    #endif
}
