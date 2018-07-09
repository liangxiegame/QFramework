using System;
using UnityEngine;
namespace QFramework
{
    /// <summary>Mecanim事件系统事件配置类_for start+completed callback </summary>
    public class A_EventConfig_A : BaseEventConfig
    {
        public A_EventConfig_A(A_EventInfo eventInfo, int frame = -1) : base(eventInfo, frame) { }
        /// <summary>
        /// 为Clip添加Onstart回调事件
        /// </summary>
        /// <param name="onStart">回调</param>
        /// <returns>参数配置器</returns>
        public A_EventConfig_A OnStart(Action<AnimationEvent> onStart)
        {
            if (a_Event == null) return null;
            ConfigProcess(0, onStart);
            return this;
        }
        /// <summary>
        /// 为Clip添加OnCompleted回调事件
        /// </summary>
        /// <param name="OnCompleted">回调</param>
        /// <returns>参数配置器</returns>
        public  A_EventConfig_A OnCompleted(Action<AnimationEvent> onCompleted)
        {
            if (a_Event == null) return null;
            ConfigProcess(a_Event.totalFrames,onCompleted);
            return this;
        }
    }
    /// <summary>Mecanim事件系统事件配置类_For Process callback </summary>
    public class A_EventConfig_B : BaseEventConfig
    {
        public A_EventConfig_B(A_EventInfo eventInfo, int frame) : base(eventInfo, frame) { }
        public A_EventConfig_B OnProcess(Action<AnimationEvent> onProcess)
        {
            if (a_Event == null) return null;
            ConfigProcess(_keyFrame, onProcess);
            return this;
        }
    }
}
