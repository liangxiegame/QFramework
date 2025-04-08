/****************************************************************************
 * Copyright (c) 2015 - 2025 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;

namespace QFramework
{
    public class ConditionSequence : IAction
    {
        private Func<bool> mCondition;
        private Action<ISequence> mSequenceSetting;
        private ISequence mSequence;

        private static SimpleObjectPool<ConditionSequence> mSimpleObjectPool =
            new SimpleObjectPool<ConditionSequence>(() => new ConditionSequence(), null, 10);
        
        private ConditionSequence(){}

        public static ConditionSequence Allocate(Func<bool> condition,Action<ISequence> sequenceSetting)
        {
            var whenAction = mSimpleObjectPool.Allocate();
            whenAction.ActionID = ActionKit.ID_GENERATOR++;
            whenAction.Deinited = false;
            whenAction.Reset();
            whenAction.mCondition = condition;
            whenAction.mSequenceSetting = sequenceSetting;
            return whenAction;
        }

        public bool Paused { get; set; }
        public bool Deinited { get; set; }
        public ulong ActionID { get; set; }
        public ActionStatus Status { get; set; }
        public void OnStart()
        {
            if (mCondition())
            {
                mSequence = ActionKit.Sequence();
                mSequenceSetting.Invoke(mSequence);
            }
            else
            {
                this.Finish();
            }
        }

        public void OnExecute(float dt)
        {
            if (mSequence != null)
            {
                if (mSequence.Execute(dt))
                {
                    this.Finish();
                }
            }
        }

        public void OnFinish()
        {
            if (mSequence != null)
            {
                mSequence.Deinit();
                mSequence = null;
            }
        }

        public void Deinit()
        {
            if (!Deinited)
            {
                Deinited = true;
                mCondition = null;
                mSequence = null;
                mSequenceSetting = null;
                ActionQueue.AddCallback(new ActionQueueRecycleCallback<ConditionSequence>(mSimpleObjectPool,this));
            }
        }

        public void Reset()
        {
            Paused = false;
            Status = ActionStatus.NotStart;
        }
    }
    
    public static class ConditionSequenceExtension
    {
        public static ISequence ConditionSequence(this ISequence self, Func<bool> condition,Action<ISequence> sequenceSetting)
        {
            return self.Append(QFramework.ConditionSequence.Allocate(condition,sequenceSetting));
        }
    }
}