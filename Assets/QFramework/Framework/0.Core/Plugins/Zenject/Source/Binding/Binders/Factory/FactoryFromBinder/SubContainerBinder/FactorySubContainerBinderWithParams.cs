using System;
using ModestTree;

namespace Zenject
{
    [NoReflectionBaking]
    public class FactorySubContainerBinderWithParams<TContract> : FactorySubContainerBinderBase<TContract>
    {
        public FactorySubContainerBinderWithParams(
            DiContainer bindContainer, BindInfo bindInfo, FactoryBindInfo factoryBindInfo, object subIdentifier)
            : base(bindContainer, bindInfo, factoryBindInfo, subIdentifier)
        {
        }

#if !NOT_UNITY3D

        [System.Obsolete("ByNewPrefab has been renamed to ByNewContextPrefab to avoid confusion with ByNewPrefabInstaller and ByNewPrefabMethod")]
        public NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder ByNewPrefab(Type installerType, UnityEngine.Object prefab)
        {
            return ByNewContextPrefab(installerType, prefab);
        }

        [System.Obsolete("ByNewPrefab has been renamed to ByNewContextPrefab to avoid confusion with ByNewPrefabInstaller and ByNewPrefabMethod")]
        public NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder ByNewPrefab<TInstaller>(UnityEngine.Object prefab)
            where TInstaller : IInstaller
        {
            return ByNewContextPrefab<TInstaller>(prefab);
        }

        public NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder ByNewContextPrefab<TInstaller>(UnityEngine.Object prefab)
            where TInstaller : IInstaller
        {
            return ByNewContextPrefab(typeof(TInstaller), prefab);
        }

        public NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder ByNewContextPrefab(Type installerType, UnityEngine.Object prefab)
        {
            BindingUtil.AssertIsValidPrefab(prefab);

            Assert.That(installerType.DerivesFrom<MonoInstaller>(),
                "Invalid installer type given during bind command.  Expected type '{0}' to derive from 'MonoInstaller'", installerType);

            var gameObjectInfo = new GameObjectCreationParameters();

            ProviderFunc = 
                (container) => new SubContainerDependencyProvider(
                    ContractType, SubIdentifier,
                    new SubContainerCreatorByNewPrefabWithParams(
                        installerType,
                        container,
                        new PrefabProvider(prefab),
                        gameObjectInfo), false);

            return new NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder(BindInfo, gameObjectInfo);
        }

        [System.Obsolete("ByNewPrefabResource has been renamed to ByNewContextPrefabResource to avoid confusion with ByNewPrefabResourceInstaller and ByNewPrefabResourceMethod")]
        public NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder ByNewPrefabResource<TInstaller>(string resourcePath)
            where TInstaller : IInstaller
        {
            return ByNewContextPrefabResource<TInstaller>(resourcePath);
        }

        public NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder ByNewPrefabResource(
            Type installerType, string resourcePath)
        {
            return ByNewContextPrefabResource(installerType, resourcePath);
        }

        public NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder ByNewContextPrefabResource<TInstaller>(string resourcePath)
            where TInstaller : IInstaller
        {
            return ByNewContextPrefabResource(typeof(TInstaller), resourcePath);
        }

        public NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder ByNewContextPrefabResource(
            Type installerType, string resourcePath)
        {
            BindingUtil.AssertIsValidResourcePath(resourcePath);

            var gameObjectInfo = new GameObjectCreationParameters();

            ProviderFunc = 
                (container) => new SubContainerDependencyProvider(
                    ContractType, SubIdentifier,
                    new SubContainerCreatorByNewPrefabWithParams(
                        installerType,
                        container,
                        new PrefabProviderResource(resourcePath),
                        gameObjectInfo), false);

            return new NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder(BindInfo, gameObjectInfo);
        }
#endif
    }
}
