using System;

namespace QFramework
{
    public class BehaviourActionLoop : BehaviourAction
    {
        public const int INFINITY = -1;
        //--------------------------------------------------------
        protected class TBTActionLoopContext : TBTActionContext
        {
            internal int currentCount;

            public TBTActionLoopContext()
            {
                currentCount = 0;
            }
        }
        //--------------------------------------------------------
        private int _loopCount;
        //--------------------------------------------------------
        public BehaviourActionLoop()
            : base(1)
        {
            _loopCount = INFINITY;
        }
        public BehaviourActionLoop SetLoopCount(int count)
        {
            _loopCount = count;
            return this;
        }
        //-------------------------------------------------------
        protected override bool onEvaluate(/*in*/BehaviourTreeData wData)
        {
            TBTActionLoopContext thisContext = getContext<TBTActionLoopContext>(wData);
            bool checkLoopCount = (_loopCount == INFINITY || thisContext.currentCount < _loopCount);
            if (checkLoopCount == false) {
                return false;
            }
            if (IsIndexValid(0)) {
                BehaviourAction node = GetChild<BehaviourAction>(0);
                return node.Evaluate(wData);
            }
            return false;
        }
        protected override int onUpdate(BehaviourTreeData wData)
        {
            TBTActionLoopContext thisContext = getContext<TBTActionLoopContext>(wData);
            int runningStatus = BehaviourTreeRunningStatus.FINISHED;
            if (IsIndexValid(0)) {
                BehaviourAction node = GetChild<BehaviourAction>(0);
                runningStatus = node.Update(wData);
                if (BehaviourTreeRunningStatus.IsFinished(runningStatus)) {
                    thisContext.currentCount++;
                    if (thisContext.currentCount < _loopCount || _loopCount == INFINITY) {
                        runningStatus = BehaviourTreeRunningStatus.EXECUTING;
                    }
                }
            }
            return runningStatus;
        }
        protected override void onTransition(BehaviourTreeData wData)
        {
            TBTActionLoopContext thisContext = getContext<TBTActionLoopContext>(wData);
            if (IsIndexValid(0)) {
                BehaviourAction node = GetChild<BehaviourAction>(0);
                node.Transition(wData);
            }
            thisContext.currentCount = 0;
        }
    }
}
