using System;
using ModestTree;

namespace Zenject
{
    [NoReflectionBaking]
    public class SubContainerBinder
    {
        readonly BindInfo _bindInfo;
        readonly BindStatement _bindStatement;
        readonly object _subIdentifier;
        readonly bool _resolveAll;

        public SubContainerBinder(
            BindInfo bindInfo,
            BindStatement bindStatement,
            object subIdentifier, bool resolveAll)
        {
            _bindInfo = bindInfo;
            _bindStatement = bindStatement;
            _subIdentifier = subIdentifier;
            _resolveAll = resolveAll;

            // Reset in case the user ends the binding here
            bindStatement.SetFinalizer(null);
        }

        protected IBindingFinalizer SubFinalizer
        {
            set { _bindStatement.SetFinalizer(value); }
        }

        public ScopeConcreteIdArgConditionCopyNonLazyBinder ByInstance(DiContainer subContainer)
        {
            SubFinalizer = new SubContainerBindingFinalizer(
                _bindInfo, _subIdentifier, _resolveAll,
                (_) => new SubContainerCreatorByInstance(subContainer));

            return new ScopeConcreteIdArgConditionCopyNonLazyBinder(_bindInfo);
        }

        public ScopeConcreteIdArgConditionCopyNonLazyBinder ByInstanceGetter(
            Func<InjectContext, DiContainer> subContainerGetter)
        {
            SubFinalizer = new SubContainerBindingFinalizer(
                _bindInfo, _subIdentifier, _resolveAll,
                (_) => new SubContainerCreatorByInstanceGetter(subContainerGetter));

            return new ScopeConcreteIdArgConditionCopyNonLazyBinder(_bindInfo);
        }

        public
#if NOT_UNITY3D
            WithKernelScopeConcreteIdArgConditionCopyNonLazyBinder
#else
            WithKernelDefaultParentScopeConcreteIdArgConditionCopyNonLazyBinder
#endif
            ByInstaller<TInstaller>()
            where TInstaller : InstallerBase
        {
            return ByInstaller(typeof(TInstaller));
        }

        public
#if NOT_UNITY3D
            WithKernelScopeConcreteIdArgConditionCopyNonLazyBinder
#else
            WithKernelDefaultParentScopeConcreteIdArgConditionCopyNonLazyBinder
#endif
            ByInstaller(Type installerType)
        {
            Assert.That(installerType.DerivesFrom<InstallerBase>(),
                "Invalid installer type given during bind command.  Expected type '{0}' to derive from 'Installer<>'", installerType);

            var subContainerBindInfo = new SubContainerCreatorBindInfo();

            SubFinalizer = new SubContainerBindingFinalizer(
                _bindInfo, _subIdentifier, _resolveAll,
                (container) => new SubContainerCreatorByInstaller(container, subContainerBindInfo, installerType));

            return new
#if NOT_UNITY3D
                WithKernelScopeConcreteIdArgConditionCopyNonLazyBinder
#else
                WithKernelDefaultParentScopeConcreteIdArgConditionCopyNonLazyBinder
#endif
                (subContainerBindInfo, _bindInfo);
        }

        public
#if NOT_UNITY3D
            WithKernelScopeConcreteIdArgConditionCopyNonLazyBinder
#else
            WithKernelDefaultParentScopeConcreteIdArgConditionCopyNonLazyBinder
#endif
            ByMethod(Action<DiContainer> installerMethod)
        {
            var subContainerBindInfo = new SubContainerCreatorBindInfo();

            SubFinalizer = new SubContainerBindingFinalizer(
                _bindInfo, _subIdentifier, _resolveAll,
                (container) => new SubContainerCreatorByMethod(container, subContainerBindInfo, installerMethod));

            return new
#if NOT_UNITY3D
                WithKernelScopeConcreteIdArgConditionCopyNonLazyBinder
#else
                WithKernelDefaultParentScopeConcreteIdArgConditionCopyNonLazyBinder
#endif
                (subContainerBindInfo, _bindInfo);
        }

#if !NOT_UNITY3D

        public NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder ByNewGameObjectMethod(
            Action<DiContainer> installerMethod)
        {
            var gameObjectInfo = new GameObjectCreationParameters();

            SubFinalizer = new SubContainerPrefabBindingFinalizer(
                _bindInfo, _subIdentifier, _resolveAll,
                (container) => new SubContainerCreatorByNewGameObjectMethod(
                    container, gameObjectInfo, installerMethod));

            return new NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder(_bindInfo, gameObjectInfo);
        }

        public NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder ByNewPrefabMethod(
            UnityEngine.Object prefab, Action<DiContainer> installerMethod)
        {
            BindingUtil.AssertIsValidPrefab(prefab);

            var gameObjectInfo = new GameObjectCreationParameters();

            SubFinalizer = new SubContainerPrefabBindingFinalizer(
                _bindInfo, _subIdentifier, _resolveAll,
                (container) => new SubContainerCreatorByNewPrefabMethod(
                    container,
                    new PrefabProvider(prefab),
                    gameObjectInfo, installerMethod));

            return new NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder(_bindInfo, gameObjectInfo);
        }

