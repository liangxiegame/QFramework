using System.Collections.Generic;

namespace QFramework
{
    /// Systems provide a convenient way to group systems.
    /// You can add IInitializeSystem, IExecuteSystem, ICleanupSystem,
    /// ITearDownSystem, ReactiveS
    /// ystem and other nested Systems instances.
    /// All systems will be initialized and executed based on the order
    /// you added them.
    public class Systems : IInitializeSystem, IExecuteSystem, ICleanupSystem, ITearDownSystem
    {
        protected readonly List<IInitializeSystem> mInitializeSystems;
        protected readonly List<IExecuteSystem> mExecuteSystems;
        protected readonly List<ICleanupSystem> mCleanupSystems;
        protected readonly List<ITearDownSystem> mTearDownSystems;

        /// Creates a new Systems instance.
        public Systems()
        {
            mInitializeSystems = new List<IInitializeSystem>();
            mExecuteSystems = new List<IExecuteSystem>();
            mCleanupSystems = new List<ICleanupSystem>();
            mTearDownSystems = new List<ITearDownSystem>();
        }

        /// Adds the system instance to the systems list.
        public virtual Systems Add(ISystem system)
        {
            var initializeSystem = system as IInitializeSystem;
            if (initializeSystem != null)
            {
                mInitializeSystems.Add(initializeSystem);
            }

            var executeSystem = system as IExecuteSystem;
            if (executeSystem != null)
            {
                mExecuteSystems.Add(executeSystem);
            }

            var cleanupSystem = system as ICleanupSystem;
            if (cleanupSystem != null)
            {
                mCleanupSystems.Add(cleanupSystem);
            }

            var tearDownSystem = system as ITearDownSystem;
            if (tearDownSystem != null)
            {
                mTearDownSystems.Add(tearDownSystem);
            }

            return this;
        }

        /// Calls Initialize() on all IInitializeSystem and other
        /// nested Systems instances in the order you added them.
        public virtual void Initialize()
        {
            mInitializeSystems.ForEach(system => system.Initialize());
        }

        /// Calls Execute() on all IExecuteSystem and other
        /// nested Systems instances in the order you added them.
        public virtual void Execute()
        {
            mExecuteSystems.ForEach(system => system.Execute());
        }

        /// Calls Cleanup() on all ICleanupSystem and other
        /// nested Systems instances in the order you added them.
        public virtual void Cleanup()
        {
            mCleanupSystems.ForEach(system => system.Cleanup());
        }

        /// Calls TearDown() on all ITearDownSystem  and other
        /// nested Systems instances in the order you added them.
        public virtual void TearDown()
        {
            mTearDownSystems.ForEach(system => system.TearDown());
        }

        /// Activates all ReactiveSystems in the systems list.
        public void ActivateReactiveSystems()
        {
            foreach (var system in mExecuteSystems)
            {
                var reactiveSystem = system as IReactiveSystem;
                if (reactiveSystem != null)
                {
                    reactiveSystem.Activate();
                }

                var nestedSystems = system as Systems;
                if (nestedSystems != null)
                {
                    nestedSystems.ActivateReactiveSystems();
                }
            }
        }

        /// Deactivates all ReactiveSystems in the systems list.
        /// This will also clear all ReactiveSystems.
        /// This is useful when you want to soft-restart your application and
        /// want to reuse your existing system instances.
        public void DeactivateReactiveSystems()
        {
            foreach (var system in mExecuteSystems)
            {
                var reactiveSystem = system as IReactiveSystem;
                if (reactiveSystem != null)
                {
                    reactiveSystem.Deactivate();
                }

                var nestedSystems = system as Systems;
                if (nestedSystems != null)
                {
                    nestedSystems.DeactivateReactiveSystems();
                }
            }
        }

        /// Clears all ReactiveSystems in the systems list.
        public void ClearReactiveSystems()
        {
            foreach (var system in mExecuteSystems)
            {
                var reactiveSystem = system as IReactiveSystem;
                if (reactiveSystem != null)
                {
                    reactiveSystem.Clear();
                }

                var nestedSystems = system as Systems;
                if (nestedSystems != null)
                {
                    nestedSystems.ClearReactiveSystems();
                }
            }
        }
    }
}