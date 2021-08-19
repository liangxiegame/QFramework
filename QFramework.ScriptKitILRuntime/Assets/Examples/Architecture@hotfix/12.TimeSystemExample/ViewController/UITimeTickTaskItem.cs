using UnityEngine;
using QFramework;
using UniRx;

namespace QFramework.Example
{
    public partial class UITimeTickTaskItem : ILComponent
    {
        public void InitWithData(TimeTickTask timeTickTask)
        {

            Observable.EveryUpdate()
                .Subscribe(_ =>
                {

                    if (timeTickTask.State == TimeTickTask.StateTicking)
                    {
                        Text.text = "类型:" + timeTickTask.Type + " 剩余时间:" + timeTickTask.RemainSeconds;
                    }
                    else
                    {
                        Text.text = "类型:" + timeTickTask.Type + " 已完成";
                    }
                    
                }).AddTo(gameObject);
        }


        protected override void OnStart()
        {
            
        }

        protected override void OnDestroy()
        {
        }
    }
}