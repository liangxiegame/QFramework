using System;
using UnityEngine;

namespace QFramework
{
    public class OnBecameVisibleEventTrigger : MonoBehaviour
    {
        public readonly EasyEvent OnBecameVisibleEvent = new EasyEvent();

        private void OnBecameVisible()
        {
            OnBecameVisibleEvent.Trigger();
        }
    }

    public static class OnBecameVisibleEventTriggerExtension
    {
        public static IUnRegister OnBecameVisibleEvent<T>(this T self, Action onBecameVisible)
            where T : Component
        {
            return self.GetOrAddComponent<OnBecameVisibleEventTrigger>().OnBecameVisibleEvent
                .Register(onBecameVisible);
        }

        public static IUnRegister OnBecameVisibleEvent(this GameObject self, Action onBecameVisible)
        {
            return self.GetOrAddComponent<OnBecameVisibleEventTrigger>().OnBecameVisibleEvent
                .Register(onBecameVisible);
        }
    }
}