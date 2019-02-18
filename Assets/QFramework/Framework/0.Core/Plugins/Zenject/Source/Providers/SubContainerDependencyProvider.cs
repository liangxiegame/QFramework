using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    [NoReflectionBaking]
    public class SubContainerDependencyProvider : IProvider
    {
        readonly ISubContainerCreator _subContainerCreator;
        readonly Type _dependencyType;
        readonly object _identifier;
        readonly bool _resolveAll;

        // if concreteType is null we use the contract type from inject context
        public SubContainerDependencyProvider(
            Type dependencyType,
            object identifier,
            ISubContainerCreator subContainerCreator, bool resolveAll)
        {
            _subContainerCreator = subContainerCreator;
            _dependencyType = dependencyType;
            _identifier = identifier;
            _resolveAll = resolveAll;
        }

        public bool IsCached
        {
            get { return false; }
        }

        public bool TypeVariesBasedOnMemberType
        {
            get { return false; }
        }

        public Type GetInstanceType(InjectContext context)
        {
            return _dependencyType;
        }

        InjectContext CreateSubContext(
            InjectContext parent, DiContainer subContainer)
        {
            var subContext = parent.CreateSubContext(_dependencyType, _identifier);

            subContext.Container = subContainer;

            // This is important to avoid infinite loops
            subContext.SourceType = InjectSources.Local;

            return subContext;
        }

        public void GetAllInstancesWithInjectSplit(
            InjectContext context, List<TypeValuePair> args, out Action injectAction, List<object> buffer)
        {
            Assert.IsNotNull(context);

            var subContainer = _subContainerCreator.CreateSubContainer(args, context);

            var subContext = CreateSubContext(context, subContainer);

            injectAction = null;

            if (_resolveAll)
            {
                subContainer.ResolveAll(subContext, buffer);
                return;
            }

            buffer.Add(subContainer.Resolve(subContext));
        }
    }
}
