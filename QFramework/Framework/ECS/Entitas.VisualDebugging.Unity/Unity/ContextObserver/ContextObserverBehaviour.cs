using UnityEngine;

namespace Entitas.VisualDebugging.Unity
{
    [ExecuteInEditMode]
    public class ContextObserverBehaviour : MonoBehaviour
    {
        public ContextObserver ContextObserver
        {
            get { return mContextObserver; }
        }

        ContextObserver mContextObserver;

        public void Init(ContextObserver contextObserver)
        {
            mContextObserver = contextObserver;
            Update();
        }

        void Update()
        {
            if (mContextObserver == null)
            {
                gameObject.DestroyGameObject();
            }
            else if (mContextObserver.GameObject != null)
            {
                mContextObserver.GameObject.name = mContextObserver.ToString();
            }
        }

        void OnDestroy()
        {
            mContextObserver.Deactivate();
        }
    }
}
