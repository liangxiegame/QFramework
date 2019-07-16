using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    [NoReflectionBaking]
    public class SubContainerBindingFinalizer : ProviderBindingFinalizer
    {
        readonly object _subIdentifier;
        readonly bool _resolveAll;
        readonly Func<DiContainer, ISubContainerCreator> _creatorFactory;

        public SubContainerBindingFinalizer(
            BindInfo bindInfo, object subIdentifier,
            bool resolveAll, Func<DiContainer, ISubContainerCreator> creatorFactory)
            : base(bindInfo)
        {
            _subIdentifier = subIdentifier;
            _resolveAll = resolveAll;
            _creatorFactory = creatorFactory;
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
                            new SubContainerDependencyProvider(
                                concreteType, _subIdentifier, _creatorFactory(container), _resolveAll));
                    break;
                }
                case ScopeTypes.Singleton:
                {
                    var containerCreator = new SubContainerCreatorCached(_creatorFactory(container));

                    RegisterProvidersForAllContractsPerConcreteType(
                        container,
                        concreteTypes,
                        (_, concreteType) =>
                            new SubContainerDependencyProvider(
                                concreteType, _subIdentifier, containerCreator, _resolveAll));
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
                        (_, contractType) => new SubContainerDependencyProvider(
                            contractType, _subIdentifier, _creatorFactory(container), _resolveAll));
                    break;
                }
                case ScopeTypes.Singleton:
                {
                    var containerCreator = new SubContainerCreatorCached(_creatorFactory(container));

                    RegisterProviderPerContract(
                        container,
                        (_, contractType) =>
                            new SubContainerDependencyProvider(
                                contractType, _subIdentifier, containerCreator, _resolveAll));
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


