#if !NOT_UNITY3D

using System;
using System.Collections.Generic;
using UnityEngine;
using ModestTree;
using Zenject.Internal;

namespace Zenject
{
    [NoReflectionBaking]
    public abstract class SubContainerCreatorByNewGameObjectDynamicContext : SubContainerCreatorDynamicContext
    {
        readonly GameObjectCreationParameters _gameObjectBindInfo;

        public SubContainerCreatorByNewGameObjectDynamicContext(
            DiContainer container, GameObjectCreationParameters gameObjectBindInfo)
            : base(container)
        {
            _gameObjectBindInfo = gameObjectBindInfo;
        }

        protected override GameObject CreateGameObject(out bool shouldMakeActive)
        {
            shouldMakeActive = true;
            var gameObject = Container.CreateEmptyGameObject(_gameObjectBindInfo, null);
            gameObject.SetActive(false);
            return gameObject;
        }
    }

    [NoReflectionBaking]
    public class SubContainerCreatorByNewGameObjectInstaller : SubContainerCreatorByNewGameObjectDynamicContext
    {
        readonly Type _installerType;
        readonly List<TypeValuePair> _extraArgs;

        public SubContainerCreatorByNewGameObjectInstaller(
            DiContainer container,
            GameObjectCreationParameters gameObjectBindInfo,
            Type installerType, List<TypeValuePair> extraArgs)
            : base(container, gameObjectBindInfo)
        {
            _installerType = installerType;
            _extraArgs = extraArgs;

            Assert.That(installerType.DerivesFrom<InstallerBase>(),
                "Invalid installer type given during bind command.  Expected type '{0}' to derive from 'Installer<>'", installerType);
        }

        protected override void AddInstallers(List<TypeValuePair> args, GameObjectContext context)
        {
            context.AddNormalInstaller(
                new ActionInstaller(subContainer =>
                    {
                        var extraArgs = ZenPools.SpawnList<TypeValuePair>();

                        extraArgs.AllocFreeAddRange(_extraArgs);
                        extraArgs.AllocFreeAddRange(args);

                        var installer = (InstallerBase)subContainer.InstantiateExplicit(
                            _installerType, extraArgs);

                        ZenPools.DespawnList(extraArgs);

                        installer.InstallBindings();
                    }));
        }
    }

    [NoReflectionBaking]
    public class SubContainerCreatorByNewGameObjectMethod : SubContainerCreatorByNewGameObjectDynamicContext
    {
        readonly Action<DiContainer> _installerMethod;

        public SubContainerCreatorByNewGameObjectMethod(
            DiContainer container,
            GameObjectCreationParameters gameObjectBindInfo,
            Action<DiContainer> installerMethod)
            : base(container, gameObjectBindInfo)
        {
            _installerMethod = installerMethod;
        }

        protected override void AddInstallers(List<TypeValuePair> args, GameObjectContext context)
        {
            Assert.That(args.IsEmpty());
            context.AddNormalInstaller(
                new ActionInstaller(_installerMethod));
        }
    }

    [NoReflectionBaking]
    public class SubContainerCreatorByNewGameObjectMethod<TParam1> : SubContainerCreatorByNewGameObjectDynamicContext
    {
        readonly Action<DiContainer, TParam1> _installerMethod;

        public SubContainerCreatorByNewGameObjectMethod(
            DiContainer container,
            GameObjectCreationParameters gameObjectBindInfo,
            Action<DiContainer, TParam1> installerMethod)
            : base(container, gameObjectBindInfo)
        {
            _installerMethod = installerMethod;
        }

        protected override void AddInstallers(List<TypeValuePair> args, GameObjectContext context)
        {
            Assert.IsEqual(args.Count, 1);
            Assert.That(args[0].Type.DerivesFromOrEqual<TParam1>());

            context.AddNormalInstaller(
                new ActionInstaller(subContainer =>
                    {
                        _installerMethod(subContainer, (TParam1)args[0].Value);
                    }));
        }
    }

    [NoReflectionBaking]
    public class SubContainerCreatorByNewGameObjectMethod<TParam1, TParam2> : SubContainerCreatorByNewGameObjectDynamicContext
    {
        readonly Action<DiContainer, TParam1, TParam2> _installerMethod;

        public SubContainerCreatorByNewGameObjectMethod(
            DiContainer container,
            GameObjectCreationParameters gameObjectBindInfo,
            Action<DiContainer, TParam1, TParam2> installerMethod)
            : base(container, gameObjectBindInfo)
        {
            _installerMethod = installerMethod;
        }

