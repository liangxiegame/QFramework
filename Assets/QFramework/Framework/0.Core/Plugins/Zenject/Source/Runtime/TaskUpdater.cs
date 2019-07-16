using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ModestTree;

namespace Zenject
{
    // Update tasks once per frame based on a priority
    [DebuggerStepThrough]
    public abstract class TaskUpdater<TTask>
    {
        readonly LinkedList<TaskInfo> _tasks = new LinkedList<TaskInfo>();
        readonly List<TaskInfo> _queuedTasks = new List<TaskInfo>();

        IEnumerable<TaskInfo> AllTasks
        {
            get { return ActiveTasks.Concat(_queuedTasks); }
        }

        IEnumerable<TaskInfo> ActiveTasks
        {
            get { return _tasks; }
        }

        public void AddTask(TTask task, int priority)
        {
            AddTaskInternal(task, priority);
        }

        void AddTaskInternal(TTask task, int priority)
        {
            Assert.That(!AllTasks.Select(x => x.Task).ContainsItem(task),
                "Duplicate task added to DependencyRoot with name '" + task.GetType().FullName + "'");

            // Wait until next frame to add the task, otherwise whether it gets updated
            // on the current frame depends on where in the update order it was added
            // from, so you might get off by one frame issues
            _queuedTasks.Add(new TaskInfo(task, priority));
        }

        public void RemoveTask(TTask task)
        {
            var info = AllTasks.Where(x => ReferenceEquals(x.Task, task)).SingleOrDefault();

            Assert.IsNotNull(info, "Tried to remove a task not added to DependencyRoot, task = " + task.GetType().Name);

            Assert.That(!info.IsRemoved, "Tried to remove task twice, task = " + task.GetType().Name);
            info.IsRemoved = true;
        }

        public void OnFrameStart()
        {
            // See above comment
            AddQueuedTasks();
        }

        public void UpdateAll()
        {
            UpdateRange(int.MinValue, int.MaxValue);
        }

        public void UpdateRange(int minPriority, int maxPriority)
        {
            var node = _tasks.First;

            while (node != null)
            {
                var next = node.Next;
                var taskInfo = node.Value;

                // Make sure that tasks with priority of int.MaxValue are updated when maxPriority is int.MaxValue
                if (!taskInfo.IsRemoved && taskInfo.Priority >= minPriority
                    && (maxPriority == int.MaxValue || taskInfo.Priority < maxPriority))
                {
                    UpdateItem(taskInfo.Task);
                }

                node = next;
            }

            ClearRemovedTasks(_tasks);
        }

        void ClearRemovedTasks(LinkedList<TaskInfo> tasks)
        {
            var node = tasks.First;

            while (node != null)
            {
                var next = node.Next;
                var info = node.Value;

                if (info.IsRemoved)
                {
                    //ModestTree.Log.Debug("Removed task '" + info.Task.GetType().ToString() + "'");
                    tasks.Remove(node);
                }

                node = next;
            }
        }

        void AddQueuedTasks()
        {
            for (int i = 0; i < _queuedTasks.Count; i++)
            {
                var task = _queuedTasks[i];

                if (!task.IsRemoved)
                {
                    InsertTaskSorted(task);
                }
            }
            _queuedTasks.Clear();
        }

        void InsertTaskSorted(TaskInfo task)
        {
            for (var current = _tasks.First; current != null; current = current.Next)
            {
                if (current.Value.Priority > task.Priority)
                {
                    _tasks.AddBefore(current, task);
                    return;
                }
            }

            _tasks.AddLast(task);
        }

        protected abstract void UpdateItem(TTask task);

        class TaskInfo
        {
            public TTask Task;
            public int Priority;
            public bool IsRemoved;

            public TaskInfo(TTask task, int priority)
            {
                Task = task;
                Priority = priority;
            }
        }
    }

    public class TickablesTaskUpdater : TaskUpdater<ITickable>
    {
        protected override void UpdateItem(ITickable task)
        {
#if ZEN_INTERNAL_PROFILING
            using (ProfileTimers.CreateTimedBlock("User Code"))
#endif
#if UNITY_EDITOR
            using (ProfileBlock.Start("{0}.Tick()", task.GetType()))
#endif
            {
                task.Tick();
            }
        }
    }

    public class LateTickablesTaskUpdater : TaskUpdater<ILateTickable>
    {
        protected override void UpdateItem(ILateTickable task)
        {
#if ZEN_INTERNAL_PROFILING
            using (ProfileTimers.CreateTimedBlock("User Code"))
#endif
#if UNITY_EDITOR
            using (ProfileBlock.Start("{0}.LateTick()", task.GetType()))
#endif
            {
                task.LateTick();
            }
        }
    }

    public class FixedTickablesTaskUpdater : TaskUpdater<IFixedTickable>
    {
        protected override void UpdateItem(IFixedTickable task)
        {
#if ZEN_INTERNAL_PROFILING
            using (ProfileTimers.CreateTimedBlock("User Code"))
#endif
#if UNITY_EDITOR
            using (ProfileBlock.Start("{0}.FixedTick()", task.GetType()))
#endif
            {
                task.FixedTick();
            }
        }
    }
}
