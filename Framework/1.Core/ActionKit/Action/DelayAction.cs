namespace QFramework
{
    /// <inheritdoc />
    /// <summary>
    /// 延时执行节点
    /// </summary>
    public class DelayAction : NodeAction, IPoolable,IResetable
    {
        public float DelayTime;

        public static DelayAction Allocate(float delayTime, System.Action onEndCallback = null)
        {
            var retNode = SafeObjectPool<DelayAction>.Instance.Allocate();
            retNode.DelayTime = delayTime;
            retNode.OnEndedCallback = onEndCallback;
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
        }

        protected override void OnDispose()
        {
            SafeObjectPool<DelayAction>.Instance.Recycle(this);
        }

        public void OnRecycled()
        {
            DelayTime = 0.0f;
            Reset();
        }

        public bool IsRecycled { get; set; }
    }
}