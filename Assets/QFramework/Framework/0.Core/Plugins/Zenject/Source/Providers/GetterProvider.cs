using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    [NoReflectionBaking]
    public class GetterProvider<TObj, TResult> : IProvider
    {
        readonly DiContainer _container;
        readonly object _identifier;
        readonly Func<TObj, TResult> _method;
        readonly bool _matchAll;
        readonly InjectSources _sourceType;

        public GetterProvider(
            object identifier, Func<TObj, TResult> method,
            DiContainer container, InjectSources sourceType, bool matchAll)
        {
            _container = container;
            _identifier = identifier;
            _method = method;
            _matchAll = matchAll;
            _sourceType = sourceType;
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
            return typeof(TResult);
        }

        InjectContext GetSubContext(InjectContext parent)
        {
            var subContext = parent.CreateSubContext(
                typeof(TObj), _identifier);

            subContext.Optional = false;
            subContext.SourceType = _sourceType;

            return subContext;
        }

        public void GetAllInstancesWithInjectSplit(
            InjectContext context, List<TypeValuePair> args, out Action injectAction, List<object> buffer)
        {
            Assert.IsEmpty(args);
            Assert.IsNotNull(context);

            Assert.That(typeof(TResult).DerivesFromOrEqual(context.MemberType));

            injectAction = null;

            if (_container.IsValidating)
            {
                // All we can do is validate that the getter object can be resolved
                if (_matchAll)
                {
                    _container.ResolveAll(GetSubContext(context));
                }
                else
                {
                    _container.Resolve(GetSubContext(context));
                }

                buffer.Add(new ValidationMarker(typeof(TResult)));
                return;
            }

            if (_matchAll)
            {
                Assert.That(buffer.Count == 0);
                _container.ResolveAll(GetSubContext(context), buffer);

                for (int i = 0; i < buffer.Count; i++)
                {
                    buffer[i] = _method((TObj)buffer[i]);
                }
            }
            else
            {
                buffer.Add(_method(
                    (TObj)_container.Resolve(GetSubContext(context))));
            }
        }
    }
}
