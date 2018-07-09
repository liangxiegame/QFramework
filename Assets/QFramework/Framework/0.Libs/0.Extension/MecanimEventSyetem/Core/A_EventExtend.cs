using UnityEngine;

namespace QFramework
{
    public static class A_EventExtend
    {
        /// <summary>
        /// 指定需要绑定回调的AnimationClip
        /// </summary>
        /// <param name="animator">动画机</param>
        /// <param name="clipName">动画片段</param>
        /// <returns>事件配置器</returns>
        public static A_EventConfig_A SetTarget(this Animator animator, string clipName)
        {
            A_EventInfo a_EventInfo = A_EventHandler.Handler.GenerAnimationInfo(animator, clipName);
            if (null != a_EventInfo)
            {
                if (null == animator.GetComponent<CallbackListener>())
                {
                    animator.gameObject.AddComponent<CallbackListener>();
                }
            }
            //获得需要处理的动画片段
            return new A_EventConfig_A(a_EventInfo);
        }
        public static A_EventConfig_B SetTarget(this Animator animator, string clipName, int frame)
        {
            A_EventInfo a_EventInfo = A_EventHandler.Handler.GenerAnimationInfo(animator, clipName);
            if (null != a_EventInfo)
            {
                if (null == animator.GetComponent<CallbackListener>())
                {
                    animator.gameObject.AddComponent<CallbackListener>();
                }
            }
            //获得需要处理的动画片段
            return new A_EventConfig_B(a_EventInfo, frame);
        }
    }
}
