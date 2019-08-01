using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    [NoReflectionBaking]
    public class InstanceProvider : IProvider
    {
        readonly object _instance;
        readonly Type _instanceType;
        readonly DiContainer _container;

        public InstanceProvider(
            Type instanceType, object instance, DiContainer container)
        {
            _instanceType = instanceType;
            _instance = instance;
            _container = container;
        }

        public bool IsCached
        {
            get { return true; }
        }

        public bool TypeVariesBasedOnMemberType
        {
            get { return false; }
        }

        public Type GetInstanceType(InjectContext context)
        {
            return _instanceType;
        }

        public void GetAllInstancesWithInjectSplit(
            InjectContext context, List<TypeValuePair> args, out Action injectAction, List<object> buffer)
        {
            Assert.That(args.Count == 0);
            Assert.IsNotNull(context);

            Assert.That(_instanceType.DerivesFromOrEqual(context.MemberType));

            injectAction = () => _container.LazyInject(_instance);

            buffer.Add(_instance);
        }
    }
}
