using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    [MonoSingletonPath("[ActionKit]/ActionQueue")]
    public class ActionQueue : MonoBehaviour, ISingleton
    {
        private List<IAction> mActions = new List<IAction>();

        public static void Append(IAction action)
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