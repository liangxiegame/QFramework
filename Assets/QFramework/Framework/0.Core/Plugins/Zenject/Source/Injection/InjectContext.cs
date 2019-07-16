using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModestTree;
using Zenject.Internal;

namespace Zenject
{
    [NoReflectionBaking]
    public class InjectContext : IDisposable
    {
        BindingId _bindingId;
        Type _objectType;
        InjectContext _parentContext;
        object _objectInstance;
        string _memberName;
        bool _optional;
        InjectSources _sourceType;
        object _fallBackValue;
        object _concreteIdentifier;
        DiContainer _container;

        public InjectContext()
        {
            _bindingId = new BindingId();
            Reset();
        }

        public InjectContext(DiContainer container, Type memberType)
            : this()
        {
            Container = container;
            MemberType = memberType;
        }

        public InjectContext(DiContainer container, Type memberType, object identifier)
            : this(container, memberType)
        {
            Identifier = identifier;
        }

        public InjectContext(DiContainer container, Type memberType, object identifier, bool optional)
            : this(container, memberType, identifier)
        {
            Optional = optional;
        }

        public void Dispose()
        {
            ZenPools.DespawnInjectContext(this);
        }

        public void Reset()
        {
            _objectType = null;
            _parentContext = null;
            _objectInstance = null;
            _memberName = "";
            _optional = false;
            _sourceType = InjectSources.Any;
            _fallBackValue = null;
            _container = null;
            _bindingId.Type = null;
            _bindingId.Identifier = null;
        }

        public BindingId BindingId
        {
            get { return _bindingId; }
        }

        // The type of the object which is having its members injected
        // NOTE: This is null for root calls to Resolve<> or Instantiate<>
        public Type ObjectType
        {
            get { return _objectType; }
            set { _objectType = value; }
        }

        // Parent context that triggered the creation of ObjectType
        // This can be used for very complex conditions using parent info such as identifiers, types, etc.
        // Note that ParentContext.MemberType is not necessarily the same as ObjectType,
        // since the ObjectType could be a derived type from ParentContext.MemberType
        public InjectContext ParentContext
        {
            get { return _parentContext; }
            set { _parentContext = value; }
        }

        // The instance which is having its members injected
        // Note that this is null when injecting into the constructor
        public object ObjectInstance
        {
            get { return _objectInstance; }
            set { _objectInstance = value; }
        }

        // Identifier - most of the time this is null
        // It will match 'foo' in this example:
        //      ... In an installer somewhere:
        //          Container.Bind<Foo>("foo").AsSingle();
        //      ...
        //      ... In a constructor:
        //          public Foo([Inject(Id = "foo") Foo foo)
        public object Identifier
        {
            get { return _bindingId.Identifier; }
            set { _bindingId.Identifier = value; }
        }

        // The constructor parameter name, or field name, or property name
        public string MemberName
        {
            get { return _memberName; }
            set { _memberName = value; }
        }

        // The type of the constructor parameter, field or property
        public Type MemberType
        {
            get { return _bindingId.Type; }
            set { _bindingId.Type = value; }
        }

        // When optional, null is a valid value to be returned
        public bool Optional
        {
            get { return _optional; }
            set { _optional = value; }
        }

        // When set to true, this will only look up dependencies in the local container and will not
        // search in parent containers
        public InjectSources SourceType
        {
            get { return _sourceType; }
            set { _sourceType = value; }
        }

        public object ConcreteIdentifier
        {
            get { return _concreteIdentifier; }
            set { _concreteIdentifier = value; }
        }

        // When optional, this is used to provide the value
        public object FallBackValue
        {
            get { return _fallBackValue; }
            set { _fallBackValue = value; }
        }

        // The container used for this injection
        public DiContainer Container
        {
            get { return _container; }
            set { _container = value; }
        }

        public IEnumerable<InjectContext> ParentContexts
        {
            get
            {
                if (ParentContext == null)
                {
                    yield break;
                }

                yield return ParentContext;

                foreach (var context in ParentContext.ParentContexts)
                {
                    yield return context;
                }
            }
        }

        public IEnumerable<InjectContext> ParentContextsAndSelf
        {
            get
            {
                yield return this;

                foreach (var context in ParentContexts)
                {
                    yield return context;
                }
            }
        }

        // This will return the types of all the objects that are being injected
        // So if you have class Foo which has constructor parameter of type IBar,
        // and IBar resolves to Bar, this will be equal to (Bar, Foo)
        public IEnumerable<Type> AllObjectTypes
        {
            get
            {
                foreach (var context in ParentContextsAndSelf)
                {
                    if (context.ObjectType != null)
                    {
                        yield return context.ObjectType;
                    }
                }
            }
        }

        public InjectContext CreateSubContext(Type memberType)
        {
            return CreateSubContext(memberType, null);
        }

        public InjectContext CreateSubContext(Type memberType, object identifier)
        {
            var subContext = new InjectContext();

            subContext.ParentContext = this;
            subContext.Identifier = identifier;
            subContext.MemberType = memberType;

            // Clear these
            subContext.ConcreteIdentifier = null;
            subContext.MemberName = "";
            subContext.FallBackValue = null;

            // Inherit these ones by default
            subContext.ObjectType = ObjectType;
            subContext.ObjectInstance = ObjectInstance;
            subContext.Optional = Optional;
            subContext.SourceType = SourceType;
            subContext.Container = Container;

            return subContext;
        }

        public InjectContext Clone()
        {
            var clone = new InjectContext();

            clone.ObjectType = ObjectType;
            clone.ParentContext = ParentContext;
            clone.ConcreteIdentifier = ConcreteIdentifier;
            clone.ObjectInstance = ObjectInstance;
            clone.Identifier = Identifier;
            clone.MemberType = MemberType;
            clone.MemberName = MemberName;
            clone.Optional = Optional;
            clone.SourceType = SourceType;
            clone.FallBackValue = FallBackValue;
            clone.Container = Container;

            return clone;
        }

        // This is very useful to print out for debugging purposes
        public string GetObjectGraphString()
        {
            var result = new StringBuilder();

            foreach (var context in ParentContextsAndSelf.Reverse())
            {
                if (context.ObjectType == null)
                {
                    continue;
                }

                result.AppendLine(context.ObjectType.PrettyName());
            }

            return result.ToString();
        }
    }
}
