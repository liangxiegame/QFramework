using System;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public class A_EventHandler
    {
        #region 单例
        private static A_EventHandler _instance;
        /// <summary>获得事件处理类实例</summary>
        public static A_EventHandler Handler
        {
            get
            {
                if (null == _instance)
                {
                    _instance = new A_EventHandler();
                }
                return _instance;
            }
        }
        #endregion
        /// <summary>动画机及其事件信息Pairs</summary>
        private List<A_EventInfo> eventContainer =  new List<A_EventInfo>();

        private const string func = "AnimatorEventCallBack";

        /// <summary>
        /// 为事件基础信息进行缓存
        /// </summary>
        /// <param name="animator">动画机</param>
        /// <param name="clipName">动画片段名称</param>
        /// <param name="frame">指定帧</param>
        public A_EventInfo GenerAnimationInfo(Animator animator, string clipName)
        {
            AnimationClip clip = GetAnimationClip(animator, clipName);
            if (null == clip)return null;
            A_EventInfo a_EventInfo = GetEventInfo(animator, clip);  //获取指定事件信息类
            return a_EventInfo;
        }

        /// <summary>
        /// 为指定动画机片段插入回调方法 
        /// </summary>
        /// <param name="eventInfo">回调信息类</param>
        /// <param name="frame">指定帧</param>
        /// <param name="func">方法名</param>
        public void GenerAnimationEvent(A_EventInfo eventInfo, int frame)
        {
            if (frame < 0 || frame > eventInfo.totalFrames)
            {
                Debug.LogErrorFormat("AnimatorEventSystem[紧急]：【{0}】所在的动画机【{1}】片段帧数设置错误【{2}】！", eventInfo.animator.name, eventInfo.animationClip.name, frame);
                return;
            }
            float _time = frame / eventInfo.animationClip.frameRate;
            AnimationEvent[] events = eventInfo.animationClip.events;
            AnimationEvent varEvent = Array.Find(events, (v) => { return v.time == _time; });
            if (null != varEvent)
            {
                if (varEvent.functionName == func)
                {
                    Debug.LogWarningFormat("AnimatorEventSystem[一般]：【{0}】所在的动画机【{1}】片段【{2}】帧已存在回调方法，无需重复添加！", eventInfo.animator.name, eventInfo.animationClip.name, frame);
                    if (!eventInfo.frameEventPairs.ContainsKey(frame)) eventInfo.frameEventPairs.Add(frame, varEvent);
                    return;
                }
                else
                {
                    Debug.LogWarningFormat("AnimatorEventSystem[一般]：【{0}】所在的动画机【{1}】片段【{2}】帧已存在回调方法【{3}】，将自动覆盖！", eventInfo.animator.name, eventInfo.animationClip.name, frame, varEvent.functionName);
                }
            }
            AnimationEvent a_event = new AnimationEvent //创建事件对象
            {
                functionName = func, //指定事件的函数名称
                time = _time,  //对应动画指定帧处触发
                messageOptions = SendMessageOptions.DontRequireReceiver, //回调未找到不提示
            };
            eventInfo.animationClip.AddEvent(a_event); //绑定事件
            eventInfo.frameEventPairs.Add(frame, a_event);
            eventInfo.animator.Rebind(); //重新绑定动画器的所有动画的属性和网格数据。
        }

        /// <summary>数据重置，用于总管理类清理数据用</summary>
        public void Clear()
        {
            foreach (var item in eventContainer)
            {
                item.Clear();
            }
            eventContainer = new List<A_EventInfo>();
        }

        #region Helper Function
        /// <summary>
        /// 获得指定的事件信息类
        /// </summary>
        /// <param name="animator">动画机</param>
        /// <param name="clip">动画片段</param>
        /// <returns>事件信息类</returns>
        private A_EventInfo GetEventInfo(Animator animator, AnimationClip clip)
        {
            A_EventInfo a_EventInfo = eventContainer.Find((v) => { return v.animator == animator && v.animationClip == clip; });
            if (null == a_EventInfo)
            {
                a_EventInfo = new A_EventInfo(animator, clip);
                eventContainer.Add(a_EventInfo);
            }
            return a_EventInfo;
        }

        /// <summary>
        /// 根据动画片段名称从指定动画机获得动画片段
        /// </summary>
        /// <param name="animator">动画机</param>
        /// <param name="name">动画片段名称</param>
        /// <returns></returns>
        public AnimationClip GetAnimationClip(Animator animator, string name)
        {
            #region 异常提示
            if (null == animator)
            {
                Debug.LogError("AnimatorEventSystem[紧急]：指定Animator不得为空！");
                return null;
            }
            RuntimeAnimatorController runtimeAnimatorController = animator.runtimeAnimatorController;
            if (null == runtimeAnimatorController)
            {
                Debug.LogError("AnimatorEventSystem[紧急]：指定【"+animator.name +"】Animator未挂载Controller！");
                return null;
            }
            AnimationClip[] clips = runtimeAnimatorController.animationClips;
            AnimationClip[] varclip = Array.FindAll(clips, (v) => { return v.name == name; });
            if (null == varclip || varclip.Length == 0)
            {
                Debug.LogError("AnimatorEventSystem[紧急]：指定【" + animator.name + "】Animator不存在名为【" + name + "】的动画片段！");
                return null;
            }
            if (varclip.Length >= 2)
            {
                Debug.LogWarningFormat("AnimatorEventSystem[一般]：指定【{0}】Animator存在【{1}】个名为【{2}】的动画片段！\n 建议：若非复用导致的重名，请务必修正！否则，事件将绑定在找的第一个Clip上。",animator.name, varclip.Length, name);
            }
            #endregion
            return varclip[0];
        }
        /// <summary>
        /// 根据给定信息获得委托
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="clip"></param>
        /// <param name="frame"></param>
        /// <returns></returns>
        public Action<AnimationEvent> GetAction(Animator animator, AnimationClip clip, int frame)
        {
            Action<AnimationEvent> action = default(Action<AnimationEvent>);
            A_EventInfo a_EventInfo = eventContainer.Find((v) => { return v.animator == animator && v.animationClip == clip; });
            if (null != a_EventInfo)
            {
                a_EventInfo.frameCallBackPairs.TryGetValue(frame, out action);
            }
            return action;
        }
        #endregion 
    }
}
