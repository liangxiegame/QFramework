using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using ModestTree.Util;

namespace Zenject
{
    public class PoolableManager
    {
        readonly List<IPoolable> _poolables;

        bool _isSpawned;

        public PoolableManager(
            [InjectLocal]
            List<IPoolable> poolables,
            [Inject(Optional = true, Source = InjectSources.Local)]
            List<ValuePair<Type, int>> priorities)
        {
            _poolables = poolables.Select(x => CreatePoolableInfo(x, priorities))
                .OrderBy(x => x.Priority).Select(x => x.Poolable).ToList();
        }

        PoolableInfo CreatePoolableInfo(IPoolable poolable, List<ValuePair<Type, int>> priorities)
        {
            var match = priorities.Where(x => poolable.GetType().DerivesFromOrEqual(x.First)).Select(x => (int?)(x.Second)).SingleOrDefault();
            int priority = match.HasValue ? match.Value : 0;

            return new PoolableInfo(poolable, priority);
        }

        public void TriggerOnSpawned()
        {
            Assert.That(!_isSpawned);
            _isSpawned = true;

            for (int i = 0; i < _poolables.Count; i++)
            {
#if ZEN_INTERNAL_PROFILING
                using (ProfileTimers.CreateTimedBlock("User Code"))
#endif
#if UNITY_EDITOR
                using (ProfileBlock.Start("{0}.OnSpawned", _poolables[i].GetType()))
#endif
                {
                    _poolables[i].OnSpawned();
                }
            }
        }

        public void TriggerOnDespawned()
        {
            Assert.That(_isSpawned);
            _isSpawned = false;

            // Call OnDespawned in the reverse order just like how dispose works
            for (int i = _poolables.Count - 1; i >= 0; i--)
            {
#if ZEN_INTERNAL_PROFILING
                using (ProfileTimers.CreateTimedBlock("User Code"))
#endif
#if UNITY_EDITOR
                using (ProfileBlock.Start("{0}.OnDespawned", _poolables[i].GetType()))
#endif
                {
                    _poolables[i].OnDespawned();
                }
            }
        }

        struct PoolableInfo
        {
            public IPoolable Poolable;
            public int Priority;

            public PoolableInfo(IPoolable poolable, int priority)
            {
                Poolable = poolable;
                Priority = priority;
            }
        }
    }
}
