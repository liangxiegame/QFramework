/****************************************************************************
 * Copyright (c) 2015 - 2025 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections.Generic;

namespace QFramework
{
    public interface ISequence : IAction
    {
        ISequence Append(IAction action);
    }

    internal class Sequence : ISequence
    {
        private IAction mCurrentAction = null;
        private int mCurrentActionIndex = 0;
        private List<IAction> mActions = ListPool<IAction>.Get();

        private Sequence()
        {
        }

        private static SimpleObjectPool<Sequence> mSimpleObjectPool =
            new SimpleObjectPool<Sequence>(() => new Sequence(), null, 10);

        public static Sequence Allocate()
        {
            var sequence = mSimpleObjectPool.Allocate();
            sequence.ActionID = ActionKit.ID_GENERATOR++;
            sequence.Reset();
            sequence.Deinited = false;
            return sequence;
        }

        public bool Paused { get; set; }

        public bool Deinited { get; set; }

        public ulong ActionID { get; set; }
        public ActionStatus Status { get; set; }

        public void OnStart()
        {
            if (mActions.Count > 0)
            {
                mCurrentActionIndex = 0;
                mCurrentAction = mActions[mCurrentActionIndex];
                mCurrentAction.Reset();
                TryExecuteUntilNextNotFinished();
            }
            else
            {
                this.Finish();
            }
        }

        void TryExecuteUntilNextNotFinished()
        {
            while (mCurrentAction != null && mCurrentAction.Execute(0))
            {
                mCurrentActionIndex++;

                if (mCurrentActionIndex < mActions.Count)
                {
                    mCurrentAction = mActions[mCurrentActionIndex];
                    mCurrentAction.Reset();
                }
                else
                {
                    mCurrentAction = null;
                    this.Finish();
                }
            }
        }

        public void OnExecute(float dt)
        {
            if (mCurrentAction != null)
            {
                if (mCurrentAction.Execute(dt))
                {
                    mCurrentActionIndex++;

                    if (mCurrentActionIndex < mActions.Count)
                    {
                        mCurrentAction = mActions[mCurrentActionIndex];
                        mCurrentAction.Reset();

                        TryExecuteUntilNextNotFinished();
                    }
                    else
                    {
                        this.Finish();
                    }
                }
            }
            else
            {
                this.Finish();
            }
        }

        public void OnFinish()
        {
        }

        public ISequence Append(IAction action)
        {
            mActions.Add(action);
            return this;
        }

        public void Deinit()
        {
            if (!Deinited)
            {
                Deinited = true;
                
                foreach (var action in mActions)
                {
                    action.Deinit();
                }

                mActions.Clear();
                
                ActionQueue.AddCallback(new ActionQueueRecycleCallback<Sequence>(mSimpleObjectPool,this));
            }
        }

        public void Reset()
        {
            mCurrentActionIndex = 0;
            Status = ActionStatus.NotStart;
            Paused = false;
            foreach (var action in mActions)
            {
                action.Reset();
            }
        }
    }
    
    public static class SequenceExtension
    {
        public static ISequence Sequence(this ISequence self, Action<ISequence> sequenceSetting)
        {
            var repeat = QFramework.Sequence.Allocate();
            sequenceSetting(repeat);
            return self.Append(repeat);
        }
    }
}