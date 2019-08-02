using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using Zenject.Internal;

namespace Zenject
{
    [NoReflectionBaking]
    public class TransientProvider : IProvider
    {
        readonly DiContainer _container;
        readonly Type _concreteType;
        readonly List<TypeValuePair> _extraArguments;
        readonly object _concreteIdentifier;
        readonly Action<InjectContext, object> _instantiateCallback;

        public TransientProvider(
            Type concreteType, DiContainer container,
            IEnumerable<TypeValuePair> extraArguments, string bindingContext,
            object concreteIdentifier,
            Action<InjectContext, object> instantiateCallback)
        {
            Assert.That(!concreteType.IsAbstract(),
                "Expected non-abstract type for given binding but instead found type '{0}'{1}",
                concreteType, bindingContext == null ? "" : " when binding '{0}'".Fmt(bindingContext));

            _container = container;
            _concreteType = concreteType;
            _extraArguments = extraArguments.ToList();
            _concreteIdentifier = concreteIdentifier;
            _instantiateCallback = instantiateCallback;
        }

        public bool IsCached
        {
            get { return false; }
        }

        public bool TypeVariesBasedOnMemberType
        {
            get { return _concreteType.IsOpenGenericType(); }
        }

        public Type GetInstanceType(InjectContext context)
        {
            if (!_concreteType.DerivesFromOrEqual(context.MemberType))
            {
                return null;
            }

            return GetTypeToCreate(context.MemberType);
        }

        public void GetAllInstancesWithInjectSplit(
            InjectContext context, List<TypeValuePair> args, out Action injectAction, List<object> buffer)
        {
            Assert.IsNotNull(context);

            var instanceType = GetTypeToCreate(context.MemberType);

            var extraArgs = ZenPools.SpawnList<TypeValuePair>();

            extraArgs.AllocFreeAddRange(_extraArguments);
            extraArgs.AllocFreeAddRange(args);

            var instance = _container.InstantiateExplicit(instanceType, false, extraArgs, context, _concreteIdentifier);

            injectAction = () =>
            {
                _container.InjectExplicit(
                    instance, instanceType, extraArgs, context, _concreteIdentifier);

                Assert.That(extraArgs.Count == 0);
                ZenPools.DespawnList(extraArgs);

                if (_instantiateCallback != null)
                {
                    _instantiateCallback(context, instance);
                }
            };

            buffer.Add(instance);
        }

        Type GetTypeToCreate(Type contractType)
        {
            return ProviderUtil.GetTypeToInstantiate(contractType, _concreteType);
        }
    }
}
