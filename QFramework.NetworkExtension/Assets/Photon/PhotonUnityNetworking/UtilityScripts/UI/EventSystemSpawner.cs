// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventSystemSpawner.cs" company="Exit Games GmbH">
// </copyright>
// <summary>
// For additive Scene Loading context, eventSystem can't be added to each scene and instead should be instantiated only if necessary.
// https://answers.unity.com/questions/1403002/multiple-eventsystem-in-scene-this-is-not-supporte.html
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.EventSystems;

namespace Photon.Pun.UtilityScripts
{
    /// <summary>
    /// Event system spawner. Will add an EventSystem GameObject with an EventSystem component and a StandaloneInputModule component.
    /// Use this in additive scene loading context where you would otherwise get a "Multiple EventSystem in scene... this is not supported" error from Unity.
    /// </summary>
    public class EventSystemSpawner : MonoBehaviour
    {
        void OnEnable()
        {
            #if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            Debug.LogError("PUN Demos are not compatible with the New Input System, unless you enable \"Both\" in: Edit > Project Settings > Player > Active Input Handling. Pausing App.");
            Debug.Break();
            return;
            #endif

            EventSystem sceneEventSystem = FindObjectOfType<EventSystem>();
            if (sceneEventSystem == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");

                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }
        }
    }
}