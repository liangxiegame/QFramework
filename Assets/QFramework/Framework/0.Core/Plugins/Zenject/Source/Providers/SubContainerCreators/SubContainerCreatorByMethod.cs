using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    // Zero parameters

    [NoReflectionBaking]
    public abstract class SubContainerCreatorByMethodBase : ISubContainerCreator
    {
        readonly DiContainer _container;
        readonly SubContainerCreatorBindInfo _containerBindInfo;

        public SubContainerCreatorByMethodBase(
            DiContainer container, SubContainerCreatorBindInfo containerBindInfo)
        {
            _container = container;
            _containerBindInfo = containerBindInfo;
        }

        public abstract DiContainer CreateSubContainer(
            List<TypeValuePair> args, InjectContext context);

        protected DiContainer CreateEmptySubContainer()
        {
            var subContainer = _container.CreateSubContainer();
            SubContainerCreatorUtil.ApplyBindSettings(_containerBindInfo, subContainer);
            return subContainer;
        }
    }

    [NoReflectionBaking]
    public class SubContainerCreatorByMethod : SubContainerCreatorByMethodBase
    {
        readonly Action<DiContainer> _installMethod;

        public SubContainerCreatorByMethod(
            DiContainer container,
            SubContainerCreatorBindInfo containerBindInfo,
            Action<DiContainer> installMethod)
            : base(container, containerBindInfo)
        {
            _installMethod = installMethod;
        }

        public override DiContainer CreateSubContainer(List<TypeValuePair> args, InjectContext context)
        {
            Assert.IsEmpty(args);

            var subContainer = CreateEmptySubContainer();

            _installMethod(subContainer);

            subContainer.ResolveRoots();

            return subContainer;
        }
    }

    // One parameters

    [NoReflectionBaking]
    public class SubContainerCreatorByMethod<TParam1> : SubContainerCreatorByMethodBase
    {
        readonly Action<DiContainer, TParam1> _installMethod;

        public SubContainerCreatorByMethod(
            DiContainer container,
            SubContainerCreatorBindInfo containerBindInfo,
            Action<DiContainer, TParam1> installMethod)
            : base(container, containerBindInfo)
        {
            _installMethod = installMethod;
        }

        public override DiContainer CreateSubContainer(List<TypeValuePair> args, InjectContext context)
        {
            Assert.IsEqual(args.Count, 1);
            Assert.That(args[0].Type.DerivesFromOrEqual<TParam1>());

            var subContainer = CreateEmptySubContainer();

            _installMethod(subContainer, (TParam1)args[0].Value);

            subContainer.ResolveRoots();

            return subContainer;
        }
    }

    // Two parameters

    [NoReflectionBaking]
    public class SubContainerCreatorByMethod<TParam1, TParam2> : SubContainerCreatorByMethodBase
    {
        readonly Action<DiContainer, TParam1, TParam2> _installMethod;

        public SubContainerCreatorByMethod(
            DiContainer container,
            SubContainerCreatorBindInfo containerBindInfo,
            Action<DiContainer, TParam1, TParam2> installMethod)
            : base(container, containerBindInfo)
        {
            _installMethod = installMethod;
        }

        public override DiContainer CreateSubContainer(List<TypeValuePair> args, InjectContext context)
        {
            Assert.IsEqual(args.Count, 2);
            Assert.That(args[0].Type.DerivesFromOrEqual<TParam1>());
            Assert.That(args[1].Type.DerivesFromOrEqual<TParam2>());

            var subContainer = CreateEmptySubContainer();

            _installMethod(
                subContainer,
                (TParam1)args[0].Value,
                (TParam2)args[1].Value);

            subContainer.ResolveRoots();

            return subContainer;
        }
    }

    // Three parameters

    [NoReflectionBaking]
    public class SubContainerCreatorByMethod<TParam1, TParam2, TParam3> : SubContainerCreatorByMethodBase
    {
        readonly Action<DiContainer, TParam1, TParam2, TParam3> _installMethod;

        public SubContainerCreatorByMethod(
            DiContainer container,
            SubContainerCreatorBindInfo containerBindInfo,
            Action<DiContainer, TParam1, TParam2, TParam3> installMethod)
            : base(container, containerBindInfo)
        {
            _installMethod = installMethod;
        }

        public override DiContainer CreateSubContainer(List<TypeValuePair> args, InjectContext context)
        {
            Assert.IsEqual(args.Count, 3);
            Assert.That(args[0].Type.DerivesFromOrEqual<TParam1>());
            Assert.That(args[1].Type.DerivesFromOrEqual<TParam2>());
            Assert.That(args[2].Type.DerivesFromOrEqual<TParam3>());

            var subContainer = CreateEmptySubContainer();

            _installMethod(
                subContainer,
                (TParam1)args[0].Value,
                (TParam2)args[1].Value,
                (TParam3)args[2].Value);

            subContainer.ResolveRoots();

            return subContainer;
        }
    }

    // Four parameters

    [NoReflectionBaking]
    public class SubContainerCreatorByMethod<TParam1, TParam2, TParam3, TParam4> : SubContainerCreatorByMethodBase
    {
        readonly
#if !NET_4_6
            ModestTree.Util.
#endif
            Action<DiContainer, TParam1, TParam2, TParam3, TParam4> _installMethod;

        public SubContainerCreatorByMethod(
            DiContainer container,
            SubContainerCreatorBindInfo containerBindInfo,
#if !NET_4_6
            ModestTree.Util.
#endif
            Action<DiContainer, TParam1, TParam2, TParam3, TParam4> installMethod)
            : base(container, containerBindInfo)
        {
            _installMethod = installMethod;
        }

        public override DiContainer CreateSubContainer(List<TypeValuePair> args, InjectContext context)
        {
            Assert.IsEqual(args.Count, 4);
            Assert.That(args[0].Type.DerivesFromOrEqual<TParam1>());
            Assert.That(args[1].Type.DerivesFromOrEqual<TParam2>());
            Assert.That(args[2].Type.DerivesFromOrEqual<TParam3>());
            Assert.That(args[3].Type.DerivesFromOrEqual<TParam4>());

            var subContainer = CreateEmptySubContainer();

            _installMethod(
                subContainer,
                (TParam1)args[0].Value,
                (TParam2)args[1].Value,
                (TParam3)args[2].Value,
                (TParam4)args[3].Value);

            subContainer.ResolveRoots();

            return subContainer;
        }
    }

    // Five parameters

    [NoReflectionBaking]
    public class SubContainerCreatorByMethod<TParam1, TParam2, TParam3, TParam4, TParam5> : SubContainerCreatorByMethodBase
    {
        readonly
#if !NET_4_6
            ModestTree.Util.
#endif
            Action<DiContainer, TParam1, TParam2, TParam3, TParam4, TParam5> _installMethod;

        public SubContainerCreatorByMethod(
            DiContainer container,
            SubContainerCreatorBindInfo containerBindInfo,
#if !NET_4_6
            ModestTree.Util.
#endif
            Action<DiContainer, TParam1, TParam2, TParam3, TParam4, TParam5> installMethod)
            : base(container, containerBindInfo)
        {
            _installMethod = installMethod;
        }

        public override DiContainer CreateSubContainer(List<TypeValuePair> args, InjectContext context)
        {
            Assert.IsEqual(args.Count, 5);
            Assert.That(args[0].Type.DerivesFromOrEqual<TParam1>());
            Assert.That(args[1].Type.DerivesFromOrEqual<TParam2>());
            Assert.That(args[2].Type.DerivesFromOrEqual<TParam3>());
            Assert.That(args[3].Type.DerivesFromOrEqual<TParam4>());
            Assert.That(args[4].Type.DerivesFromOrEqual<TParam5>());

            var subContainer = CreateEmptySubContainer();

            _installMethod(
                subContainer,
                (TParam1)args[0].Value,
                (TParam2)args[1].Value,
                (TParam3)args[2].Value,
                (TParam4)args[3].Value,
                (TParam5)args[4].Value);

            subContainer.ResolveRoots();

            return subContainer;
        }
    }

    // Six parameters

    [NoReflectionBaking]
    public class SubContainerCreatorByMethod<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> : SubContainerCreatorByMethodBase
    {
        readonly
#if !NET_4_6
            ModestTree.Util.
#endif
            Action<DiContainer, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> _installMethod;

        public SubContainerCreatorByMethod(
            DiContainer container,
            SubContainerCreatorBindInfo containerBindInfo,
#if !NET_4_6
            ModestTree.Util.
#endif
            Action<DiContainer, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> installMethod)
            : base(container, containerBindInfo)
        {
            _installMethod = installMethod;
        }

        public override DiContainer CreateSubContainer(List<TypeValuePair> args, InjectContext context)
        {
            Assert.IsEqual(args.Count, 5);
            Assert.That(args[0].Type.DerivesFromOrEqual<TParam1>());
            Assert.That(args[1].Type.DerivesFromOrEqual<TParam2>());
            Assert.That(args[2].Type.DerivesFromOrEqual<TParam3>());
            Assert.That(args[3].Type.DerivesFromOrEqual<TParam4>());
            Assert.That(args[4].Type.DerivesFromOrEqual<TParam5>());
            Assert.That(args[5].Type.DerivesFromOrEqual<TParam6>());

            var subContainer = CreateEmptySubContainer();

            _installMethod(
                subContainer,
                (TParam1)args[0].Value,
                (TParam2)args[1].Value,
                (TParam3)args[2].Value,
                (TParam4)args[3].Value,
                (TParam5)args[4].Value,
                (TParam6)args[5].Value);

            subContainer.ResolveRoots();

            return subContainer;
        }
    }

    // 10 parameters

    [NoReflectionBaking]
    public class SubContainerCreatorByMethod<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10> : SubContainerCreatorByMethodBase
    {
        readonly
#if !NET_4_6
            ModestTree.Util.
#endif
            Action<DiContainer, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10> _installMethod;

        public SubContainerCreatorByMethod(
            DiContainer container,
            SubContainerCreatorBindInfo containerBindInfo,
#if !NET_4_6
            ModestTree.Util.
#endif
            Action<DiContainer, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10> installMethod)
            : base(container, containerBindInfo)
        {
            _installMethod = installMethod;
        }

        public override DiContainer CreateSubContainer(List<TypeValuePair> args, InjectContext context)
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

            var subContainer = CreateEmptySubContainer();

            _installMethod(
                subContainer,
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

            subContainer.ResolveRoots();

            return subContainer;
        }
    }
}
