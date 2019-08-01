using System;
using System.Collections.Generic;
using System.Linq;
#if !NOT_UNITY3D
using UnityEngine;
#endif
using ModestTree;

namespace Zenject
{
    [NoReflectionBaking]
    public class FactoryFromBinder<TContract> : FactoryFromBinderBase
    {
        public FactoryFromBinder(
            DiContainer container, BindInfo bindInfo, FactoryBindInfo factoryBindInfo)
            : base(container, typeof(TContract), bindInfo, factoryBindInfo)
        {
        }

        public ConditionCopyNonLazyBinder FromResolveGetter<TObj>(Func<TObj, TContract> method)
        {
            return FromResolveGetter<TObj>(null, method);
        }

        public ConditionCopyNonLazyBinder FromResolveGetter<TObj>(
            object subIdentifier, Func<TObj, TContract> method)
        {
            return FromResolveGetter<TObj>(subIdentifier, method, InjectSources.Any);
        }

        public ConditionCopyNonLazyBinder FromResolveGetter<TObj>(
            object subIdentifier, Func<TObj, TContract> method, InjectSources source)
        {
            FactoryBindInfo.ProviderFunc =
                (container) => new GetterProvider<TObj, TContract>(subIdentifier, method, container, source, false);

            return this;
        }

        public ConditionCopyNonLazyBinder FromMethod(Func<DiContainer, TContract> method)
        {
            ProviderFunc =
                (container) => new MethodProviderWithContainer<TContract>(method);

            return this;
        }

        // Shortcut for FromIFactory and also for backwards compatibility
        public ArgConditionCopyNonLazyBinder FromFactory<TSubFactory>()
            where TSubFactory : IFactory<TContract>
        {
            return this.FromIFactory(x => x.To<TSubFactory>().AsCached());
        }

        public FactorySubContainerBinder<TContract> FromSubContainerResolve()
        {
            return FromSubContainerResolve(null);
        }

        public FactorySubContainerBinder<TContract> FromSubContainerResolve(object subIdentifier)
        {
            return new FactorySubContainerBinder<TContract>(
                BindContainer, BindInfo, FactoryBindInfo, subIdentifier);
        }

#if !NOT_UNITY3D

        public ConditionCopyNonLazyBinder FromComponentInHierarchy(
            bool includeInactive = true)
        {
            BindingUtil.AssertIsInterfaceOrComponent(ContractType);

            return FromMethod(_ =>
                {
                    var res = BindContainer.Resolve<Context>().GetRootGameObjects()
                        .Select(x => x.GetComponentInChildren<TContract>(includeInactive))
                        .Where(x => x != null).FirstOrDefault();

                    Assert.IsNotNull(res,
                        "Could not find component '{0}' through FromComponentInHierarchy factory binding", typeof(TContract));

                    return res;
                });
        }
#endif
    }

    // These methods have to be extension methods for the UWP build (with .NET backend) to work correctly
    // When these are instance methods it takes a really long time then fails with StackOverflowException
    public static class FactoryFromBinder0Extensions
    {
        public static ArgConditionCopyNonLazyBinder FromPoolableMemoryPool<TContract, TMemoryPool>(
            this FactoryFromBinder<TContract> fromBinder,
            Action<MemoryPoolInitialSizeMaxSizeBinder<TContract>> poolBindGenerator)
            // Unfortunately we have to pass the same contract in again to satisfy the generic
            // constraints below
            where TContract : IPoolable<IMemoryPool>
            where TMemoryPool : MemoryPool<IMemoryPool, TContract>
        {
            // Use a random ID so that our provider is the only one that can find it and so it doesn't
            // conflict with anything else
            var poolId = Guid.NewGuid();

            // Important to use NoFlush otherwise the binding will be finalized early
            var binder = fromBinder.BindContainer.BindMemoryPoolCustomInterfaceNoFlush<TContract, TMemoryPool, TMemoryPool>().WithId(poolId);

            // Always make it non lazy by default in case the user sets an InitialSize
            binder.NonLazy();

            poolBindGenerator(binder);

            fromBinder.ProviderFunc =
                (container) => { return new PoolableMemoryPoolProvider<TContract, TMemoryPool>(container, poolId); };

            return new ArgConditionCopyNonLazyBinder(fromBinder.BindInfo);
        }

        public static ArgConditionCopyNonLazyBinder FromPoolableMemoryPool<TContract>(
            this FactoryFromBinder<TContract> fromBinder)
            // Unfortunately we have to pass the same contract in again to satisfy the generic
            // constraints below
            where TContract : IPoolable<IMemoryPool>
        {
            return fromBinder.FromPoolableMemoryPool<TContract>(x => {});
        }

        public static ArgConditionCopyNonLazyBinder FromPoolableMemoryPool<TContract>(
            this FactoryFromBinder<TContract> fromBinder,
            Action<MemoryPoolInitialSizeMaxSizeBinder<TContract>> poolBindGenerator)
            // Unfortunately we have to pass the same contract in again to satisfy the generic
            // constraints below
            where TContract : IPoolable<IMemoryPool>
        {
            return fromBinder.FromPoolableMemoryPool<TContract, PoolableMemoryPool<IMemoryPool, TContract>>(poolBindGenerator);
        }

#if !NOT_UNITY3D
        public static ArgConditionCopyNonLazyBinder FromMonoPoolableMemoryPool<TContract>(
            this FactoryFromBinder<TContract> fromBinder)
            // Unfortunately we have to pass the same contract in again to satisfy the generic
            // constraints below
            where TContract : Component, IPoolable<IMemoryPool>
        {
            return fromBinder.FromMonoPoolableMemoryPool<TContract>(x => {});
        }

        public static ArgConditionCopyNonLazyBinder FromMonoPoolableMemoryPool<TContract>(
            this FactoryFromBinder<TContract> fromBinder,
            Action<MemoryPoolInitialSizeMaxSizeBinder<TContract>> poolBindGenerator)
            // Unfortunately we have to pass the same contract in again to satisfy the generic
            // constraints below
            where TContract : Component, IPoolable<IMemoryPool>
        {
            return fromBinder.FromPoolableMemoryPool<TContract, MonoPoolableMemoryPool<IMemoryPool, TContract>>(poolBindGenerator);
        }
#endif

        public static ArgConditionCopyNonLazyBinder FromPoolableMemoryPool<TContract, TMemoryPool>(
            this FactoryFromBinder<TContract> fromBinder)
            // Unfortunately we have to pass the same contract in again to satisfy the generic
            // constraints below
            where TContract : IPoolable<IMemoryPool>
            where TMemoryPool : MemoryPool<IMemoryPool, TContract>
        {
            return fromBinder.FromPoolableMemoryPool<TContract, TMemoryPool>(x => {});
        }

        public static ArgConditionCopyNonLazyBinder FromIFactory<TContract>(
            this FactoryFromBinder<TContract> fromBinder,
            Action<ConcreteBinderGeneric<IFactory<TContract>>> factoryBindGenerator)
        {
            Guid factoryId;
            factoryBindGenerator(
                fromBinder.CreateIFactoryBinder<IFactory<TContract>>(out factoryId));

            fromBinder.ProviderFunc =
                (container) => { return new IFactoryProvider<TContract>(container, factoryId); };

            return new ArgConditionCopyNonLazyBinder(fromBinder.BindInfo);
        }
    }
}
