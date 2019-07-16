#if !NOT_UNITY3D

using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;
using Zenject.Internal;

namespace Zenject
{
    [NoReflectionBaking]
    public abstract class AddToGameObjectComponentProviderBase : IProvider
    {
        readonly Type _componentType;
        readonly DiContainer _container;
        readonly List<TypeValuePair> _extraArguments;
        readonly object _concreteIdentifier;
        readonly Action<InjectContext, object> _instantiateCallback;

        public AddToGameObjectComponentProviderBase(
            DiContainer container, Type componentType,
            IEnumerable<TypeValuePair> extraArguments, object concreteIdentifier,
            Action<InjectContext, object> instantiateCallback)
        {
            Assert.That(componentType.DerivesFrom<Component>());

            _extraArguments = extraArguments.ToList();
            _componentType = componentType;
            _container = container;
            _concreteIdentifier = concreteIdentifier;
            _instantiateCallback = instantiateCallback;
        }

        public bool IsCached
        {
            get { return false; }
        }

        public bool TypeVariesBasedOnMemberType
        {
            get { return false; }
        }

        protected DiContainer Container
        {
            get { return _container; }
        }

        protected Type ComponentType
        {
            get { return _componentType; }
        }

        protected abstract bool ShouldToggleActive
        {
            get;
        }

        public Type GetInstanceType(InjectContext context)
        {
            return _componentType;
        }

        public void GetAllInstancesWithInjectSplit(
            InjectContext context, List<TypeValuePair> args, out Action injectAction, List<object> buffer)
        {
            Assert.IsNotNull(context);

            object instance;

            // We still want to make sure we can get the game object during validation
            var gameObj = GetGameObject(context);

            var wasActive = gameObj.activeSelf;

            if (wasActive && ShouldToggleActive)
            {
                // We need to do this in some cases to ensure that [Inject] always gets
                // called before awake / start
                gameObj.SetActive(false);
            }

            if (!_container.IsValidating || TypeAnalyzer.ShouldAllowDuringValidation(_componentType))
            {
                if (_componentType == typeof(Transform))
                    // Treat transform as a special case because it's the one component that's always automatically added
                    // Otherwise, calling AddComponent below will fail and return null
                    // This is nice to allow doing things like
                    //      Container.Bind<Transform>().FromNewComponentOnNewGameObject();
                {
                    instance = gameObj.transform;
                }
                else
                {
                    instance = gameObj.AddComponent(_componentType);
                }

                Assert.IsNotNull(instance);
            }
            else
            {
                instance = new ValidationMarker(_componentType);
            }

            injectAction = () =>
            {
                try
                {
                    var extraArgs = ZenPools.SpawnList<TypeValuePair>();

                    extraArgs.AllocFreeAddRange(_extraArguments);
                    extraArgs.AllocFreeAddRange(args);

                    _container.InjectExplicit(instance, _componentType, extraArgs, context, _concreteIdentifier);

                    Assert.That(extraArgs.Count == 0);

                    ZenPools.DespawnList(extraArgs);

                    if (_instantiateCallback != null)
                    {
                        _instantiateCallback(context, instance);
                    }
                }
                finally
                {
                    if (wasActive && ShouldToggleActive)
                    {
                        gameObj.SetActive(true);
                    }
                }
            };

            buffer.Add(instance);
        }

        protected abstract GameObject GetGameObject(InjectContext context);
    }
}

#endif
