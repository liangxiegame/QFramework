#if !NOT_UNITY3D

using System;
using System.Collections.Generic;
using ModestTree;
using UnityEngine;

namespace Zenject
{
    [NoReflectionBaking]
    public class GetFromGameObjectGetterComponentProvider : IProvider
    {
        readonly Func<InjectContext, GameObject> _gameObjectGetter;
        readonly Type _componentType;
        readonly bool _matchSingle;

        // if concreteType is null we use the contract type from inject context
        public GetFromGameObjectGetterComponentProvider(
            Type componentType, Func<InjectContext, GameObject> gameObjectGetter, bool matchSingle)
        {
            _componentType = componentType;
            _matchSingle = matchSingle;
            _gameObjectGetter = gameObjectGetter;
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

            injectAction = null;

            if (context.Container.IsValidating)
            {
                buffer.Add(new ValidationMarker(_componentType));
            }
            else
            {
                var gameObject = _gameObjectGetter(context);

                if (_matchSingle)
                {
                    var match = gameObject.GetComponent(_componentType);

                    Assert.IsNotNull(match, "Could not find component with type '{0}' on game object '{1}'",
                    _componentType, gameObject.name);

                    buffer.Add(match);
                    return;
                }

                var allComponents = gameObject.GetComponents(_componentType);

                Assert.That(allComponents.Length >= 1,
                "Expected to find at least one component with type '{0}' on prefab '{1}'",
                _componentType, gameObject.name);

                buffer.AllocFreeAddRange(allComponents);
            }
        }
    }
}

#endif


