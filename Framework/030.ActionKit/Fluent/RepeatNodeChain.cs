namespace QFramework
{
    public class RepeatNodeChain : ActionChain
    {
        protected override NodeAction mNode
        {
            get { return mRepeatAction; }
        }

        private RepeatNode mRepeatAction;

        private SequenceNode mSequenceNode;

        public RepeatNodeChain(int repeatCount)
        {
            mSequenceNode = new SequenceNode();
            mRepeatAction = new RepeatNode(mSequenceNode, repeatCount);
        }

        public override IActionChain Append(IAction node)
        {
            mSequenceNode.Append(node);
            return this;
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            if (null != mRepeatAction)
            {
                mRepeatAction.Dispose();
            }

            mRepeatAction = null;

            mSequenceNode.Dispose();
            mSequenceNode = null;
        }
    }
}