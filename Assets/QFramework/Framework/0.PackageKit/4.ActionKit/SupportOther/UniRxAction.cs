using System;
using UniRx;

namespace QF.Action
{
    public class UniRxAction<T> : NodeAction, IPoolable, IPoolType
    {
        private Func<IObservable<T>> mTweenFactory;

        public static UniRxAction<T> Allocate(Func<IObservable<T>> tweenFactory)
        {
            var action = SafeObjectPool<UniRxAction<T>>.Instance.Allocate();

            action.mTweenFactory = tweenFactory;

            return action;
        }

        protected override void OnBegin()
        {
            var tween = mTweenFactory.Invoke();

            tween.Subscribe(_=>Finish());
        }

        public void OnRecycled()
        {
            mTweenFactory = null;
        }

        public bool IsRecycled { get; set; }

        public void Recycle2Cache()
        {
            SafeObjectPool<UniRxAction<T>>.Instance.Recycle(this);
        }
    }

    public static partial class IActionChainExtention
    {
        public static IActionChain UniRx<T>(this IActionChain selfChain, Func<IObservable<T>> tweenFactory)
        {
            return selfChain.Append(UniRxAction<T>.Allocate(tweenFactory));
        }
    }
}