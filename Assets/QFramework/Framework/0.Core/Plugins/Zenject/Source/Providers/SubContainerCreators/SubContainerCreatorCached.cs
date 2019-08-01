using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    [NoReflectionBaking]
    public class SubContainerCreatorCached : ISubContainerCreator
    {
        readonly ISubContainerCreator _subCreator;

#if ZEN_MULTITHREADING
        readonly object _locker = new object();
#else
        bool _isLookingUp;
#endif
        DiContainer _subContainer;

        public SubContainerCreatorCached(ISubContainerCreator subCreator)
        {
            _subCreator = subCreator;
        }

        public DiContainer CreateSubContainer(List<TypeValuePair> args, InjectContext context)
        {
            // We can't really support arguments if we are using the cached value since
            // the arguments might change when called after the first time
            Assert.IsEmpty(args);

#if ZEN_MULTITHREADING
            lock (_locker)
#endif
            {
                if (_subContainer == null)
                {
#if !ZEN_MULTITHREADING
                    Assert.That(!_isLookingUp,
                        "Found unresolvable circular dependency when looking up sub container!  Object graph:\n {0}", context.GetObjectGraphString());
                    _isLookingUp = true;
#endif

                    _subContainer = _subCreator.CreateSubContainer(new List<TypeValuePair>(), context);

#if !ZEN_MULTITHREADING
                    _isLookingUp = false;
#endif

                    Assert.IsNotNull(_subContainer);
                }

                return _subContainer;
            }
        }
    }
}
