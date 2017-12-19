using UnityEngine;


namespace QFramework.VisualDebugging.Unity
{

    [ExecuteInEditMode]
    public class EntityBehaviour : MonoBehaviour
    {

        public IContext Context { get; private set; }

        public IEntity Entity { get; private set; }

        string mCachedName;

        public void Init(IContext context, IEntity entity)
        {
            Context = context;
            Entity = entity;
            Entity.OnEntityReleased += onEntityReleased;
            Update();
        }

        void onEntityReleased(IEntity e)
        {
            gameObject.DestroyGameObject();
        }

        void Update()
        {
            if (Entity != null && mCachedName != Entity.ToString())
            {
                name = mCachedName = Entity.ToString();
            }
        }

        void OnDestroy()
        {
            if (Entity != null)
            {
                Entity.OnEntityReleased -= onEntityReleased;
            }
        }
    }
}