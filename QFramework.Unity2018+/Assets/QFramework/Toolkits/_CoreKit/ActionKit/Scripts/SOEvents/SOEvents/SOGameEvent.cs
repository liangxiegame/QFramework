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
    [CreateAssetMenu(fileName = "soGameEvent", menuName = "soGameEvents/soGameEvent", order = 1)]
    public class SOGameEvent : ScriptableObject
    {
        //List of all objects/methods subscribed to this GameEvent
        private List<GameEventListener> listeners = new List<GameEventListener>();

        //Description of when this event is raised
        [TextArea]
        [Tooltip("When is this event raised")]
        public string eventDescription = "[When does this event trigger]";

        public void Raise()
        {
#if UNITY_EDITOR
            //Debug - show the event has been raised
            Debug.Log(this.name + " event raised");
#endif
            //Loop through the listener list and raise the events passed
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised();
            }
        }

        //Add the gameEventListener to the listener list
        public void RegisterListener(GameEventListener listener)
        {
            listeners.Add(listener);
        }

        //Remove the gameEventListener to the listener list
        public void UnregisterListener(GameEventListener listener)
        {
            listeners.Remove(listener);
        }
    }
}
