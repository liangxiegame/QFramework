/****************************************************************************
 * Copyright (c) 2015 - 2024 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using UnityEngine;

namespace QFramework
{
    internal class Delay : IAction
    {
        public float DelayTime;

        public Func<float> DelayTimeFactory = null;

        public System.Action OnDelayFinish { get; set; }

        public float CurrentSeconds { get; set; }

        private Delay()
        {
        }

        private static readonly SimpleObjectPool<Delay> mPool =
            new SimpleObjectPool<Delay>(() => new Delay(), null, 10);

        public static Delay Allocate(float delayTime, System.Action onDelayFinish = null)
        {
            var retNode = mPool.Allocate();
            retNode.ActionID = ActionKit.ID_GENERATOR++;
            retNode.Deinited = false;
            retNode.Reset();
            retNode.DelayTime = delayTime;
            retNode.OnDelayFinish = onDelayFinish;
            retNode.CurrentSeconds = 0.0f;
            return retNode;
        }
        
        public static Delay Allocate(Func<float> delayTimeFactory, System.Action onDelayFinish = null)
        {
            var retNode = mPool.Allocate();
            retNode.Deinited = false;
            retNode.Reset();
            retNode.DelayTimeFactory = delayTimeFactory;
            retNode.OnDelayFinish = onDelayFinish;
            retNode.CurrentSeconds = 0.0f;
            return retNode;
        }


        public ulong ActionID { get; set; }
        public ActionStatus Status { get; set; }

        public void OnStart()
        {
            if (DelayTimeFactory != null)
            {
                DelayTime = DelayTimeFactory();
            }
        }

        public void OnExecute(float dt)
        {
            if (CurrentSeconds >= DelayTime)
            {
                this.Finish();
                OnDelayFinish?.Invoke();
            }
            
            CurrentSeconds += dt;
        }

        public void OnFinish()
        {
        }

        public void Reset()
        {
            Status = ActionStatus.NotStart;
            Paused = false;
            CurrentSeconds = 0.0f;
        }

        public bool Paused { get; set; }

        public void Deinit()
        {
            if (!Deinited)
            {
                OnDelayFinish = null;
                Deinited = true;
                ActionQueue.AddCallback(new ActionQueueRecycleCallback<Delay>(mPool,this));
            }
        }

        public bool Deinited { get; set; }
    }
    
    public static class DelayExtension
    {
        public static ISequence Delay(this ISequence self, float seconds,Action onDelayFinish = null)
        {
            return self.Append(QFramework.Delay.Allocate(seconds,onDelayFinish));
        }
        
        public static ISequence Delay(this ISequence self,Func<float> delayTimeFactory,Action onDelayFinish = null)
        {
            return self.Append(QFramework.Delay.Allocate(delayTimeFactory,onDelayFinish));
        }
    }
}