using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    [NoReflectionBaking]
    public class CachedProvider : IProvider
    {
        readonly IProvider _creator;

        List<object> _instances;

#if ZEN_MULTITHREADING
        readonly object _locker = new object();
#else
        bool _isCreatingInstance;
#endif

        public CachedProvider(IProvider creator)
        {
            _creator = creator;
        }

        public bool IsCached
        {
            get { return true; }
        }

        public bool TypeVariesBasedOnMemberType
        {
            get
            {
                // Should not call this
                throw Assert.CreateException();
            }
        }

        public int NumInstances
        {
            get
            {
#if ZEN_MULTITHREADING
                lock (_locker)
#endif
                {
                    return _instances == null ? 0 : _instances.Count;
                }
            }
        }

        // This method can be called if you want to clear the memory for an AsSingle instance,
        // See isssue https://github.com/svermeulen/Zenject/issues/441
        public void ClearCache()
        {
#if ZEN_MULTITHREADING
            lock (_locker)
#endif
            {
                _instances = null;
            }
        }

        public Type GetInstanceType(InjectContext context)
        {
            return _creator.GetInstanceType(context);
        }

        public void GetAllInstancesWithInjectSplit(
            InjectContext context, List<TypeValuePair> args, out Action injectAction, List<object> buffer)
        {
            Assert.IsNotNull(context);

#if ZEN_MULTITHREADING
            lock (_locker)
#endif
            {
                if (_instances != null)
                {
                    injectAction = null;
                    buffer.AllocFreeAddRange(_instances);
                    return;
                }

#if !ZEN_MULTITHREADING
                // This should only happen with constructor injection
                // Field or property injection should allow circular dependencies
                if (_isCreatingInstance)
                {
                    var instanceType = _creator.GetInstanceType(context);
                    throw Assert.CreateException(
                        "Found circular dependency when creating type '{0}'. Object graph:\n {1}{2}\n",
                        instanceType, context.GetObjectGraphString(), instanceType);
                }

                _isCreatingInstance = true;
#endif

                var instances = new List<object>();
                _creator.GetAllInstancesWithInjectSplit(context, args, out injectAction, instances);
                Assert.IsNotNull(instances);

                _instances = instances;
#if !ZEN_MULTITHREADING
                _isCreatingInstance = false;
#endif
                buffer.AllocFreeAddRange(instances);
            }
        }
    }
}
