using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;

namespace Zenject
{
    // If you want to ensure that all items are always returned to the pool, include the following
    // in an installer
    // Container.BindInterfacesTo<PoolCleanupChecker>().AsSingle()
    public class PoolCleanupChecker : ILateDisposable
    {
        readonly List<IMemoryPool> _poolFactories;
        readonly List<Type> _ignoredPools;

        public PoolCleanupChecker(
            [Inject(Optional = true, Source = InjectSources.Local)]
            List<IMemoryPool> poolFactories,
            [Inject(Source = InjectSources.Local)]
            List<Type> ignoredPools)
        {
            _poolFactories = poolFactories;
            _ignoredPools = ignoredPools;

            Assert.That(ignoredPools.All(x => x.DerivesFrom<IMemoryPool>()));
        }

        public void LateDispose()
        {
            foreach (var pool in _poolFactories)
            {
                if (!_ignoredPools.Contains(pool.GetType()))
                {
                    Assert.IsEqual(pool.NumActive, 0,
                        "Found active objects in pool '{0}' during dispose.  Did you forget to despawn an object of type '{1}'?".Fmt(pool.GetType(), pool.ItemType));
                }
            }
        }
    }
}
