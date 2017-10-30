using QFramework;
using System.Collections.Generic;
using System.Linq;

namespace Entitas
{
    /// A ReactiveSystem calls Execute(entities) if there were changes based on
    /// the specified Collector and will only pass in changed entities.
    /// A common use-case is to react to changes, e.g. a change of the position
    /// of an entity to update the gameObject.transform.position
    /// of the related gameObject.
    public abstract class MultiReactiveSystem<TEntity, TContexts> : IReactiveSystem
        where TEntity : class, IEntity
        where TContexts : class, IContexts
    {

        readonly ICollector[] mCollectors;
        readonly List<TEntity> mBuffer;
        string mToStringCache;

        protected MultiReactiveSystem(TContexts contexts)
        {
            mCollectors = GetTrigger(contexts);
            mBuffer = new List<TEntity>();
        }

        protected MultiReactiveSystem(ICollector[] collectors)
        {
            mCollectors = collectors;
            mBuffer = new List<TEntity>();
        }

        /// Specify the collector that will trigger the ReactiveSystem.
        protected abstract ICollector[] GetTrigger(TContexts contexts);

        /// This will exclude all entities which don't pass the filter.
        protected abstract bool Filter(TEntity entity);

        protected abstract void Execute(List<TEntity> entities);

        /// Activates the ReactiveSystem and starts observing changes
        /// based on the specified Collector.
        /// ReactiveSystem are activated by default.
        public void Activate()
        {
            mCollectors.ForEach(collector => collector.Activate());
        }

        /// Deactivates the ReactiveSystem.
        /// No changes will be tracked while deactivated.
        /// This will also clear the ReactiveSystem.
        /// ReactiveSystem are activated by default.
        public void Deactivate()
        {
            mCollectors.ForEach(collector => collector.Deactivate());
        }

        /// Clears all accumulated changes.
        public void Clear()
        {
            mCollectors.ForEach(collector => collector.ClearCollectedEntities());
        }

        /// Will call Execute(entities) with changed entities
        /// if there are any. Otherwise it will not call Execute(entities).
        public void Execute()
        {
            for (int i = 0; i < mCollectors.Length; i++)
            {
                var collector = mCollectors[i];
                if (collector.count != 0)
                {
                    var CollectedEntities = collector.GetCollectedEntities<TEntity>();
                    foreach (var e in CollectedEntities.Where(e => Filter(e)))
                    {
                        e.Retain(this);
                        mBuffer.Add(e);
                    }

                    collector.ClearCollectedEntities();
                }
            }

            if (mBuffer.Count != 0)
            {
                Execute(mBuffer);
                mBuffer.ForEach(e => e.Release(this));
                mBuffer.Clear();
            }
        }

        public override string ToString()
        {
            if (mToStringCache == null)
            {
                mToStringCache = "MultiReactiveSystem(" + GetType().Name + ")";
            }

            return mToStringCache;
        }

        ~MultiReactiveSystem()
        {
            Deactivate();
        }
    }
}
