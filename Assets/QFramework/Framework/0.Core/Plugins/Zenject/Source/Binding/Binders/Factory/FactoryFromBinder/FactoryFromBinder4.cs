using System;
using System.Collections.Generic;
#if !NOT_UNITY3D
using UnityEngine;
#endif
using ModestTree;

namespace Zenject
{
    [NoReflectionBaking]
    public class FactoryFromBinder<TParam1, TParam2, TParam3, TParam4, TContract> : FactoryFromBinderBase
    {
        public FactoryFromBinder(
            DiContainer container, BindInfo bindInfo, FactoryBindInfo factoryBindInfo)
            : base(container, typeof(TContract), bindInfo, factoryBindInfo)
        {
        }

        public ConditionCopyNonLazyBinder FromMethod(
#if !NET_4_6
            ModestTree.Util.
#endif
            Func<DiContainer, TParam1, TParam2, TParam3, TParam4, TContract> method)
        {
            ProviderFunc =
                (container) => new MethodProviderWithContainer<TParam1, TParam2, TParam3, TParam4, TContract>(method);

            return this;
        }

        // Shortcut for FromIFactory and also for backwards compatibility
        public ConditionCopyNonLazyBinder FromFactory<TSubFactory>()
            where TSubFactory : IFactory<TParam1, TParam2, TParam3, TParam4, TContract>
        {
            return this.FromIFactory(x => x.To<TSubFactory>().AsCached());
        }

        public FactorySubContainerBinder<TParam1, TParam2, TParam3, TParam4, TContract> FromSubContainerResolve()
        {
            return FromSubContainerResolve(null);
        }

        public FactorySubContainerBinder<TParam1, TParam2, TParam3, TParam4, TContract> FromSubContainerResolve(object subIdentifier)
        {
            return new FactorySubContainerBinder<TParam1, TParam2, TParam3, TParam4, TContract>(
                BindContainer, BindInfo, FactoryBindInfo, subIdentifier);
        }
    }

    // These methods have to be extension methods for the UWP build (with .NET backend) to work correctly
    // When these are instance methods it takes a really long time then fails with StackOverflowException
    public static class FactoryFromBinder4Extensions
    {
        public static ArgConditionCopyNonLazyBinder FromIFactory<TParam1, TParam2, TParam3, TParam4, TContract>(
            this FactoryFromBinder<TParam1, TParam2, TParam3, TParam4, TContract> fromBinder,
            Action<ConcreteBinderGeneric<IFactory<TParam1, TParam2, TParam3, TParam4, TContract>>> factoryBindGenerator)
        {
            Guid factoryId;
            factoryBindGenerator(
                fromBinder.CreateIFactoryBinder<IFactory<TParam1, TParam2, TParam3, TParam4, TContract>>(out factoryId));

            fromBinder.ProviderFunc =
                (container) => { return new IFactoryProvider<TParam1, TParam2, TParam3, TParam4, TContract>(container, factoryId); };

            return new ArgConditionCopyNonLazyBinder(fromBinder.BindInfo);
        }

        public static ArgConditionCopyNonLazyBinder FromPoolableMemoryPool<TParam1, TParam2, TParam3, TParam4, TContract>(
            this FactoryFromBinder<TParam1, TParam2, TParam3, TParam4, TContract> fromBinder)
            // Unfortunately we have to pass the same contract in again to satisfy the generic
            // constraints below
            where TContract : IPoolable<TParam1, TParam2, TParam3, TParam4, IMemoryPool>
        {
            return fromBinder.FromPoolableMemoryPool<TParam1, TParam2, TParam3, TParam4, TContract>(x => {});
        }

