/****************************************************************************
 * Copyright (c) 2016 - 2022 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace QFramework.Experimental
{
    public class ScriptableObjectEventListener : MonoBehaviour
    {
        public ScriptableObjectEvent Event; //Which event does this listen for
        [FormerlySerializedAs("Response")] public UnityEvent OnEvent; //Reponse to happen when the event is raised

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
}
