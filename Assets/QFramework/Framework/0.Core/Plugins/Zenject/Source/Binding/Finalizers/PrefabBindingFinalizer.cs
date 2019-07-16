#if !NOT_UNITY3D

using System;
using System.Collections.Generic;
using ModestTree;
using UnityEngine;

namespace Zenject
{
    [NoReflectionBaking]
    public class PrefabBindingFinalizer : ProviderBindingFinalizer
    {
        readonly GameObjectCreationParameters _gameObjectBindInfo;
        readonly UnityEngine.Object _prefab;
        readonly Func<Type, IPrefabInstantiator, IProvider> _providerFactory;

        public PrefabBindingFinalizer(
            BindInfo bindInfo,
            GameObjectCreationParameters gameObjectBindInfo,
            UnityEngine.Object prefab, Func<Type, IPrefabInstantiator, IProvider> providerFactory)
            : base(bindInfo)
        {
            _gameObjectBindInfo = gameObjectBindInfo;
            _prefab = prefab;
            _providerFactory = providerFactory;
        }

        protected override void OnFinalizeBinding(DiContainer container)
        {
            if (BindInfo.ToChoice == ToChoices.Self)
            {
                Assert.IsEmpty(BindInfo.ToTypes);
                FinalizeBindingSelf(container);
            }
            else
            {
                FinalizeBindingConcrete(container, BindInfo.ToTypes);
            }
        }

        void FinalizeBindingConcrete(DiContainer container, List<Type> concreteTypes)
        {
            var scope = GetScope();

            switch (scope)
            {
                case ScopeTypes.Transient:
                {
                    RegisterProvidersForAllContractsPerConcreteType(
                        container,
                        concreteTypes,
                        (_, concreteType) =>
                            _providerFactory(
                                concreteType,
                                new PrefabInstantiator(
                                    container,
                                    _gameObjectBindInfo,
                                    concreteType,
                                    concreteTypes,
                                    BindInfo.Arguments,
                                    new PrefabProvider(_prefab),
                                    BindInfo.InstantiatedCallback)));
                    break;
                }
                case ScopeTypes.Singleton:
                {
                    var argumentTarget = concreteTypes.OnlyOrDefault();

                    if (argumentTarget == null)
                    {
                        Assert.That(BindInfo.Arguments.IsEmpty(),
                            "Cannot provide arguments to prefab instantiator when using more than one concrete type");
                    }

                    var prefabCreator = new PrefabInstantiatorCached(
                        new PrefabInstantiator(
                            container,
                            _gameObjectBindInfo,
                            argumentTarget,
                            concreteTypes,
                            BindInfo.Arguments,
                            new PrefabProvider(_prefab),
                            BindInfo.InstantiatedCallback));

                    RegisterProvidersForAllContractsPerConcreteType(
                        container,
                        concreteTypes,
                        (_, concreteType) => BindingUtil.CreateCachedProvider(
                            _providerFactory(concreteType, prefabCreator)));
                    break;
                }
                default:
                {
                    throw Assert.CreateException();
                }
            }
        }

        void FinalizeBindingSelf(DiContainer container)
        {
            var scope = GetScope();

            switch (scope)
            {
                case ScopeTypes.Transient:
                {
                    RegisterProviderPerContract(
                        container,
                        (_, contractType) =>
                            _providerFactory(
                                contractType,
                                new PrefabInstantiator(
                                    container,
                                    _gameObjectBindInfo,
                                    contractType,
                                    BindInfo.ContractTypes,
                                    BindInfo.Arguments,
                                    new PrefabProvider(_prefab),
                                    BindInfo.InstantiatedCallback)));
                    break;
                }
                case ScopeTypes.Singleton:
                {
                    var argumentTarget = BindInfo.ContractTypes.OnlyOrDefault();

                    if (argumentTarget == null)
                    {
                        Assert.That(BindInfo.Arguments.IsEmpty(),
                            "Cannot provide arguments to prefab instantiator when using more than one concrete type");
                    }

                    var prefabCreator = new PrefabInstantiatorCached(
                        new PrefabInstantiator(
                            container,
                            _gameObjectBindInfo,
                            argumentTarget,
                            BindInfo.ContractTypes,
                            BindInfo.Arguments,
                            new PrefabProvider(_prefab),
                            BindInfo.InstantiatedCallback));

                    RegisterProviderPerContract(
                        container,
                        (_, contractType) =>
                            BindingUtil.CreateCachedProvider(
                                _providerFactory(contractType, prefabCreator)));
                    break;
                }
                default:
                {
                    throw Assert.CreateException();
                }
            }
        }
    }
}

#endif
