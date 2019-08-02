using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using Zenject.Internal;

namespace Zenject
{
    [NoReflectionBaking]
    public class SubContainerCreatorByInstaller : ISubContainerCreator
    {
        readonly Type _installerType;
        readonly DiContainer _container;
        readonly List<TypeValuePair> _extraArgs;
        readonly SubContainerCreatorBindInfo _containerBindInfo;

        public SubContainerCreatorByInstaller(
            DiContainer container,
            SubContainerCreatorBindInfo containerBindInfo,
            Type installerType,
            IEnumerable<TypeValuePair> extraArgs)
        {
            _installerType = installerType;
            _container = container;
            _extraArgs = extraArgs.ToList();
            _containerBindInfo = containerBindInfo;

            Assert.That(installerType.DerivesFrom<InstallerBase>(),
                "Invalid installer type given during bind command.  Expected type '{0}' to derive from 'Installer<>'", installerType);
        }

        public SubContainerCreatorByInstaller(
            DiContainer container,
            SubContainerCreatorBindInfo containerBindInfo,
            Type installerType)
            : this(container, containerBindInfo, installerType, new List<TypeValuePair>())
        {
        }

        public DiContainer CreateSubContainer(List<TypeValuePair> args, InjectContext context)
        {
            var subContainer = _container.CreateSubContainer();

            SubContainerCreatorUtil.ApplyBindSettings(_containerBindInfo, subContainer);

            var extraArgs = ZenPools.SpawnList<TypeValuePair>();

            extraArgs.AllocFreeAddRange(_extraArgs);
            extraArgs.AllocFreeAddRange(args);

            var installer = (InstallerBase)subContainer.InstantiateExplicit(
                _installerType, extraArgs);

            ZenPools.DespawnList(extraArgs);

            installer.InstallBindings();

            subContainer.ResolveRoots();

            return subContainer;
        }
    }
}
