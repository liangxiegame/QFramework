using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    internal interface IActionQueueCallback
    {
        void Call();
    }

    internal struct ActionQueueRecycleCallback<T> : IActionQueueCallback
    {
        public SimpleObjectPool<T> Pool;
        public T Action;

        public ActionQueueRecycleCallback(SimpleObjectPool<T> pool, T action)
        {
            Pool = pool;
            Action = action;
        }

        public void Call()
        {
            Pool.Recycle(Action);
            Pool = null;
            Action = default;
        }
    }
    
    [MonoSingletonPath("QFramework/ActionKit/Queue")]
    internal class ActionQueue : MonoBehaviour, ISingleton
    {
        private List<IDeprecateAction> mActions = new List<IDeprecateAction>();


        private List<IActionQueueCallback> mActionQueueCallbacks = new List<IActionQueueCallback>();

        public static void AddCallback(IActionQueueCallback actionQueueCallback)
        {
            mInstance.mActionQueueCallbacks.Add(actionQueueCallback);
        }

        public static void Append(IDeprecateAction action)
        {
            mInstance.mActions.Add(action);
        }

        // Update is called once per frame
        private void Update()
        {
            if (mActions.Count != 0 && mActions[0].Execute(Time.deltaTime))
            {
                mActions.RemoveAt(0);
            }

            if (mActionQueueCallbacks.Count > 0)
            {
                foreach (var actionQueueCallback in mActionQueueCallbacks)
                {
                    actionQueueCallback.Call();
                }
                
                mActionQueueCallbacks.Clear();
            }
        }

        void ISingleton.OnSingletonInit()
        {
        }

        private static ActionQueue mInstance
        {
            get { return MonoSingletonProperty<ActionQueue>.Instance; }
        }
    }
}