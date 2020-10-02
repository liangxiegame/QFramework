#if UNITY_5_6_OR_NEWER
using UnityEngine;
#endif

namespace QFramework
{
    public class ActionKit
    {
#if UNITY_5_6_OR_NEWER
        [RuntimeInitializeOnLoadMethod]
#endif
        private static void InitNodeSystem()
        {
            // cache list			

            // cache node
            SafeObjectPool<DelayAction>.Instance.Init(50, 50);
            SafeObjectPool<EventAction>.Instance.Init(50, 50);
        }
    }
}