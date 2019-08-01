using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    [NoReflectionBaking]
    public class SubContainerCreatorByInstance : ISubContainerCreator
    {
        readonly DiContainer _subcontainer;

        public SubContainerCreatorByInstance(DiContainer subcontainer)
        {
            _subcontainer = subcontainer;
        }

        public DiContainer CreateSubContainer(List<TypeValuePair> args, InjectContext context)
        {
            Assert.That(args.IsEmpty());
            // It is assumed here that the subcontainer has already had ResolveRoots called elsewhere
            // Since most likely you are adding a subcontainer that is already in a context or
            // something rather than directly using DiContainer.CreateSubContainer
            return _subcontainer;
        }
    }
}

