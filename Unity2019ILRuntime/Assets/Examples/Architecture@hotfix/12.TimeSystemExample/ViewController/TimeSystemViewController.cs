using System.Collections.Generic;
using UnityEngine;
using QFramework;
using UniRx;

namespace QFramework.Example
{
    public partial class TimeSystemViewController : ILController<TimeSystemExample>
    {
        private ILEventSystemNode<TimeSystemExample> mEventSystemNode = ILEventSystemNode<TimeSystemExample>.Allocate();
        
        private List<TimeTickTask> mAllTask = null;
        void OnStart()
        {
            var timeSystem = GetSystem<ITimeSystem>();

            
            mAllTask = timeSystem.GetAllTask();
            UpdateTaskView();
            
            // 监听时间系统更新事件
            mEventSystemNode.Register<OnTimeSystemChangeEvent>(_ =>
            {
                mAllTask = timeSystem.GetAllTask();
                UpdateTaskView();
            });
            


            BtnCreateA.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    timeSystem.AddTask(TimeTickTask.TypeA, 60);
                });
            
            BtnCreateB.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    timeSystem.AddTask(TimeTickTask.TypeB, 10);
                });
            
            BtnCreateC.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    timeSystem.AddTask(TimeTickTask.TypeC, 30);
                });
        }

        void UpdateTaskView()
        {
            ItemRoot.DestroyChildren();
            
            foreach (var timeTickTask in mAllTask)
            {
                UITimeTickTaskItem.InstantiateWithParent(ItemRoot)
                    .Show()
                    .GetILComponent<UITimeTickTaskItem>()
                    .InitWithData(timeTickTask);
            }

            UITimeTickTaskItem.Hide();
        }

        void OnDestroy()
        {
            mEventSystemNode.Recycle2Cache();
            mEventSystemNode = null;
        }
    }
}