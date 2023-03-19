/****************************************************************************
 * Copyright (c) 2016 - 2022 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace QFramework.Experimental
{
    [CreateAssetMenu(fileName = "Scriptable Event", menuName = "EventKit/Scriptable Event", order = 1)]
    public class ScriptableEvent : ScriptableObject
    {
        //List of all objects/methods subscribed to this GameEvent
        private List<ScriptableEventListener> mListeners = new List<ScriptableEventListener>();

        //Description of when this event is raised
        [TextArea]
        [Tooltip("When is this event raised")]
        public string eventDescription = "[When does this event trigger]";

        public void Trigger()
        {
#if UNITY_EDITOR
            //Debug - show the event has been raised
            Debug.Log(this.name + " event raised");
#endif
            //Loop through the listener list and raise the events passed
            for (int i = mListeners.Count - 1; i >= 0; i--)
            {
                mListeners[i].OnEventTrigger();
            }
        }

        //Add the gameEventListener to the listener list
        public void RegisterListener(ScriptableEventListener listener)
        {
            mListeners.Add(listener);
        }

        //Remove the gameEventListener to the listener list
        public void UnregisterListener(ScriptableEventListener listener)
        {
            mListeners.Remove(listener);
        }
    }
    
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(ScriptableEvent))]
    [CanEditMultipleObjects]
    public class ScriptableObjectEventEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //Draw the defualt inspector options
            DrawDefaultInspector();

            ScriptableEvent script = (ScriptableEvent)target;

            //Draw button
            if (GUILayout.Button("Trigger Event"))
            {
                //If the application is playing - raise/trigger the event
                if (EditorApplication.isPlaying)
                {
                    script.Trigger();
                }
            }
        }
    }
    #endif
}
