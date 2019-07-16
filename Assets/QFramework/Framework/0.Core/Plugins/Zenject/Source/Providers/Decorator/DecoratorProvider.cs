using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject.Internal
{
    public interface IDecoratorProvider
    {
        void GetAllInstances(
            IProvider provider, InjectContext context, List<object> buffer);
    }

    [NoReflectionBaking]
    public class DecoratorProvider<TContract> : IDecoratorProvider
    {
        readonly Dictionary<IProvider, List<object>> _cachedInstances =
            new Dictionary<IProvider, List<object>>();

        readonly DiContainer _container;
        readonly List<Guid> _factoryBindIds = new List<Guid>();

        List<IFactory<TContract, TContract>> _decoratorFactories;

#if ZEN_MULTITHREADING
        readonly object _locker = new object();
#endif

        public DecoratorProvider(DiContainer container)
        {
            _container = container;
        }

        public void AddFactoryId(Guid factoryBindId)
        {
            _factoryBindIds.Add(factoryBindId);
        }

        void LazyInitializeDecoratorFactories()
        {
            if (_decoratorFactories == null)
            {
                _decoratorFactories = new List<IFactory<TContract, TContract>>();

                for (int i = 0; i < _factoryBindIds.Count; i++)
                {
                    var bindId = _factoryBindIds[i];
                    var factory = _container.ResolveId<IFactory<TContract, TContract>>(bindId);
                    _decoratorFactories.Add(factory);
                }
            }
        }

        public void GetAllInstances(
            IProvider provider, InjectContext context, List<object> buffer)
        {
            if (provider.IsCached)
            {
                List<object> instances;

#if ZEN_MULTITHREADING
                lock (_locker)
#endif
                {
                    if (!_cachedInstances.TryGetValue(provider, out instances))
                    {
                        instances = new List<object>();
                        WrapProviderInstances(provider, context, instances);
                        _cachedInstances.Add(provider, instances);
                    }
                }

                buffer.AllocFreeAddRange(instances);
            }
            else
            {
                WrapProviderInstances(provider, context, buffer);
            }
        }

        void WrapProviderInstances(IProvider provider, InjectContext context, List<object> buffer)
        {
            LazyInitializeDecoratorFactories();

            provider.GetAllInstances(context, buffer);

            for (int i = 0; i < buffer.Count; i++)
            {
                buffer[i] = DecorateInstance(buffer[i], context);
            }
        }

        object DecorateInstance(object instance, InjectContext context)
        {
            for (int i = 0; i < _decoratorFactories.Count; i++)
            {
                instance = _decoratorFactories[i].Create(
                    context.Container.IsValidating ? default(TContract) : (TContract)instance);
            }

            return instance;
        }
    }
}
