using System;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

namespace SnakeGame
{
    public interface ITimeSystem : ISystem
    {
        float CurrentSeconds { get; }
        DelayTask AddDelayTask(float seconds, Action onDelayFinish, bool isContinue = false);
    }
    public class TimeSystem : AbstractSystem, ITimeSystem
    {
        private float mCurrentSeconds = 0;

        private Queue<DelayTask> mTaskPool = new Queue<DelayTask>();
        private LinkedList<DelayTask> mDelayTasks = new LinkedList<DelayTask>();

        protected override void OnInit() => CommonMono.AddUpdateAction(OnUpdate);

        float ITimeSystem.CurrentSeconds => mCurrentSeconds;
        DelayTask ITimeSystem.AddDelayTask(float seconds, Action onDelayFinish, bool isContinue)
        {
            DelayTask delayTask = mTaskPool.Count > 0 ? mTaskPool.Dequeue() : new DelayTask();
            delayTask.Init(seconds, onDelayFinish, isContinue);
            mDelayTasks.AddLast(delayTask);
            return delayTask;
        }
        private void OnUpdate()
        {
            mCurrentSeconds += Time.deltaTime;
            if (mDelayTasks.Count == 0) return;
            var currentNode = mDelayTasks.First;
            while (currentNode != null)
            {
                var nextNode = currentNode.Next;
                if (currentNode.Value.UpdateTasks(mCurrentSeconds))
                {
                    mTaskPool.Enqueue(currentNode.Value);
                    mDelayTasks.Remove(currentNode);
                }
                currentNode = nextNode;
            }
        }
    }
    public class DelayTask
    {
        private float Seconds;
        private Action OnFinish;
        private float StartTime;
        private float FinishTime;
        private bool mIsStart;
        private bool mIsLoop;

        public void Init(float seconds, Action onFinish, bool isLoop)
        {
            Seconds = seconds;
            OnFinish = onFinish;
            mIsStart = false;
            mIsLoop = isLoop;
        }
        public void StopTask() => mIsLoop = false;

        public bool UpdateTasks(float currentSeconds)
        {
            if (!mIsStart)
            {
                mIsStart = true;
                StartTime = currentSeconds;
                FinishTime = StartTime + Seconds;
            }
            else if (currentSeconds >= FinishTime)
            {
                OnFinish();
                if (mIsLoop)
                {
                    mIsStart = false;
                    return false;
                }
                OnFinish = null;
                return true;
            }
            return false;
        }
    }
}