        protected override void AddInstallers(List<TypeValuePair> args, GameObjectContext context)
        {
            Assert.IsEqual(args.Count, 2);
            Assert.That(args[0].Type.DerivesFromOrEqual<TParam1>());
            Assert.That(args[1].Type.DerivesFromOrEqual<TParam2>());

            context.AddNormalInstaller(
                new ActionInstaller(subContainer =>
                    {
                        _installerMethod(subContainer,
                            (TParam1)args[0].Value,
                            (TParam2)args[1].Value);
                    }));
        }
    }

    [NoReflectionBaking]
    public class SubContainerCreatorByNewGameObjectMethod<TParam1, TParam2, TParam3> : SubContainerCreatorByNewGameObjectDynamicContext
    {
        readonly Action<DiContainer, TParam1, TParam2, TParam3> _installerMethod;

        public SubContainerCreatorByNewGameObjectMethod(
            DiContainer container,
            GameObjectCreationParameters gameObjectBindInfo,
            Action<DiContainer, TParam1, TParam2, TParam3> installerMethod)
            : base(container, gameObjectBindInfo)
        {
            _installerMethod = installerMethod;
        }

        protected override void AddInstallers(List<TypeValuePair> args, GameObjectContext context)
        {
            Assert.IsEqual(args.Count, 3);
            Assert.That(args[0].Type.DerivesFromOrEqual<TParam1>());
            Assert.That(args[1].Type.DerivesFromOrEqual<TParam2>());
            Assert.That(args[2].Type.DerivesFromOrEqual<TParam3>());

            context.AddNormalInstaller(
                new ActionInstaller(subContainer =>
                    {
                        _installerMethod(subContainer,
                            (TParam1)args[0].Value,
                            (TParam2)args[1].Value,
                            (TParam3)args[2].Value);
                    }));
        }
    }

    [NoReflectionBaking]
    public class SubContainerCreatorByNewGameObjectMethod<TParam1, TParam2, TParam3, TParam4> : SubContainerCreatorByNewGameObjectDynamicContext
    {
        readonly
#if !NET_4_6
            ModestTree.Util.
#endif
            Action<DiContainer, TParam1, TParam2, TParam3, TParam4> _installerMethod;

        public SubContainerCreatorByNewGameObjectMethod(
            DiContainer container,
            GameObjectCreationParameters gameObjectBindInfo,
#if !NET_4_6
            ModestTree.Util.
#endif
            Action<DiContainer, TParam1, TParam2, TParam3, TParam4> installerMethod)
            : base(container, gameObjectBindInfo)
        {
            _installerMethod = installerMethod;
        }

        protected override void AddInstallers(List<TypeValuePair> args, GameObjectContext context)
        {
            Assert.IsEqual(args.Count, 4);
            Assert.That(args[0].Type.DerivesFromOrEqual<TParam1>());
            Assert.That(args[1].Type.DerivesFromOrEqual<TParam2>());
            Assert.That(args[2].Type.DerivesFromOrEqual<TParam3>());
            Assert.That(args[3].Type.DerivesFromOrEqual<TParam4>());

            context.AddNormalInstaller(
                new ActionInstaller(subContainer =>
                    {
                        _installerMethod(subContainer,
                            (TParam1)args[0].Value,
                            (TParam2)args[1].Value,
                            (TParam3)args[2].Value,
                            (TParam4)args[3].Value);
                    }));
        }
    }

    [NoReflectionBaking]
    public class SubContainerCreatorByNewGameObjectMethod<TParam1, TParam2, TParam3, TParam4, TParam5> : SubContainerCreatorByNewGameObjectDynamicContext
    {
        readonly
#if !NET_4_6
            ModestTree.Util.
#endif
            Action<DiContainer, TParam1, TParam2, TParam3, TParam4, TParam5> _installerMethod;

        public SubContainerCreatorByNewGameObjectMethod(
            DiContainer container,
            GameObjectCreationParameters gameObjectBindInfo,
#if !NET_4_6
            ModestTree.Util.
#endif
            Action<DiContainer, TParam1, TParam2, TParam3, TParam4, TParam5> installerMethod)
            : base(container, gameObjectBindInfo)
        {
            _installerMethod = installerMethod;
        }

