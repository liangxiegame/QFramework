using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace QFramework.VisualDebugging.Unity
{
    public class ContextObserver
    {
        public IContext Context { get; protected set; }

        public IGroup[] Groups
        {
            get { return mGroups.ToArray(); }
        }

        public GameObject GameObject
        {
            get { return mGameObject; }
        }

        readonly List<IGroup> mGroups;
        readonly GameObject mGameObject;

        StringBuilder mToStringBuilder = new StringBuilder();

        public ContextObserver(IContext context)
        {
            Context = context;
            mGroups = new List<IGroup>();
            mGameObject = new GameObject();
            mGameObject.AddComponent<ContextObserverBehaviour>().Init(this);

            Context.OnEntityCreated += onEntityCreated;
            Context.OnGroupCreated += onGroupCreated;
        }

        public void Deactivate()
        {
            Context.OnEntityCreated -= onEntityCreated;
            Context.OnGroupCreated -= onGroupCreated;
        }

        void onEntityCreated(IContext context, IEntity entity)
        {
            var entityBehaviour = new GameObject().AddComponent<EntityBehaviour>();
            entityBehaviour.Init(context, entity);
            entityBehaviour.transform.SetParent(mGameObject.transform, false);
        }

        void onGroupCreated(IContext context, IGroup group)
        {
            mGroups.Add(group);
        }

        public override string ToString()
        {
            mToStringBuilder.Length = 0;
            mToStringBuilder
                .Append(Context.ContextInfo.Name).Append(" (")
                .Append(Context.Count).Append(" entities, ")
                .Append(Context.ReusableEntitiesCount).Append(" reusable, ");

            if (Context.RetainedEntitiesCount != 0)
            {
                mToStringBuilder
                    .Append(Context.RetainedEntitiesCount).Append(" retained, ");
            }

            mToStringBuilder
                .Append(mGroups.Count)
                .Append(" groups)");

            var str = mToStringBuilder.ToString();
            mGameObject.name = str;
            return str;
        }
    }
}