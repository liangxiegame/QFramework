using System;
using System.Collections.Generic;
using ModestTree;
using System.Linq;

#if !NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject
{
    [NoReflectionBaking]
    public class FromBinderGeneric<TContract> : FromBinder
    {
        public FromBinderGeneric(
            DiContainer bindContainer,
            BindInfo bindInfo,
            BindStatement bindStatement)
            : base(bindContainer, bindInfo, bindStatement)
        {
            BindingUtil.AssertIsDerivedFromTypes(typeof(TContract), BindInfo.ContractTypes);
        }

        // Shortcut for FromIFactory and also for backwards compatibility
        public ScopeConcreteIdArgConditionCopyNonLazyBinder FromFactory<TFactory>()
            where TFactory : IFactory<TContract>
        {
            return FromIFactory(x => x.To<TFactory>().AsCached());
        }

        public ScopeConcreteIdArgConditionCopyNonLazyBinder FromIFactory(
            Action<ConcreteBinderGeneric<IFactory<TContract>>> factoryBindGenerator)
        {
            return FromIFactoryBase<TContract>(factoryBindGenerator);
        }

        public ScopeConcreteIdArgConditionCopyNonLazyBinder FromMethod(Func<TContract> method)
        {
            return FromMethodBase<TContract>(ctx => method());
        }

        public ScopeConcreteIdArgConditionCopyNonLazyBinder FromMethod(Func<InjectContext, TContract> method)
        {
            return FromMethodBase<TContract>(method);
        }

        public ScopeConcreteIdArgConditionCopyNonLazyBinder FromMethodMultiple(Func<InjectContext, IEnumerable<TContract>> method)
        {
            BindingUtil.AssertIsDerivedFromTypes(typeof(TContract), AllParentTypes);
            return FromMethodMultipleBase<TContract>(method);
        }

        public ScopeConcreteIdArgConditionCopyNonLazyBinder FromResolveGetter<TObj>(Func<TObj, TContract> method)
        {
            return FromResolveGetter<TObj>(null, method);
        }

        public ScopeConcreteIdArgConditionCopyNonLazyBinder FromResolveGetter<TObj>(object identifier, Func<TObj, TContract> method)
        {
            return FromResolveGetter<TObj>(identifier, method, InjectSources.Any);
        }

        public ScopeConcreteIdArgConditionCopyNonLazyBinder FromResolveGetter<TObj>(object identifier, Func<TObj, TContract> method, InjectSources source)
        {
            return FromResolveGetterBase<TObj, TContract>(identifier, method, source, false);
        }

        public ScopeConcreteIdArgConditionCopyNonLazyBinder FromResolveAllGetter<TObj>(Func<TObj, TContract> method)
        {
            return FromResolveAllGetter<TObj>(null, method);
        }

        public ScopeConcreteIdArgConditionCopyNonLazyBinder FromResolveAllGetter<TObj>(object identifier, Func<TObj, TContract> method)
        {
            return FromResolveAllGetter<TObj>(identifier, method, InjectSources.Any);
        }

        public ScopeConcreteIdArgConditionCopyNonLazyBinder FromResolveAllGetter<TObj>(object identifier, Func<TObj, TContract> method, InjectSources source)
        {
            return FromResolveGetterBase<TObj, TContract>(identifier, method, source, true);
        }

        public ScopeConcreteIdArgConditionCopyNonLazyBinder FromInstance(TContract instance)
        {
            return FromInstanceBase(instance);
        }

#if !NOT_UNITY3D

        public ScopeConcreteIdArgConditionCopyNonLazyBinder FromComponentsInChildren(
            Func<TContract, bool> predicate, bool includeInactive = true)
        {
            return FromComponentsInChildren(false, predicate, includeInactive);
        }

        public ScopeConcreteIdArgConditionCopyNonLazyBinder FromComponentsInChildren(
            bool excludeSelf = false, Func<TContract, bool> predicate = null, bool includeInactive = true)
        {
            Func<Component, bool> subPredicate;

            if (predicate != null)
            {
                subPredicate = component => predicate((TContract)(object)component);
            }
            else
            {
                subPredicate = null;
            }

            return FromComponentsInChildrenBase(
                excludeSelf, subPredicate, includeInactive);
        }

        public ScopeConcreteIdArgConditionCopyNonLazyBinder FromComponentsInHierarchy(
            Func<TContract, bool> predicate = null, bool includeInactive = true)
        {
            Func<Component, bool> subPredicate;

            if (predicate != null)
            {
                subPredicate = component => predicate((TContract)(object)component);
            }
            else
            {
                subPredicate = null;
            }

            return FromComponentsInHierarchyBase(subPredicate, includeInactive);
        }
#endif
    }
}
