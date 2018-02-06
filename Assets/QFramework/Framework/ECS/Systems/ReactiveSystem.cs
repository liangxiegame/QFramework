
using System.Collections.Generic;
using System.Linq;

namespace QFramework
{
    /// A ReactiveSystem calls Execute(entities) if there were changes based on
    /// the specified Collector and will only pass in changed entities.
    /// A common use-case is to react to changes, e.g. a change of the position
    /// of an entity to update the gameObject.transform.position
    /// of the related gameObject.
    public abstract class ReactiveSystem<TEntity> : IReactiveSystem where TEntity : class, IEntity
    {

        readonly ICollector<TEntity> mCollector;
        readonly List<TEntity> mBuffer;
        string mToStringCache;

        protected ReactiveSystem(IContext<TEntity> context)
        {
            mCollector = GetTrigger(context);
            mBuffer = new List<TEntity>();
        }

        protected ReactiveSystem(ICollector<TEntity> collector)
        {
            mCollector = collector;
            mBuffer = new List<TEntity>();
        }

        /// Specify the collector that will trigger the ReactiveSystem.
        protected abstract ICollector<TEntity> GetTrigger(IContext<TEntity> context);

        /// This will exclude all entities which don't pass the filter.
        protected abstract bool Filter(TEntity entity);

        protected abstract void Execute(List<TEntity> entities);

        /// Activates the ReactiveSystem and starts observing changes
        /// based on the specified Collector.
        /// ReactiveSystem are activated by default.
        public void Activate()
        {
            mCollector.Activate();
        }

        /// Deactivates the ReactiveSystem.
        /// No changes will be tracked while deactivated.
        /// This will also clear the ReactiveSystem.
        /// ReactiveSystem are activated by default.
        public void Deactivate()
        {
            mCollector.Deactivate();
        }

        /// Clears all accumulated changes.
        public void Clear()
        {
            mCollector.ClearCollectedEntities();
        }

        /// Will call Execute(entities) with changed entities
        /// if there are any. Otherwise it will not call Execute(entities).
        public void Execute()
        {
            if (mCollector.count != 0)
            {
                foreach (var e in mCollector.collectedEntities.Where(e => Filter(e)))
                {
                    e.Retain(this);
                    mBuffer.Add(e);
                }

                mCollector.ClearCollectedEntities();

                if (mBuffer.Count != 0)
                {
                    Execute(mBuffer);
                    mBuffer.ForEach(e => e.Release(this));
                    mBuffer.Clear();
                }
            }
        }

        public override string ToString()
        {
            if (mToStringCache == null)
            {
                mToStringCache = "ReactiveSystem(" + GetType().Name + ")";
            }

            return mToStringCache;
        }

        ~ReactiveSystem()
        {
            Deactivate();
        }
    }
}
