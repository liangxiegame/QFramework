/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;

namespace QFramework
{
    internal class Delay : IAction
    {
        public float DelayTime;

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
            retNode.Deinited = false;
            retNode.Reset();
            retNode.DelayTime = delayTime;
            retNode.OnDelayFinish = onDelayFinish;
            retNode.CurrentSeconds = 0.0f;
            return retNode;
        }
        

        public ActionStatus Status { get; set; }

        public void OnStart()
        {
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
            CurrentSeconds = 0.0f;
        }

        public bool Paused { get; set; }

        public void Deinit()
        {
            if (!Deinited)
            {
                OnDelayFinish = null;
                Deinited = true;
                mPool.Recycle(this);
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
    }
}