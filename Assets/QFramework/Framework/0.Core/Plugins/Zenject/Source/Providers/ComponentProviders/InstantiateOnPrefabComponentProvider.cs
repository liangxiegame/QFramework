#if !NOT_UNITY3D

using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    [NoReflectionBaking]
    public class InstantiateOnPrefabComponentProvider : IProvider
    {
        readonly IPrefabInstantiator _prefabInstantiator;
        readonly Type _componentType;

        // if concreteType is null we use the contract type from inject context
        public InstantiateOnPrefabComponentProvider(
            Type componentType,
            IPrefabInstantiator prefabInstantiator)
        {
            _prefabInstantiator = prefabInstantiator;
            _componentType = componentType;
        }

        public bool IsCached
        {
            get { return false; }
        }

        public bool TypeVariesBasedOnMemberType
        {
            get { return false; }
        }

        public Type GetInstanceType(InjectContext context)
        {
            return _componentType;
        }

        public void GetAllInstancesWithInjectSplit(
            InjectContext context, List<TypeValuePair> args, out Action injectAction, List<object> buffer)
        {
            Assert.IsNotNull(context);

            var gameObject = _prefabInstantiator.Instantiate(context, args, out injectAction);

            var component = gameObject.AddComponent(_componentType);

            buffer.Add(component);
        }
    }
}

#endif

