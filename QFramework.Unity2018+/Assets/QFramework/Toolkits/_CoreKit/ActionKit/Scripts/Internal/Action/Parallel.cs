/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public interface IParallel : ISequence
    {
        
    }
    internal class Parallel : IParallel
    {
        private Parallel(){}

        private static SimpleObjectPool<Parallel> mSimpleObjectPool =
            new SimpleObjectPool<Parallel>(() => new Parallel(), null, 5);

        private List<IAction> mActions = ListPool<IAction>.Get();

        private int mFinishedCount = 0;
        
        public static Parallel Allocate()
        {
            var parallel = mSimpleObjectPool.Allocate();
            parallel.ActionID = ActionKit.ID_GENERATOR++;
            parallel.Deinited = false;
            parallel.Reset();
            return parallel;
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
            for (var i = mFinishedCount; i < mActions.Count; i++)
            {
                if (!mActions[i].Execute(dt)) continue;
                
                mFinishedCount++;

                if (mFinishedCount >= mActions.Count)
                {
                    this.Finish();
                }
                else
                {
                    // swap
                    (mActions[i], mActions[mFinishedCount - 1]) = (mActions[mFinishedCount - 1], mActions[i]);
                }

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
                
                ActionQueue.AddCallback(new ActionQueueRecycleCallback<Parallel>(mSimpleObjectPool,this));
            }

        }

        public void Reset()
        {
            Status = ActionStatus.NotStart;
            mFinishedCount = 0;
            Paused = false;
            foreach (var action in mActions)
            {
                action.Reset();
            }
            
        }
    }
    
    public static class ParallelExtension
    {
        public static ISequence Parallel(this ISequence self, Action<ISequence> parallelSetting)
        {
            var parallel = QFramework.Parallel.Allocate();
            parallelSetting(parallel);
            return self.Append(parallel);
        }
    }
}