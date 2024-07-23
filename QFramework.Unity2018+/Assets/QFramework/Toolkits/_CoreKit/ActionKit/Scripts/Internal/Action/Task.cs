/****************************************************************************
 * Copyright (c) 2015 - 2024 liangxiegame UNDER MIT License
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Threading.Tasks;

namespace QFramework
{
    public class TaskAction : IAction
    {
        private static SimpleObjectPool<TaskAction> mPool =
            new SimpleObjectPool<TaskAction>(() => new TaskAction(), null, 10);

        private Func<Task> mTaskGetter = null;
        private Task mExecutingTask;

        private TaskAction()
        {
        }

        public static TaskAction Allocate(Func<Task> taskGetter)
        {
            var coroutineAction = mPool.Allocate();
            coroutineAction.ActionID = ActionKit.ID_GENERATOR++;
            coroutineAction.Deinited = false;
            coroutineAction.Reset();
            coroutineAction.mTaskGetter = taskGetter;
            return coroutineAction;
        }

        public bool Paused { get; set; }

        public void Deinit()
        {
            if (!Deinited)
            {
                Deinited = true;
                mTaskGetter = null;
                if (mExecutingTask != null)
                {
                    mExecutingTask.Dispose();
                    mExecutingTask = null;
                }

                ActionQueue.AddCallback(new ActionQueueRecycleCallback<TaskAction>(mPool, this));
            }
        }

        public void Reset()
        {
            Paused = false;
            Status = ActionStatus.NotStart;
        }

        public bool Deinited { get; set; }
        public ulong ActionID { get; set; }
        public ActionStatus Status { get; set; }

        public void OnStart()
        {
            StartTask();
        }

        async void StartTask()
        {
            mExecutingTask = mTaskGetter();
            await mExecutingTask;
            Status = ActionStatus.Finished;
            mExecutingTask = null;
        }

        public void OnExecute(float dt)
        {
        }

        public void OnFinish()
        {
        }
    }

    public static class TaskExtension
    {
        public static ISequence Task(this ISequence self, Func<Task> taskGetter)
        {
            return self.Append(TaskAction.Allocate(taskGetter));
        }

        public static IAction ToAction(this Task self)
        {
            return TaskAction.Allocate(() => self);
        }
    }
}