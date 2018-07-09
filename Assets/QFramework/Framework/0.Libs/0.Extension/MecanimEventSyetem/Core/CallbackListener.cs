using System;
using UnityEngine;

namespace QFramework
{
    public class CallbackListener : MonoBehaviour
    {
        Animator animator;
        A_EventHandler eventHandler;
        void Start()
        {
            eventHandler = A_EventHandler.Handler; 
            animator = GetComponent<Animator>();
        }
        /// <summary>通用事件回调</summary>
        /// <param name="ae">事件传递的参数信息</param>
        private void AnimatorEventCallBack(AnimationEvent ae)
        {
            AnimationClip clip = ae.animatorClipInfo.clip;//动画片段名称
            int currentFrame = Mathf.CeilToInt(ae.animatorClipInfo.clip.frameRate* ae.time);  //动画片段当前帧
            Action<AnimationEvent> action = eventHandler.GetAction(animator,clip,currentFrame);
            if (null!=action)
            {
                action(ae);
            }
        }
    }
}
