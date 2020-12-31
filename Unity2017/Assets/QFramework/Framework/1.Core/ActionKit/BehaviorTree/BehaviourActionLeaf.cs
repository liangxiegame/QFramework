using System;
using System.Collections.Generic;

namespace QFramework
{
    public abstract class BehaviourActionLeaf : BehaviourAction
    {
        private const int ACTION_READY = 0;
        private const int ACTION_RUNNING = 1;
        private const int ACTION_FINISHED = 2;

        class TBTActionLeafContext : TBTActionContext
        {
            internal int  status;
            internal bool needExit;

            private object _userData;
            public T getUserData<T>() where T : class, new()
            {
                if (_userData == null) {
                    _userData = new T();
                }
                return (T)_userData;
            }
            public TBTActionLeafContext()
            {
                status = ACTION_READY;
                needExit = false;

                _userData = null;
            }
        }
        public BehaviourActionLeaf()
            : base(0)
        {
        }
        protected sealed override int onUpdate(BehaviourTreeData wData)
        {
            int runningState = BehaviourTreeRunningStatus.FINISHED;
            TBTActionLeafContext thisContext = getContext<TBTActionLeafContext>(wData);
            if (thisContext.status == ACTION_READY) {
                onEnter(wData);
                thisContext.needExit = true;
                thisContext.status = ACTION_RUNNING;
            }
            if (thisContext.status == ACTION_RUNNING) {
                runningState = onExecute(wData);
                if (BehaviourTreeRunningStatus.IsFinished(runningState)) {
                    thisContext.status = ACTION_FINISHED;
                }
            }
            if (thisContext.status == ACTION_FINISHED) {
                if (thisContext.needExit) {
                    onExit(wData, runningState);
                }
                thisContext.status = ACTION_READY;
                thisContext.needExit = false;
            }
            return runningState;
        }
        protected sealed override void onTransition(BehaviourTreeData wData)
        {
            TBTActionLeafContext thisContext = getContext<TBTActionLeafContext>(wData);
            if (thisContext.needExit) {
                onExit(wData, BehaviourTreeRunningStatus.TRANSITION);
            }
            thisContext.status = ACTION_READY;
            thisContext.needExit = false;
        }
        protected T getUserContexData<T>(BehaviourTreeData wData) where T : class, new()
        {
            return getContext<TBTActionLeafContext>(wData).getUserData<T>();
        }
        //--------------------------------------------------------
        // inherented by children-
        protected virtual void onEnter(/*in*/BehaviourTreeData wData)
        {
        }
        protected virtual int onExecute(BehaviourTreeData wData)
        {
            return BehaviourTreeRunningStatus.FINISHED;
        }
        protected virtual void onExit(BehaviourTreeData wData, int runningStatus)
        {

        }
    }
}
