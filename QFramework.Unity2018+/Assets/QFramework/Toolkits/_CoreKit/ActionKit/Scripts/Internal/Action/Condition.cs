/****************************************************************************
 * Copyright (c) 2015 - 2024 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;

namespace QFramework
{
    public class ConditionAction : IAction
    {
        private Func<bool> mCondition;
        private Action mOnCondition;

        private static SimpleObjectPool<ConditionAction> mSimpleObjectPool =
            new SimpleObjectPool<ConditionAction>(() => new ConditionAction(), null, 10);
        
        private ConditionAction(){}

        public static ConditionAction Allocate(Func<bool> condition,Action onCondition = null)
        {
            var conditionAction = mSimpleObjectPool.Allocate();
            conditionAction.ActionID = ActionKit.ID_GENERATOR++;
            conditionAction.Deinited = false;
            conditionAction.Reset();
            conditionAction.mCondition = condition;
            conditionAction.mOnCondition = onCondition;
            return conditionAction;
        }

        public bool Paused { get; set; }
        public bool Deinited { get; set; }
        public ulong ActionID { get; set; }
        public ActionStatus Status { get; set; }
        public void OnStart()
        {
        }

        public void OnExecute(float dt)
        {
            if (mCondition.Invoke())
            {
                mOnCondition?.Invoke();
                this.Finish();
            }
        }

        public void OnFinish()
        {
        }

        public void Deinit()
        {
            if (!Deinited)
            {
                Deinited = true;
                mCondition = null;
                mOnCondition = null;
                ActionQueue.AddCallback(new ActionQueueRecycleCallback<ConditionAction>(mSimpleObjectPool,this));
            }
        }

        public void Reset()
        {
            Paused = false;
            Status = ActionStatus.NotStart;
        }
    }
    
    public static class ConditionExtension
    {
        public static ISequence Condition(this ISequence self, Func<bool> condition)
        {
            return self.Append(QFramework.ConditionAction.Allocate(condition));
        }
    }
}