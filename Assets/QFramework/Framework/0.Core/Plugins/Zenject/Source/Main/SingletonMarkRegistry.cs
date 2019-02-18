using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject.Internal
{
    [NoReflectionBaking]
    public class SingletonMarkRegistry
    {
        readonly HashSet<Type> _boundSingletons = new HashSet<Type>();
        readonly HashSet<Type> _boundNonSingletons = new HashSet<Type>();

        public void MarkNonSingleton(Type type)
        {
            Assert.That(!_boundSingletons.Contains(type),
                "Found multiple creation bindings for type '{0}' in addition to AsSingle.  The AsSingle binding must be the definitive creation binding.  If this is intentional, use AsCached instead of AsSingle.", type);
            _boundNonSingletons.Add(type);
        }

        public void MarkSingleton(Type type)
        {
            bool added = _boundSingletons.Add(type);
            Assert.That(added, "Attempted to use AsSingle multiple times for type '{0}'.  As of Zenject 6+, AsSingle as can no longer be used for the same type across different bindings.  See the upgrade guide for details.", type);

            Assert.That(!_boundNonSingletons.Contains(type),
                "Found multiple creation bindings for type '{0}' in addition to AsSingle.  The AsSingle binding must be the definitive creation binding.  If this is intentional, use AsCached instead of AsSingle.", type);
        }

    }
}
