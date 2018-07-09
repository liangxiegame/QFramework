using System;
using UnityEngine;

namespace QFramework
{ 
    /// <summary>
    /// 参数配置类，因为使用了链式编程思想（后期会改为基于方法扩展的链式编程）使得动画机的回调事件拥有闭包特性，所以不建议配置任何参数，除非您非要配合事件系统向外分发事件
    /// </summary>
    public class BaseEventConfig
    {
        protected AnimationEvent _ClipEvent;
        protected int _keyFrame;
        protected A_EventInfo a_Event;
        protected Animator _animator;

        public BaseEventConfig(A_EventInfo eventInfo, int frame)
        {
            _keyFrame = frame;
            a_Event = eventInfo;
            _animator = eventInfo.animator;
        }

        /// <summary>设置组合参数</summary>
        /// <param name="intParm">int参数</param>
        /// <param name="floatParm">float参数</param>
        /// <param name="stringParm">string参数(必填)</param>
        /// <param name="objectParm">Object参数</param>
        /// <returns></returns>
        public Animator SetParms(string stringParm, int intParm = default(int), float floatParm = default(float), UnityEngine.Object objectParm = default(UnityEngine.Object))
        {
            if (null == a_Event){ return _animator;}
            AnimationEvent _ClipEvent;
            a_Event.frameEventPairs.TryGetValue(_keyFrame, out _ClipEvent);
            if (null == _ClipEvent){ return _animator; }
            _ClipEvent.intParameter = intParm;
            _ClipEvent.floatParameter = floatParm;
            _ClipEvent.stringParameter = stringParm;
            _ClipEvent.objectReferenceParameter = objectParm;
            ResignEvent();
            return a_Event.animator;
        }

        /// <summary>
        /// 参数被变更，需要重新绑定所有的事件
        /// </summary>
        private void ResignEvent()
        {
            a_Event.animationClip.events = default(AnimationEvent[]); //被逼的，AnimationEvent不是简单的对象引用及其字段修改的问题，只能从新插入事件
            foreach (AnimationEvent item in a_Event.frameEventPairs.Values)
            {
                a_Event.animationClip.AddEvent(item);
            }
            a_Event.animator.Rebind();
        }

        /// <summary>
        /// 为指定帧加入回调链
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="action"></param>
        protected void ConfigProcess(int frame, Action<AnimationEvent> action)
        {
            if (null == action) return;
            _keyFrame = frame;
            if (!a_Event.frameCallBackPairs.ContainsKey(_keyFrame))
            {
                a_Event.frameCallBackPairs.Add(_keyFrame, action);
            }
            else
            {
                Action<AnimationEvent> t_action = a_Event.frameCallBackPairs[_keyFrame];
                if (null == t_action)
                {
                    a_Event.frameCallBackPairs[_keyFrame] = action;
                }
                else
                {
                    Delegate[] delegates = t_action.GetInvocationList();
                    if (Array.IndexOf(delegates, action) == -1)
                    {
                        a_Event.frameCallBackPairs[_keyFrame] += action;
                    }
                    else
                    {
                        Debug.LogWarningFormat("AnimatorEventSystem[一般]：指定AnimationClip【{0}】已经订阅了该事件【{1}】！\n 建议：请勿频繁订阅！", a_Event.animationClip.name,action.Method.Name);
                    }
                }
            }
            if (!a_Event.frameEventPairs.ContainsKey(_keyFrame))
            {
                A_EventHandler.Handler.GenerAnimationEvent(a_Event, _keyFrame);
            }
        }

        #region Adapter For Animator
        /// <summary>
        /// 设置动画机bool参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        /// <returns></returns>
        public Animator SetBool(string name, bool value)
        {
            _animator.SetBool(name, value);
            return _animator;
        }
        /// <summary>
        /// 设置动画机bool参数
        /// </summary>
        /// <param name="name">参数id</param>
        /// <param name="value">参数值</param>
        /// <returns></returns>
        public Animator SetBool(int id, bool value)
        {
            _animator.SetBool(id, value);
            return _animator;
        }
        /// <summary>
        /// 设置动画机float参数
        /// </summary>
        /// <param name="name">参数id</param>
        /// <param name="value">参数值</param>
        /// <returns></returns>
        public Animator SetFloat(int id, float value)
        {
            _animator.SetFloat(id, value);
            return _animator;
        }
        /// <summary>
        /// 设置动画机float参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        /// <returns></returns>
        public Animator SetFloat(string name, float value)
        {
            _animator.SetFloat(name, value);
            return _animator;
        }
        /// <summary>
        ///  设置动画机float参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        /// <param name="dampTime"></param>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public Animator SetFloat(string name, float value, float dampTime, float deltaTime)
        {
            _animator.SetFloat(name, value, dampTime, deltaTime);
            return _animator;
        }
        /// <summary>
        /// 设置动画机float参数
        /// </summary>
        /// <param name="name">参数id</param>
        /// <param name="value">参数值</param>
        /// <returns></returns>
        public Animator SetFloat(int id, float value, float dampTime, float deltaTime)
        {
            _animator.SetFloat(id, value, dampTime, deltaTime);
            return _animator;
        }
        /// <summary>
        /// 设置动画机trigger参数
        /// </summary>
        /// <param name="name">参数id</param>
        /// <returns></returns>
        public Animator SetTrigger(int id)
        {
            _animator.SetTrigger(id);
            return _animator;
        }
        /// <summary>
        /// 设置动画机trigger参数
        /// </summary>
        /// <param name="name">参数name</param>
        /// <returns></returns>
        public Animator SetTrigger(string name)
        {
            _animator.SetTrigger(name);
            return _animator;
        }
        #endregion
    }
}