        public NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder ByNewGameObjectInstaller<TInstaller>()
            where TInstaller : InstallerBase
        {
            return ByNewGameObjectInstaller(typeof(TInstaller));
        }

        public NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder ByNewGameObjectInstaller(Type installerType)
        {
            Assert.That(installerType.DerivesFrom<InstallerBase>(),
                "Invalid installer type given during bind command.  Expected type '{0}' to derive from 'Installer<>'", installerType);

            var gameObjectInfo = new GameObjectCreationParameters();

            SubFinalizer = new SubContainerPrefabBindingFinalizer(
                _bindInfo, _subIdentifier, _resolveAll,
                (container) => new SubContainerCreatorByNewGameObjectInstaller(
                    container, gameObjectInfo, installerType, _bindInfo.Arguments));

            return new NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder(_bindInfo, gameObjectInfo);
        }

        public NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder ByNewPrefabInstaller<TInstaller>(
            UnityEngine.Object prefab)
            where TInstaller : InstallerBase
        {
            return ByNewPrefabInstaller(prefab, typeof(TInstaller));
        }

        public NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder ByNewPrefabInstaller(
            UnityEngine.Object prefab, Type installerType)
        {
            Assert.That(installerType.DerivesFrom<InstallerBase>(),
                "Invalid installer type given during bind command.  Expected type '{0}' to derive from 'Installer<>'", installerType);

            var gameObjectInfo = new GameObjectCreationParameters();

            SubFinalizer = new SubContainerPrefabBindingFinalizer(
                _bindInfo, _subIdentifier, _resolveAll,
                (container) => new SubContainerCreatorByNewPrefabInstaller(
                    container,
                    new PrefabProvider(prefab),
                    gameObjectInfo, installerType, _bindInfo.Arguments));

            return new NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder(_bindInfo, gameObjectInfo);
        }


        public NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder ByNewPrefabResourceMethod(
            string resourcePath, Action<DiContainer> installerMethod)
        {
            BindingUtil.AssertIsValidResourcePath(resourcePath);

            var gameObjectInfo = new GameObjectCreationParameters();

            SubFinalizer = new SubContainerPrefabBindingFinalizer(
                _bindInfo, _subIdentifier, _resolveAll,
                (container) => new SubContainerCreatorByNewPrefabMethod(
                    container,
                    new PrefabProviderResource(resourcePath),
                    gameObjectInfo, installerMethod));

            return new NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder(_bindInfo, gameObjectInfo);
        }

        public NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder ByNewPrefabResourceInstaller<TInstaller>(
            string resourcePath)
            where TInstaller : InstallerBase
        {
            return ByNewPrefabResourceInstaller(resourcePath, typeof(TInstaller));
        }

        public NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder ByNewPrefabResourceInstaller(
            string resourcePath, Type installerType)
        {
            BindingUtil.AssertIsValidResourcePath(resourcePath);
            Assert.That(installerType.DerivesFrom<InstallerBase>(),
                "Invalid installer type given during bind command.  Expected type '{0}' to derive from 'Installer<>'", installerType);

            var gameObjectInfo = new GameObjectCreationParameters();

            SubFinalizer = new SubContainerPrefabBindingFinalizer(
                _bindInfo, _subIdentifier, _resolveAll,
                (container) => new SubContainerCreatorByNewPrefabInstaller(
                    container,
                    new PrefabProviderResource(resourcePath),
                    gameObjectInfo, installerType, _bindInfo.Arguments));

            return new NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder(_bindInfo, gameObjectInfo);
        }

        [System.Obsolete("ByNewPrefab has been renamed to ByNewContextPrefab to avoid confusion with ByNewPrefabInstaller and ByNewPrefabMethod")]
        public NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder ByNewPrefab(UnityEngine.Object prefab)
        {
            return ByNewContextPrefab(prefab);
        }

        public NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder ByNewContextPrefab(UnityEngine.Object prefab)
        {
            BindingUtil.AssertIsValidPrefab(prefab);

            var gameObjectInfo = new GameObjectCreationParameters();

            SubFinalizer = new SubContainerPrefabBindingFinalizer(
                _bindInfo, _subIdentifier, _resolveAll,
                (container) => new SubContainerCreatorByNewPrefab(
                    container, new PrefabProvider(prefab), gameObjectInfo));

            return new NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder(_bindInfo, gameObjectInfo);
        }

        [System.Obsolete("ByNewPrefabResource has been renamed to ByNewContextPrefabResource to avoid confusion with ByNewPrefabResourceInstaller and ByNewPrefabResourceMethod")]
        public NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder ByNewPrefabResource(string resourcePath)
        {
            return ByNewContextPrefabResource(resourcePath);
        }

        public NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder ByNewContextPrefabResource(string resourcePath)
        {
            BindingUtil.AssertIsValidResourcePath(resourcePath);

            var gameObjectInfo = new GameObjectCreationParameters();

            SubFinalizer = new SubContainerPrefabBindingFinalizer(
                _bindInfo, _subIdentifier, _resolveAll,
                (container) => new SubContainerCreatorByNewPrefab(
                    container, new PrefabProviderResource(resourcePath), gameObjectInfo));

            return new NameTransformScopeConcreteIdArgConditionCopyNonLazyBinder(_bindInfo, gameObjectInfo);
        }
#endif
    }
}
