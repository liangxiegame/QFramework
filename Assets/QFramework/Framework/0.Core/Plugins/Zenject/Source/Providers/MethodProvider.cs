using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    [NoReflectionBaking]
    public class MethodProvider<TReturn> : IProvider
    {
        readonly DiContainer _container;
        readonly Func<InjectContext, TReturn> _method;

        public MethodProvider(
            Func<InjectContext, TReturn> method,
            DiContainer container)
        {
            _container = container;
            _method = method;
        }

        public bool IsCached
        {
            get { return false; }
        }

        public bool TypeVariesBasedOnMemberType
        {
            get { return false; }
        }

        public Type GetInstanceType(InjectContext context)
        {
            return typeof(TReturn);
        }

        public void GetAllInstancesWithInjectSplit(
            InjectContext context, List<TypeValuePair> args, out Action injectAction, List<object> buffer)
        {
            Assert.IsEmpty(args);
            Assert.IsNotNull(context);

            Assert.That(typeof(TReturn).DerivesFromOrEqual(context.MemberType));

            injectAction = null;
            if (_container.IsValidating && !TypeAnalyzer.ShouldAllowDuringValidation(context.MemberType))
            {
                buffer.Add(new ValidationMarker(typeof(TReturn)));
            }
            else
            {
                // We cannot do a null assert here because in some cases they might intentionally
                // return null
                buffer.Add(_method(context));
            }
        }
    }
}
