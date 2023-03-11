/****************************************************************************
 * Copyright (c) 2016 - 2022 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace QFramework.Experimental
{
    [CreateAssetMenu(fileName = "Scriptable Object Event", menuName = "EventKit/Scriptable Object Event", order = 1)]
    public class ScriptableObjectEvent : ScriptableObject
    {
        //List of all objects/methods subscribed to this GameEvent
        private List<ScriptableObjectEventListener> mListeners = new List<ScriptableObjectEventListener>();

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
        public void RegisterListener(ScriptableObjectEventListener listener)
        {
            mListeners.Add(listener);
        }

        //Remove the gameEventListener to the listener list
        public void UnregisterListener(ScriptableObjectEventListener listener)
        {
            mListeners.Remove(listener);
        }
    }
}
