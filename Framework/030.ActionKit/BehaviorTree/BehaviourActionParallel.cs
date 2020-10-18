using System;
using System.Collections.Generic;

namespace QFramework
{
    public class BehaviourActionParallel : BehaviourAction
    {
        public enum ECHILDREN_RELATIONSHIP
        {
            AND, OR
        }
        //-------------------------------------------------------
        protected class TBTActionParallelContext : TBTActionContext
        {
            internal List<bool> evaluationStatus;
            internal List<int> runningStatus;

            public TBTActionParallelContext()
            {
                evaluationStatus = new List<bool>();
                runningStatus = new List<int>();
            }
        }
        //-------------------------------------------------------
        private ECHILDREN_RELATIONSHIP _evaluationRelationship;
        private ECHILDREN_RELATIONSHIP _runningStatusRelationship;
        //-------------------------------------------------------
        public BehaviourActionParallel()
            : base(-1)
        {
            _evaluationRelationship = ECHILDREN_RELATIONSHIP.AND;
            _runningStatusRelationship = ECHILDREN_RELATIONSHIP.OR;
        }
        public BehaviourActionParallel SetEvaluationRelationship(ECHILDREN_RELATIONSHIP v)
        {
            _evaluationRelationship = v;
            return this;
        }
        public BehaviourActionParallel SetRunningStatusRelationship(ECHILDREN_RELATIONSHIP v)
        {
            _runningStatusRelationship = v;
            return this;
        }
        //------------------------------------------------------
        protected override bool onEvaluate(/*in*/BehaviourTreeData wData)
        {
            TBTActionParallelContext thisContext = getContext<TBTActionParallelContext>(wData);
            initListTo<bool>(thisContext.evaluationStatus, false);
            bool finalResult = false;
            for (int i = 0; i < GetChildCount(); ++i) {
                BehaviourAction node = GetChild<BehaviourAction>(i);
                bool ret = node.Evaluate(wData);
                //early break
                if (_evaluationRelationship == ECHILDREN_RELATIONSHIP.AND && ret == false) {
                    finalResult = false;
                    break;
                }
                if (ret == true){
                    finalResult = true;
                }
                thisContext.evaluationStatus[i] = ret;
            }
            return finalResult;
        }
        protected override int onUpdate(BehaviourTreeData wData)
        {
            TBTActionParallelContext thisContext = getContext<TBTActionParallelContext>(wData);
            //first time initialization
            if (thisContext.runningStatus.Count != GetChildCount()) {
                initListTo<int>(thisContext.runningStatus, BehaviourTreeRunningStatus.EXECUTING);
            }
            bool hasFinished  = false;
            bool hasExecuting = false;
            for (int i = 0; i < GetChildCount(); ++i) {
                if (thisContext.evaluationStatus[i] == false) {
                    continue;
                }
                if (BehaviourTreeRunningStatus.IsFinished(thisContext.runningStatus[i])) {
                    hasFinished = true;
                    continue;
                }
                BehaviourAction node = GetChild<BehaviourAction>(i);
                int runningStatus = node.Update(wData);
                if (BehaviourTreeRunningStatus.IsFinished(runningStatus)) {
                    hasFinished  = true;
                } else {
                    hasExecuting = true;
                }
                thisContext.runningStatus[i] = runningStatus;
            }
            if (_runningStatusRelationship == ECHILDREN_RELATIONSHIP.OR && hasFinished || _runningStatusRelationship == ECHILDREN_RELATIONSHIP.AND && hasExecuting == false) {
                initListTo<int>(thisContext.runningStatus, BehaviourTreeRunningStatus.EXECUTING);
                return BehaviourTreeRunningStatus.FINISHED;
            }
            return BehaviourTreeRunningStatus.EXECUTING;
        }
        protected override void onTransition(BehaviourTreeData wData)
        {
            TBTActionParallelContext thisContext = getContext<TBTActionParallelContext>(wData);
            for (int i = 0; i < GetChildCount(); ++i) {
                BehaviourAction node = GetChild<BehaviourAction>(i);
                node.Transition(wData);
            }
            //clear running status
            initListTo<int>(thisContext.runningStatus, BehaviourTreeRunningStatus.EXECUTING);
        }
        private void initListTo<T>(List<T> list, T value)
        {
            int childCount = GetChildCount();
            if (list.Count != childCount) {
                list.Clear();
                for (int i = 0; i < childCount; ++i) {
                    list.Add(value);
                }
            } else {
                for (int i = 0; i < childCount; ++i) {
                    list[i] = value;
                }
            }
        }
    }
}
