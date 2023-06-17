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
    public class Condition : IAction
    {
        private Func<bool> mCondition;

        private static SimpleObjectPool<Condition> mSimpleObjectPool =
            new SimpleObjectPool<Condition>(() => new Condition(), null, 10);
        
        private Condition(){}

        public static Condition Allocate(Func<bool> condition)
        {
            var conditionAction = mSimpleObjectPool.Allocate();
            conditionAction.ActionID = ActionKit.ID_GENERATOR++;
            conditionAction.Deinited = false;
            conditionAction.Reset();
            conditionAction.mCondition = condition;
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
                ActionQueue.AddCallback(new ActionQueueRecycleCallback<Condition>(mSimpleObjectPool,this));
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
            return self.Append(QFramework.Condition.Allocate(condition));
        }
    }
}