        public static ArgConditionCopyNonLazyBinder FromPoolableMemoryPool<TParam1, TParam2, TParam3, TParam4, TContract>(
            this FactoryFromBinder<TParam1, TParam2, TParam3, TParam4, TContract> fromBinder,
            Action<MemoryPoolInitialSizeMaxSizeBinder<TContract>> poolBindGenerator)
            // Unfortunately we have to pass the same contract in again to satisfy the generic
            // constraints below
            where TContract : IPoolable<TParam1, TParam2, TParam3, TParam4, IMemoryPool>
        {
            return fromBinder.FromPoolableMemoryPool<TParam1, TParam2, TParam3, TParam4, TContract, PoolableMemoryPool<TParam1, TParam2, TParam3, TParam4, IMemoryPool, TContract>>(poolBindGenerator);
        }

#if !NOT_UNITY3D
        public static ArgConditionCopyNonLazyBinder FromMonoPoolableMemoryPool<TParam1, TParam2, TParam3, TParam4, TContract>(
            this FactoryFromBinder<TParam1, TParam2, TParam3, TParam4, TContract> fromBinder)
            // Unfortunately we have to pass the same contract in again to satisfy the generic
            // constraints below
            where TContract : Component, IPoolable<TParam1, TParam2, TParam3, TParam4, IMemoryPool>
        {
            return fromBinder.FromMonoPoolableMemoryPool<TParam1, TParam2, TParam3, TParam4, TContract>(x => {});
        }

        public static ArgConditionCopyNonLazyBinder FromMonoPoolableMemoryPool<TParam1, TParam2, TParam3, TParam4, TContract>(
            this FactoryFromBinder<TParam1, TParam2, TParam3, TParam4, TContract> fromBinder,
            Action<MemoryPoolInitialSizeMaxSizeBinder<TContract>> poolBindGenerator)
            // Unfortunately we have to pass the same contract in again to satisfy the generic
            // constraints below
            where TContract : Component, IPoolable<TParam1, TParam2, TParam3, TParam4, IMemoryPool>
        {
            return fromBinder.FromPoolableMemoryPool<TParam1, TParam2, TParam3, TParam4, TContract, MonoPoolableMemoryPool<TParam1, TParam2, TParam3, TParam4, IMemoryPool, TContract>>(poolBindGenerator);
        }
#endif

        public static ArgConditionCopyNonLazyBinder FromPoolableMemoryPool<TParam1, TParam2, TParam3, TParam4, TContract, TMemoryPool>(
            this FactoryFromBinder<TParam1, TParam2, TParam3, TParam4, TContract> fromBinder)
            // Unfortunately we have to pass the same contract in again to satisfy the generic
            // constraints below
            where TContract : IPoolable<TParam1, TParam2, TParam3, TParam4, IMemoryPool>
            where TMemoryPool : MemoryPool<TParam1, TParam2, TParam3, TParam4, IMemoryPool, TContract>
        {
            return fromBinder.FromPoolableMemoryPool<TParam1, TParam2, TParam3, TParam4, TContract, TMemoryPool>(x => {});
        }

        public static ArgConditionCopyNonLazyBinder FromPoolableMemoryPool<TParam1, TParam2, TParam3, TParam4, TContract, TMemoryPool>(
            this FactoryFromBinder<TParam1, TParam2, TParam3, TParam4, TContract> fromBinder,
            Action<MemoryPoolInitialSizeMaxSizeBinder<TContract>> poolBindGenerator)
            // Unfortunately we have to pass the same contract in again to satisfy the generic
            // constraints below
            where TContract : IPoolable<TParam1, TParam2, TParam3, TParam4, IMemoryPool>
            where TMemoryPool : MemoryPool<TParam1, TParam2, TParam3, TParam4, IMemoryPool, TContract>
        {
            // Use a random ID so that our provider is the only one that can find it and so it doesn't
            // conflict with anything else
            var poolId = Guid.NewGuid();

            // Important to use NoFlush otherwise the binding will be finalized early
            var binder = fromBinder.BindContainer.BindMemoryPoolCustomInterfaceNoFlush<TContract, TMemoryPool, TMemoryPool>()
                .WithId(poolId);

            // Always make it non lazy by default in case the user sets an InitialSize
            binder.NonLazy();

            poolBindGenerator(binder);

            fromBinder.ProviderFunc =
                (container) => { return new PoolableMemoryPoolProvider<TParam1, TParam2, TParam3, TParam4, TContract, TMemoryPool>(container, poolId); };

            return new ArgConditionCopyNonLazyBinder(fromBinder.BindInfo);
        }
    }
}
