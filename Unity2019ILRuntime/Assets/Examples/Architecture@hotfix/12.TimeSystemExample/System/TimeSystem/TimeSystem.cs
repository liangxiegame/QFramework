using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace QFramework.Example
{
    public interface ITimeSystem : ILSystem
    {
        void AddTask(int taskType, int seconds);


        List<TimeTickTask> GetAllTask();
    }

    public class OnTimeSystemChangeEvent
    {
    }


    public class TimeSystem : ILSystem<TimeSystemExample>, ITimeSystem
    {
        public TimeSystem()
        {
            // 注册 Update
            Observable.EveryUpdate()
                .Subscribe(_ => { Update(); });
        }

        /// <summary>
        /// key: Type
        /// </summary>
        Dictionary<int, ReactiveCollection<TimeTickTask>> mTasks =
            new Dictionary<int, ReactiveCollection<TimeTickTask>>();


        void Update()
        {
            foreach (var taskKeyValue in mTasks)
            {
                foreach (var task in taskKeyValue.Value)
                {
                    if (task.State == TimeTickTask.StateWaiting)
                    {
                        task.State = TimeTickTask.StateTicking;
                        task.OnStart();
                    }
                    else if (task.State == TimeTickTask.StateTicking)
                    {
                        if (task.RemainSeconds == 0)
                        {
                            task.State = TimeTickTask.StateFinished;
                            task.OnFinish();
                            this.SendEvent<OnTimeSystemChangeEvent>();
                        }
                    }
                }
            }
        }

        ReactiveCollection<TimeTickTask> GetOrCreateListByType(int type)
        {
            if (mTasks.TryGetValue(type, out var retList))
            {
            }
            else
            {
                retList = new ReactiveCollection<TimeTickTask>();
                mTasks.Add(type, retList);
            }

            return retList;
        }

        public Subject<Unit> OnChange { get; }

        public void AddTask(int taskType, int seconds)
        {
            var list = GetOrCreateListByType(taskType);

            var task = TaskFactory.Create(taskType);
            task.StartTime = DateTime.Now;
            task.EndTime = DateTime.Now.AddSeconds(seconds);
            task.Type = taskType;

            list.Add(task);

            SendEvent<OnTimeSystemChangeEvent>();
        }

        public List<TimeTickTask> GetAllTask()
        {
            return mTasks.SelectMany(t => t.Value).ToList();
        }
    }
}