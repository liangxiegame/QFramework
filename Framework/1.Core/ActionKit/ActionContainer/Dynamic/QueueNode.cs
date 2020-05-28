using System.Collections.Generic;

namespace QFramework
{
    public class QueueNode : NodeAction
    {
        private Queue<IAction> mQueue = new Queue<IAction>(20);

        public void Enqueue(IAction action)
        {
            mQueue.Enqueue(action);
        }

        protected override void OnExecute(float dt)
        {
            if (mQueue.Count != 0 && mQueue.Peek().Execute(dt))
            {
                mQueue.Dequeue().Dispose();
            }
        }
    }
}