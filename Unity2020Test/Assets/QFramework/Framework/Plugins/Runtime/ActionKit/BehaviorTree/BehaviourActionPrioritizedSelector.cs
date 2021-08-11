namespace QFramework
{
    public class BehaviourActionPrioritizedSelector : BehaviourAction
    {
        protected class TBTActionPrioritizedSelectorContext : TBTActionContext
        {
            internal int currentSelectedIndex;
            internal int lastSelectedIndex;

            public TBTActionPrioritizedSelectorContext()
            {
                currentSelectedIndex = -1;
                lastSelectedIndex = -1;
            }
        }
        public BehaviourActionPrioritizedSelector()
            : base(-1)
        {
        }
        protected override bool onEvaluate(/*in*/BehaviourTreeData wData)
        {
            TBTActionPrioritizedSelectorContext thisContext = getContext<TBTActionPrioritizedSelectorContext>(wData);
            thisContext.currentSelectedIndex = -1;
            int childCount = GetChildCount();
            for(int i = 0; i < childCount; ++i) {
                BehaviourAction node = GetChild<BehaviourAction>(i);
                if (node.Evaluate(wData)) {
                    thisContext.currentSelectedIndex = i;
                    return true;
                }
            }
            return false;
        }
        protected override int onUpdate(BehaviourTreeData wData)
        {
            TBTActionPrioritizedSelectorContext thisContext = getContext<TBTActionPrioritizedSelectorContext>(wData);
            int runningState = BehaviourTreeRunningStatus.FINISHED;
            if (thisContext.currentSelectedIndex != thisContext.lastSelectedIndex) {
                if (IsIndexValid(thisContext.lastSelectedIndex)) {
                    BehaviourAction node = GetChild<BehaviourAction>(thisContext.lastSelectedIndex);
                    node.Transition(wData);
                }
                thisContext.lastSelectedIndex = thisContext.currentSelectedIndex;
            }
            if (IsIndexValid(thisContext.lastSelectedIndex)) {
                BehaviourAction node = GetChild<BehaviourAction>(thisContext.lastSelectedIndex);
                runningState = node.Update(wData);
                if (BehaviourTreeRunningStatus.IsFinished(runningState)) {
                    thisContext.lastSelectedIndex = -1;
                }
            }
            return runningState;
        }
        protected override void onTransition(BehaviourTreeData wData)
        {
            TBTActionPrioritizedSelectorContext thisContext = getContext<TBTActionPrioritizedSelectorContext>(wData);
            BehaviourAction node = GetChild<BehaviourAction>(thisContext.lastSelectedIndex);
            if (node != null) {
                node.Transition(wData);
            }
            thisContext.lastSelectedIndex = -1;
        }
    }
}
