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
    public class AddToCurrentGameObjectComponentProvider : IProvider
    {
        readonly Type _componentType;
        readonly DiContainer _container;
        readonly List<TypeValuePair> _extraArguments;
        readonly object _concreteIdentifier;
        readonly Action<InjectContext, object> _instantiateCallback;

        public AddToCurrentGameObjectComponentProvider(
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

        public Type GetInstanceType(InjectContext context)
        {
            return _componentType;
        }

        public void GetAllInstancesWithInjectSplit(
            InjectContext context, List<TypeValuePair> args, out Action injectAction, List<object> buffer)
        {
            Assert.IsNotNull(context);

            Assert.That(context.ObjectType.DerivesFrom<Component>(),
                "Object '{0}' can only be injected into MonoBehaviour's since it was bound with 'FromNewComponentSibling'. Attempted to inject into non-MonoBehaviour '{1}'",
                context.MemberType, context.ObjectType);

            object instance;

            if (!_container.IsValidating || TypeAnalyzer.ShouldAllowDuringValidation(_componentType))
            {
                var gameObj = ((Component)context.ObjectInstance).gameObject;

                var componentInstance = gameObj.GetComponent(_componentType);
                instance = componentInstance;

                // Use componentInstance so that it triggers unity's overloaded comparison operator
                // So if the component is there but missing then it returns null
                // (https://github.com/svermeulen/Zenject/issues/582)
                if (componentInstance != null)
                {
                    injectAction = null;
                    buffer.Add(instance);
                    return;
                }

                instance = gameObj.AddComponent(_componentType);
            }
            else
            {
                instance = new ValidationMarker(_componentType);
            }

            // Note that we don't just use InstantiateComponentOnNewGameObjectExplicit here
            // because then circular references don't work

            injectAction = () =>
            {
                var extraArgs = ZenPools.SpawnList<TypeValuePair>();

                extraArgs.AllocFreeAddRange(_extraArguments);
                extraArgs.AllocFreeAddRange(args);

                _container.InjectExplicit(instance, _componentType, extraArgs, context, _concreteIdentifier);

                Assert.That(extraArgs.IsEmpty());
                ZenPools.DespawnList(extraArgs);

                if (_instantiateCallback != null)
                {
                    _instantiateCallback(context, instance);
                }
            };

            buffer.Add(instance);
        }
    }
}

#endif
