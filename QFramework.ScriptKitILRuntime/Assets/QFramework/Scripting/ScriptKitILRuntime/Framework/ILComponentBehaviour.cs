using System;
using UnityEngine;

namespace QFramework
{
    public class ILComponentBehaviour : ViewController
    {
        public object Script;
        
        public Action OnDestroyAction = () => { };

        private void OnDestroy()
        {
            if (Application.isPlaying)
            {
                OnDestroyAction.Invoke();
            }
            
            OnDestroyAction = null;
        }
    }
}