using System.Collections.Generic;
using System.Linq;

namespace QFramework
{
    public class AsyncNode : NodeAction
    {
        public HashSet<IAction> mActions = new HashSet<IAction>();
        
        public void Add(IAction action)
        {
            mActions.Add(action);
        }

        protected override void OnExecute(float dt)
        {
            foreach (var action in mActions.Where(action => action.Execute(dt)))
            {
                mActions.Remove(action);
                action.Dispose();
            }
        }
    }
}