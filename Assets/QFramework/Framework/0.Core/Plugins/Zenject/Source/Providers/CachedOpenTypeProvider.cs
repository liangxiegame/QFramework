using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;

namespace Zenject
{
    [NoReflectionBaking]
    public class CachedOpenTypeProvider : IProvider
    {
        readonly IProvider _creator;
        readonly Dictionary<Type, CachedProvider> _providerMap = new Dictionary<Type, CachedProvider>();

#if ZEN_MULTITHREADING
        readonly object _locker = new object();
#endif

        public CachedOpenTypeProvider(IProvider creator)
        {
            Assert.That(creator.TypeVariesBasedOnMemberType);
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
                    return _providerMap.Values.Select(x => x.NumInstances).Sum();
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
                _providerMap.Clear();
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

            CachedProvider provider;

#if ZEN_MULTITHREADING
            lock (_locker)
#endif
            {
                if (!_providerMap.TryGetValue(context.MemberType, out provider))
                {
                    provider = new CachedProvider(_creator);
                    _providerMap.Add(context.MemberType, provider);
                }
            }

            provider.GetAllInstancesWithInjectSplit(
                context, args, out injectAction, buffer);
        }
    }
}

