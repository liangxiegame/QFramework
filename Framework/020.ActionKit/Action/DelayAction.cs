using System;

namespace QFramework
{
    /// <inheritdoc />
    /// <summary>
    /// 延时执行节点
    /// </summary>
    public class DelayAction : NodeAction, IPoolable,IResetable
    {
        public float DelayTime;
        private Action onDelayFinish;

        public static DelayAction Allocate(float delayTime, System.Action onDelayFinish = null)
        {
            var retNode = SafeObjectPool<DelayAction>.Instance.Allocate();
            retNode.DelayTime = delayTime;
            retNode.onDelayFinish = onDelayFinish;
            return retNode;
        }

        public DelayAction()
        {
        }

        public DelayAction(float delayTime)
        {
            DelayTime = delayTime;
        }

        private float mCurrentSeconds = 0.0f;

        protected override void OnReset()
        {
            mCurrentSeconds = 0.0f;
        }

        protected override void OnExecute(float dt)
        {
            mCurrentSeconds += dt;
            Finished = mCurrentSeconds >= DelayTime;
            if (Finished && onDelayFinish != null)
            {
                onDelayFinish();
            }
        }

        protected override void OnDispose()
        {
            SafeObjectPool<DelayAction>.Instance.Recycle(this);
        }

        public void OnRecycled()
        {
            onDelayFinish = null;
            DelayTime = 0.0f;
            Reset();
        }

        public bool IsRecycled { get; set; }
    }
}