        protected override void AddInstallers(List<TypeValuePair> args, GameObjectContext context)
        {
            Assert.IsEqual(args.Count, 5);
            Assert.That(args[0].Type.DerivesFromOrEqual<TParam1>());
            Assert.That(args[1].Type.DerivesFromOrEqual<TParam2>());
            Assert.That(args[2].Type.DerivesFromOrEqual<TParam3>());
            Assert.That(args[3].Type.DerivesFromOrEqual<TParam4>());
            Assert.That(args[4].Type.DerivesFromOrEqual<TParam5>());

            context.AddNormalInstaller(
                new ActionInstaller(subContainer =>
                    {
                        _installerMethod(subContainer,
                            (TParam1)args[0].Value,
                            (TParam2)args[1].Value,
                            (TParam3)args[2].Value,
                            (TParam4)args[3].Value,
                            (TParam5)args[4].Value);
                    }));
        }
    }

    [NoReflectionBaking]
    public class SubContainerCreatorByNewGameObjectMethod<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> : SubContainerCreatorByNewGameObjectDynamicContext
    {
        readonly
#if !NET_4_6
            ModestTree.Util.
#endif
            Action<DiContainer, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> _installerMethod;

        public SubContainerCreatorByNewGameObjectMethod(
            DiContainer container,
            GameObjectCreationParameters gameObjectBindInfo,
#if !NET_4_6
            ModestTree.Util.
#endif
            Action<DiContainer, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> installerMethod)
            : base(container, gameObjectBindInfo)
        {
            _installerMethod = installerMethod;
        }

        protected override void AddInstallers(List<TypeValuePair> args, GameObjectContext context)
        {
            Assert.IsEqual(args.Count, 5);
            Assert.That(args[0].Type.DerivesFromOrEqual<TParam1>());
            Assert.That(args[1].Type.DerivesFromOrEqual<TParam2>());
            Assert.That(args[2].Type.DerivesFromOrEqual<TParam3>());
            Assert.That(args[3].Type.DerivesFromOrEqual<TParam4>());
            Assert.That(args[4].Type.DerivesFromOrEqual<TParam5>());
            Assert.That(args[5].Type.DerivesFromOrEqual<TParam6>());

            context.AddNormalInstaller(
                new ActionInstaller(subContainer =>
                    {
                        _installerMethod(subContainer,
                            (TParam1)args[0].Value,
                            (TParam2)args[1].Value,
                            (TParam3)args[2].Value,
                            (TParam4)args[3].Value,
                            (TParam5)args[4].Value,
                            (TParam6)args[5].Value);
                    }));
        }
    }

    [NoReflectionBaking]
    public class SubContainerCreatorByNewGameObjectMethod<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10> : SubContainerCreatorByNewGameObjectDynamicContext
    {
        readonly
#if !NET_4_6
            ModestTree.Util.
#endif
            Action<DiContainer, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10> _installerMethod;

        public SubContainerCreatorByNewGameObjectMethod(
            DiContainer container,
            GameObjectCreationParameters gameObjectBindInfo,
#if !NET_4_6
            ModestTree.Util.
#endif
            Action<DiContainer, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10> installerMethod)
            : base(container, gameObjectBindInfo)
        {
            _installerMethod = installerMethod;
        }

        protected override void AddInstallers(List<TypeValuePair> args, GameObjectContext context)
        {
            Assert.IsEqual(args.Count, 10);

            Assert.That(args[0].Type.DerivesFromOrEqual<TParam1>());
            Assert.That(args[1].Type.DerivesFromOrEqual<TParam2>());
            Assert.That(args[2].Type.DerivesFromOrEqual<TParam3>());
            Assert.That(args[3].Type.DerivesFromOrEqual<TParam4>());
            Assert.That(args[4].Type.DerivesFromOrEqual<TParam5>());
            Assert.That(args[5].Type.DerivesFromOrEqual<TParam6>());
            Assert.That(args[6].Type.DerivesFromOrEqual<TParam7>());
            Assert.That(args[7].Type.DerivesFromOrEqual<TParam8>());
            Assert.That(args[8].Type.DerivesFromOrEqual<TParam9>());
            Assert.That(args[9].Type.DerivesFromOrEqual<TParam10>());

            context.AddNormalInstaller(
                new ActionInstaller(subContainer =>
                    {
                        _installerMethod(subContainer,
                            (TParam1)args[0].Value,
                            (TParam2)args[1].Value,
                            (TParam3)args[2].Value,
                            (TParam4)args[3].Value,
                            (TParam5)args[4].Value,
                            (TParam6)args[5].Value,
                            (TParam7)args[6].Value,
                            (TParam8)args[7].Value,
                            (TParam9)args[8].Value,
                            (TParam10)args[9].Value);
                    }));
        }
    }
}

#endif
