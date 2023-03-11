using System;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace ShootingEditor2D
{
    public interface ITimeSystem : ISystem
    {
        float CurrentSeconds { get; }
        void AddDelayTask(float seconds, Action onDelayFinish);
    }

    public class DelayTask
    {
        public float Seconds { get; set; }
        public Action OnFinish { get; set; }
        
        public float StartSeconds { get; set; }
        public float FinishSeconds { get; set; }
        public DelayTaskState State { get; set; }
    }

    public enum DelayTaskState
    {
        NotStart,
        Started,
        Finish
    }

    public class TimeSystem : AbstractSystem, ITimeSystem
    {
        public class TimeSystemUpdateBehaviour : MonoBehaviour
        {
            public event Action OnUpdate;

            private void Update()
            {
                OnUpdate?.Invoke();
            }
        }
        
        protected override void OnInit()
        {
            var updateBehaviourGameObj = new GameObject(nameof(TimeSystemUpdateBehaviour));
            var updateBehaviour = updateBehaviourGameObj.AddComponent<TimeSystemUpdateBehaviour>();

            updateBehaviour.OnUpdate += OnUpdate;
                
            CurrentSeconds = 0;
        }

        void OnUpdate()
        {
            CurrentSeconds += Time.deltaTime;

            if (mDelayTasks.Count > 0)
            {
                var currentNode = mDelayTasks.First;

                while (currentNode != null)
                {
                    var nextNode = currentNode.Next;
                    var delayTask = currentNode.Value;

                    if (delayTask.State == DelayTaskState.NotStart)
                    {
                        delayTask.State = DelayTaskState.Started;
                        delayTask.StartSeconds = CurrentSeconds;
                        delayTask.FinishSeconds = CurrentSeconds + delayTask.Seconds;
                    } else if (delayTask.State == DelayTaskState.Started)
                    {
                        if (CurrentSeconds >= delayTask.FinishSeconds)
                        {
                            delayTask.State = DelayTaskState.Finish;
                            delayTask.OnFinish();
                            
                            delayTask.OnFinish = null;

                            mDelayTasks.Remove(currentNode);
                        }
                    }

                    currentNode = nextNode;
                }
            }
        }

        public float CurrentSeconds { get; private set; }

        private LinkedList<DelayTask> mDelayTasks = new LinkedList<DelayTask>();

        public void AddDelayTask(float seconds, Action onDelayFinish)
        {
            var delayTask = new DelayTask()
            {
                Seconds = seconds,
                OnFinish = onDelayFinish,
                State = DelayTaskState.NotStart
            };

            mDelayTasks.AddLast(delayTask);
        }
    }
}