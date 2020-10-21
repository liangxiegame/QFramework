using System;
using System.Collections.Generic;

namespace QFramework
{
    public class BehaviourActionSequence : BehaviourAction
    {
        //-------------------------------------------------------
        protected class TBTActionSequenceContext : TBTActionContext
        {
            internal int currentSelectedIndex;
            public TBTActionSequenceContext()
            {
                currentSelectedIndex = -1;
            }
        }
        //-------------------------------------------------------
        private bool _continueIfErrorOccors;
        //-------------------------------------------------------
        public BehaviourActionSequence()
            : base(-1)
        {
            _continueIfErrorOccors = false;
        }
        public BehaviourActionSequence SetContinueIfErrorOccors(bool v)
        {
            _continueIfErrorOccors = v;
            return this;
        }
        //------------------------------------------------------
        protected override bool onEvaluate(/*in*/BehaviourTreeData wData)
        {
            TBTActionSequenceContext thisContext = getContext<TBTActionSequenceContext>(wData);
            int checkedNodeIndex = -1;
            if (IsIndexValid(thisContext.currentSelectedIndex)) {
                checkedNodeIndex = thisContext.currentSelectedIndex;
            } else {
                checkedNodeIndex = 0;
            }
            if (IsIndexValid(checkedNodeIndex)) {
                BehaviourAction node = GetChild<BehaviourAction>(checkedNodeIndex);
                if (node.Evaluate(wData)) {
                    thisContext.currentSelectedIndex = checkedNodeIndex;
                    return true;
                }
            }
            return false;
        }
        protected override int onUpdate(BehaviourTreeData wData)
        {
            TBTActionSequenceContext thisContext = getContext<TBTActionSequenceContext>(wData);
            int runningStatus = BehaviourTreeRunningStatus.FINISHED;
            BehaviourAction node = GetChild<BehaviourAction>(thisContext.currentSelectedIndex);
            runningStatus = node.Update(wData);
            if (_continueIfErrorOccors == false && BehaviourTreeRunningStatus.IsError(runningStatus)) {
                thisContext.currentSelectedIndex = -1;
                return runningStatus;
            }
            if (BehaviourTreeRunningStatus.IsFinished(runningStatus)) {
                thisContext.currentSelectedIndex++;
                if (IsIndexValid(thisContext.currentSelectedIndex)) {
                    runningStatus = BehaviourTreeRunningStatus.EXECUTING;
                } else {
                    thisContext.currentSelectedIndex = -1;
                }
            }
            return runningStatus;
        }
        protected override void onTransition(BehaviourTreeData wData)
        {
            TBTActionSequenceContext thisContext = getContext<TBTActionSequenceContext>(wData);
            BehaviourAction node = GetChild<BehaviourAction>(thisContext.currentSelectedIndex);
            if (node != null) {
                node.Transition(wData);
            }
            thisContext.currentSelectedIndex = -1;
        }
    }
}
