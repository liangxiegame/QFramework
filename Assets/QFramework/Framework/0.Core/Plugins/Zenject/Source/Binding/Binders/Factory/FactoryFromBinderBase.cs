using System;
using System.Collections.Generic;
using ModestTree;

#if !NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject
{
    [NoReflectionBaking]
    public class FactoryFromBinderBase : ScopeConcreteIdArgConditionCopyNonLazyBinder
    {
        public FactoryFromBinderBase(
            DiContainer bindContainer, Type contractType, BindInfo bindInfo, FactoryBindInfo factoryBindInfo)
            : base(bindInfo)
        {
            FactoryBindInfo = factoryBindInfo;
            BindContainer = bindContainer;
            ContractType = contractType;
            factoryBindInfo.ProviderFunc =
                (container) => new TransientProvider(
                    ContractType, container, BindInfo.Arguments, BindInfo.ContextInfo, BindInfo.ConcreteIdentifier,
                    BindInfo.InstantiatedCallback);
        }

        // Don't use this
        internal DiContainer BindContainer
        {
            get; private set;
        }

        protected FactoryBindInfo FactoryBindInfo
        {
            get; private set;
        }

        // Don't use this
        internal Func<DiContainer, IProvider> ProviderFunc
        {
            get { return FactoryBindInfo.ProviderFunc; }
            set { FactoryBindInfo.ProviderFunc = value; }
        }

        protected Type ContractType
        {
            get; private set;
        }

        public IEnumerable<Type> AllParentTypes
        {
            get
            {
                yield return ContractType;

                foreach (var type in BindInfo.ToTypes)
                {
                    yield return type;
                }
            }
        }

        // Note that this isn't necessary to call since it's the default
        public ConditionCopyNonLazyBinder FromNew()
        {
            BindingUtil.AssertIsNotComponent(ContractType);
            BindingUtil.AssertIsNotAbstract(ContractType);

            return this;
        }

        public ConditionCopyNonLazyBinder FromResolve()
        {
            return FromResolve(null);
        }

        public ConditionCopyNonLazyBinder FromInstance(object instance)
        {
            BindingUtil.AssertInstanceDerivesFromOrEqual(instance, AllParentTypes);

            ProviderFunc =
                (container) => new InstanceProvider(ContractType, instance, container);

            return this;
        }

        public ConditionCopyNonLazyBinder FromResolve(object subIdentifier)
        {
            ProviderFunc =
                (container) => new ResolveProvider(
                    ContractType, container,
                    subIdentifier, false, InjectSources.Any, false);

            return this;
        }

        // Don't use this
        internal ConcreteBinderGeneric<T> CreateIFactoryBinder<T>(out Guid factoryId)
        {
            // Use a random ID so that our provider is the only one that can find it and so it doesn't
            // conflict with anything else
            factoryId = Guid.NewGuid();

            // Very important here that we use NoFlush otherwise the main binding will be finalized early
            return BindContainer.BindNoFlush<T>().WithId(factoryId);
        }

#if !NOT_UNITY3D

        public ConditionCopyNonLazyBinder FromComponentOn(GameObject gameObject)
        {
            BindingUtil.AssertIsValidGameObject(gameObject);
            BindingUtil.AssertIsComponent(ContractType);
            BindingUtil.AssertIsNotAbstract(ContractType);

            ProviderFunc =
                (container) => new GetFromGameObjectComponentProvider(
                    ContractType, gameObject, true);

            return this;
        }

        public ConditionCopyNonLazyBinder FromComponentOn(Func<InjectContext, GameObject> gameObjectGetter)
        {
            BindingUtil.AssertIsComponent(ContractType);
            BindingUtil.AssertIsNotAbstract(ContractType);

            ProviderFunc =
                (container) => new GetFromGameObjectGetterComponentProvider(
                    ContractType, gameObjectGetter, true);

            return this;
        }

        public ConditionCopyNonLazyBinder FromComponentOnRoot()
        {
            return FromComponentOn(
                ctx => BindContainer.Resolve<Context>().gameObject);
        }

        public ConditionCopyNonLazyBinder FromNewComponentOn(GameObject gameObject)
        {
            BindingUtil.AssertIsValidGameObject(gameObject);
            BindingUtil.AssertIsComponent(ContractType);
            BindingUtil.AssertIsNotAbstract(ContractType);

            ProviderFunc =
                (container) => new AddToExistingGameObjectComponentProvider(
                    gameObject, container, ContractType,
                    new List<TypeValuePair>(), BindInfo.ConcreteIdentifier, BindInfo.InstantiatedCallback);

            return this;
        }

        public ConditionCopyNonLazyBinder FromNewComponentOn(
            Func<InjectContext, GameObject> gameObjectGetter)
        {
            BindingUtil.AssertIsComponent(ContractType);
            BindingUtil.AssertIsNotAbstract(ContractType);

            ProviderFunc =
                (container) => new AddToExistingGameObjectComponentProviderGetter(
                    gameObjectGetter, container, ContractType,
                    new List<TypeValuePair>(), BindInfo.ConcreteIdentifier, BindInfo.InstantiatedCallback);

            return this;
        }

        public NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder FromNewComponentOnNewGameObject()
        {
            BindingUtil.AssertIsComponent(ContractType);
            BindingUtil.AssertIsNotAbstract(ContractType);

            var gameObjectInfo = new GameObjectCreationParameters();

            ProviderFunc =
                (container) => new AddToNewGameObjectComponentProvider(
                    container, ContractType,
                    new List<TypeValuePair>(), gameObjectInfo, BindInfo.ConcreteIdentifier, BindInfo.InstantiatedCallback);

            return new NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder(BindInfo, gameObjectInfo);
        }

        public NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder FromNewComponentOnNewPrefab(UnityEngine.Object prefab)
        {
            BindingUtil.AssertIsValidPrefab(prefab);
            BindingUtil.AssertIsComponent(ContractType);
            BindingUtil.AssertIsNotAbstract(ContractType);

            var gameObjectInfo = new GameObjectCreationParameters();

            ProviderFunc =
                (container) => new InstantiateOnPrefabComponentProvider(
                    ContractType,
                    new PrefabInstantiator(
                        container, gameObjectInfo,
                        ContractType, new [] { ContractType }, new List<TypeValuePair>(),
                        new PrefabProvider(prefab), BindInfo.InstantiatedCallback));

            return new NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder(BindInfo, gameObjectInfo);
        }

        public NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder FromComponentInNewPrefab(UnityEngine.Object prefab)
        {
            BindingUtil.AssertIsValidPrefab(prefab);
            BindingUtil.AssertIsInterfaceOrComponent(ContractType);

            var gameObjectInfo = new GameObjectCreationParameters();

            ProviderFunc =
                (container) => new GetFromPrefabComponentProvider(
                    ContractType,
                    new PrefabInstantiator(
                        container, gameObjectInfo,
                        ContractType, new [] { ContractType }, new List<TypeValuePair>(),
                        new PrefabProvider(prefab),
                        BindInfo.InstantiatedCallback), true);

            return new NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder(BindInfo, gameObjectInfo);
        }

        public NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder FromComponentInNewPrefabResource(string resourcePath)
        {
            BindingUtil.AssertIsValidResourcePath(resourcePath);
            BindingUtil.AssertIsInterfaceOrComponent(ContractType);

            var gameObjectInfo = new GameObjectCreationParameters();

            ProviderFunc =
                (container) => new GetFromPrefabComponentProvider(
                    ContractType,
                    new PrefabInstantiator(
                        container, gameObjectInfo,
                        ContractType, new [] { ContractType }, new List<TypeValuePair>(),
                        new PrefabProviderResource(resourcePath), BindInfo.InstantiatedCallback), true);

            return new NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder(BindInfo, gameObjectInfo);
        }

        public NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder FromNewComponentOnNewPrefabResource(string resourcePath)
        {
            BindingUtil.AssertIsValidResourcePath(resourcePath);
            BindingUtil.AssertIsComponent(ContractType);
            BindingUtil.AssertIsNotAbstract(ContractType);

            var gameObjectInfo = new GameObjectCreationParameters();

            ProviderFunc =
                (container) => new InstantiateOnPrefabComponentProvider(
                    ContractType,
                    new PrefabInstantiator(
                        container, gameObjectInfo,
                        ContractType, new [] { ContractType }, new List<TypeValuePair>(),
                        new PrefabProviderResource(resourcePath),
                        BindInfo.InstantiatedCallback));

            return new NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder(BindInfo, gameObjectInfo);
        }

        public ConditionCopyNonLazyBinder FromNewScriptableObjectResource(string resourcePath)
        {
            BindingUtil.AssertIsValidResourcePath(resourcePath);
            BindingUtil.AssertIsInterfaceOrScriptableObject(ContractType);

            ProviderFunc =
                (container) => new ScriptableObjectResourceProvider(
                    resourcePath, ContractType, container, new List<TypeValuePair>(),
                    true, null, BindInfo.InstantiatedCallback);

            return this;
        }

        public ConditionCopyNonLazyBinder FromScriptableObjectResource(string resourcePath)
        {
            BindingUtil.AssertIsValidResourcePath(resourcePath);
            BindingUtil.AssertIsInterfaceOrScriptableObject(ContractType);

            ProviderFunc =
                (container) => new ScriptableObjectResourceProvider(
                    resourcePath, ContractType, container, new List<TypeValuePair>(),
                    false, null, BindInfo.InstantiatedCallback);

            return this;
        }

        public ConditionCopyNonLazyBinder FromResource(string resourcePath)
        {
            BindingUtil.AssertDerivesFromUnityObject(ContractType);

            ProviderFunc =
                (container) => new ResourceProvider(resourcePath, ContractType, true);

            return this;
        }
#endif
    }
}
