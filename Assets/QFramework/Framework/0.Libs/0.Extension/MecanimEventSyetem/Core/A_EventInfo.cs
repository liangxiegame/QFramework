using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 事件订阅信息信息存储类
/// </summary>
public class A_EventInfo  {

    /// <summary>订阅事件所在的动画机</summary>
    public Animator animator;       
    /// <summary>动画机指定状态的动画片段</summary>
    public AnimationClip animationClip;
    /// <summary>动画片段的总帧数</summary>
    public int totalFrames;
    /// <summary>帧以及对应的回调链</summary>
    public Dictionary<int, Action<AnimationEvent>> frameCallBackPairs;
    /// <summary>帧以及对应的事件</summary>
    public Dictionary<int, AnimationEvent> frameEventPairs;

    public A_EventInfo(Animator anim, AnimationClip clip )
    {
        frameCallBackPairs = new Dictionary<int, Action<AnimationEvent>>();
        frameEventPairs = new Dictionary<int, AnimationEvent>();
        animator = anim;
        animationClip = clip;
        //向下取整（可以不需要），获得该动画片段的总帧数
        totalFrames = Mathf.CeilToInt(animationClip.frameRate* animationClip.length);
    }

    /// <summary>清除数据</summary>
    public void Clear()
    {
        animationClip.events = default(AnimationEvent[]);
        frameCallBackPairs=new Dictionary<int, Action<AnimationEvent>> ();
        frameEventPairs=new Dictionary<int, AnimationEvent> ();
        animationClip = null;
        animator = null;
    }

}
