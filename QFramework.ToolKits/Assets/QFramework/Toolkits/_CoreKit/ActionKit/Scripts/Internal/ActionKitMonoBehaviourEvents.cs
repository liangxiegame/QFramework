/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using UnityEngine;

namespace QFramework
{
    [MonoSingletonPath("QFramework/ActionKit/GlobalMonoBehaviourEvents")]
    internal class ActionKitMonoBehaviourEvents : MonoSingleton<ActionKitMonoBehaviourEvents>
    {
        internal readonly EasyEvent OnUpdate = new EasyEvent();
        internal readonly EasyEvent OnFixedUpdate = new EasyEvent();
        internal readonly EasyEvent OnLateUpdate = new EasyEvent();
        internal readonly EasyEvent OnGUIEvent = new EasyEvent();
        internal readonly EasyEvent<bool> OnApplicationFocusEvent = new EasyEvent<bool>();
        internal readonly EasyEvent<bool> OnApplicationPauseEvent = new EasyEvent<bool>();
        internal readonly EasyEvent OnApplicationQuitEvent = new EasyEvent();

        private void Awake()
        {
            hideFlags = HideFlags.HideInHierarchy;
        }

        private void Update()
        {
            OnUpdate?.Trigger();
        }

        private void OnGUI()
        {
            OnGUIEvent?.Trigger();
        }

        private void FixedUpdate()
        {
            OnFixedUpdate?.Trigger();
        }

        private void LateUpdate()
        {
            OnLateUpdate?.Trigger();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            OnApplicationFocusEvent?.Trigger(hasFocus);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            OnApplicationPauseEvent?.Trigger(pauseStatus);
        }

        protected override void OnApplicationQuit()
        {
            OnApplicationQuitEvent?.Trigger();
            base.OnApplicationQuit();
        }
    }
}