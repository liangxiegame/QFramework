using System;
using System.Collections.Generic;

namespace QFramework
{
    public class BehaviourActionNonPrioritizedSelector : BehaviourActionPrioritizedSelector
    {
        public BehaviourActionNonPrioritizedSelector()
            : base()
        {
        }
        protected override bool onEvaluate(/*in*/BehaviourTreeData wData)
        {
            BehaviourActionPrioritizedSelector.TBTActionPrioritizedSelectorContext thisContext = 
                getContext<BehaviourActionPrioritizedSelector.TBTActionPrioritizedSelectorContext>(wData);
            //check last node first
            if (IsIndexValid(thisContext.currentSelectedIndex)) {
                BehaviourAction node = GetChild<BehaviourAction>(thisContext.currentSelectedIndex);
                if (node.Evaluate(wData)) {
                    return true;
                }
            }
            return base.onEvaluate(wData);
        }
    }
}
