using System;
using System.Collections.Generic;
using ModestTree;

namespace Zenject
{
    // Zero params

    [NoReflectionBaking]
    public class MethodProviderWithContainer<TValue> : IProvider
    {
        readonly Func<DiContainer, TValue> _method;

        public MethodProviderWithContainer(Func<DiContainer, TValue> method)
        {
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
            return typeof(TValue);
        }

        public void GetAllInstancesWithInjectSplit(
            InjectContext context, List<TypeValuePair> args, out Action injectAction, List<object> buffer)
        {
            Assert.IsEmpty(args);
            Assert.IsNotNull(context);

            Assert.That(typeof(TValue).DerivesFromOrEqual(context.MemberType));

            injectAction = null;
            if (context.Container.IsValidating)
            {
                // Don't do anything when validating, we can't make any assumptions on the given method
                buffer.Add(new ValidationMarker(typeof(TValue)));
            }
            else
            {
                buffer.Add(_method(context.Container));
            }
        }
    }

    // One params

    [NoReflectionBaking]
    public class MethodProviderWithContainer<TParam1, TValue> : IProvider
    {
        readonly Func<DiContainer, TParam1, TValue> _method;

        public MethodProviderWithContainer(Func<DiContainer, TParam1, TValue> method)
        {
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
            return typeof(TValue);
        }

        public void GetAllInstancesWithInjectSplit(
            InjectContext context, List<TypeValuePair> args, out Action injectAction, List<object> buffer)
        {
            Assert.IsEqual(args.Count, 1);
            Assert.IsNotNull(context);

            Assert.That(typeof(TValue).DerivesFromOrEqual(context.MemberType));
            Assert.That(args[0].Type.DerivesFromOrEqual(typeof(TParam1)));

            injectAction = null;
            if (context.Container.IsValidating)
            {
                // Don't do anything when validating, we can't make any assumptions on the given method
                buffer.Add(new ValidationMarker(typeof(TValue)));
            }
            else
            {
                buffer.Add(
                    _method(
                        context.Container,
                        (TParam1)args[0].Value));
            }
        }
    }

    // Two params

    [NoReflectionBaking]
    public class MethodProviderWithContainer<TParam1, TParam2, TValue> : IProvider
    {
        readonly Func<DiContainer, TParam1, TParam2, TValue> _method;

        public MethodProviderWithContainer(Func<DiContainer, TParam1, TParam2, TValue> method)
        {
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
            return typeof(TValue);
        }

        public void GetAllInstancesWithInjectSplit(
            InjectContext context, List<TypeValuePair> args, out Action injectAction, List<object> buffer)
        {
            Assert.IsEqual(args.Count, 2);
            Assert.IsNotNull(context);

            Assert.That(typeof(TValue).DerivesFromOrEqual(context.MemberType));
            Assert.That(args[0].Type.DerivesFromOrEqual(typeof(TParam1)));
            Assert.That(args[1].Type.DerivesFromOrEqual(typeof(TParam2)));

            injectAction = null;
            if (context.Container.IsValidating)
            {
                // Don't do anything when validating, we can't make any assumptions on the given method
                buffer.Add(new ValidationMarker(typeof(TValue)));
            }
            else
            {
                buffer.Add(
                    _method(
                        context.Container,
                        (TParam1)args[0].Value,
                        (TParam2)args[1].Value));
            }
        }
    }

    // Three params

    [NoReflectionBaking]
    public class MethodProviderWithContainer<TParam1, TParam2, TParam3, TValue> : IProvider
    {
        readonly Func<DiContainer, TParam1, TParam2, TParam3, TValue> _method;

        public MethodProviderWithContainer(Func<DiContainer, TParam1, TParam2, TParam3, TValue> method)
        {
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
            return typeof(TValue);
        }

        public void GetAllInstancesWithInjectSplit(
            InjectContext context, List<TypeValuePair> args, out Action injectAction, List<object> buffer)
        {
            Assert.IsEqual(args.Count, 3);
            Assert.IsNotNull(context);

            Assert.That(typeof(TValue).DerivesFromOrEqual(context.MemberType));
            Assert.That(args[0].Type.DerivesFromOrEqual(typeof(TParam1)));
            Assert.That(args[1].Type.DerivesFromOrEqual(typeof(TParam2)));
            Assert.That(args[2].Type.DerivesFromOrEqual(typeof(TParam3)));

            injectAction = null;
            if (context.Container.IsValidating)
            {
                // Don't do anything when validating, we can't make any assumptions on the given method
                buffer.Add(new ValidationMarker(typeof(TValue)));
            }
            else
            {
                buffer.Add(
                    _method(
                        context.Container,
                        (TParam1)args[0].Value,
                        (TParam2)args[1].Value,
                        (TParam3)args[2].Value));
            }
        }
    }

    // Four params

    [NoReflectionBaking]
    public class MethodProviderWithContainer<TParam1, TParam2, TParam3, TParam4, TValue> : IProvider
    {
        readonly
#if !NET_4_6
            ModestTree.Util.
#endif
            Func<DiContainer, TParam1, TParam2, TParam3, TParam4, TValue> _method;

        public MethodProviderWithContainer(
#if !NET_4_6
            ModestTree.Util.
#endif
            Func<DiContainer, TParam1, TParam2, TParam3, TParam4, TValue> method)
        {
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
            return typeof(TValue);
        }

        public void GetAllInstancesWithInjectSplit(
            InjectContext context, List<TypeValuePair> args, out Action injectAction, List<object> buffer)
        {
            Assert.IsEqual(args.Count, 4);
            Assert.IsNotNull(context);

            Assert.That(typeof(TValue).DerivesFromOrEqual(context.MemberType));
            Assert.That(args[0].Type.DerivesFromOrEqual(typeof(TParam1)));
            Assert.That(args[1].Type.DerivesFromOrEqual(typeof(TParam2)));
            Assert.That(args[2].Type.DerivesFromOrEqual(typeof(TParam3)));
            Assert.That(args[3].Type.DerivesFromOrEqual(typeof(TParam4)));

            injectAction = null;
            if (context.Container.IsValidating)
            {
                // Don't do anything when validating, we can't make any assumptions on the given method
                buffer.Add(new ValidationMarker(typeof(TValue)));
            }
            else
            {
                buffer.Add(
                    _method(
                        context.Container,
                        (TParam1)args[0].Value,
                        (TParam2)args[1].Value,
                        (TParam3)args[2].Value,
                        (TParam4)args[3].Value));
            }
        }
    }

    // Five params

    [NoReflectionBaking]
    public class MethodProviderWithContainer<TParam1, TParam2, TParam3, TParam4, TParam5, TValue> : IProvider
    {
        readonly
#if !NET_4_6
            ModestTree.Util.
#endif
            Func<DiContainer, TParam1, TParam2, TParam3, TParam4, TParam5, TValue> _method;

        public MethodProviderWithContainer(
#if !NET_4_6
            ModestTree.Util.
#endif
            Func<DiContainer, TParam1, TParam2, TParam3, TParam4, TParam5, TValue> method)
        {
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
            return typeof(TValue);
        }

        public void GetAllInstancesWithInjectSplit(
            InjectContext context, List<TypeValuePair> args, out Action injectAction, List<object> buffer)
        {
            Assert.IsEqual(args.Count, 5);
            Assert.IsNotNull(context);

            Assert.That(typeof(TValue).DerivesFromOrEqual(context.MemberType));
            Assert.That(args[0].Type.DerivesFromOrEqual(typeof(TParam1)));
            Assert.That(args[1].Type.DerivesFromOrEqual(typeof(TParam2)));
            Assert.That(args[2].Type.DerivesFromOrEqual(typeof(TParam3)));
            Assert.That(args[3].Type.DerivesFromOrEqual(typeof(TParam4)));
            Assert.That(args[4].Type.DerivesFromOrEqual(typeof(TParam5)));

            injectAction = null;
            if (context.Container.IsValidating)
            {
                // Don't do anything when validating, we can't make any assumptions on the given method
                buffer.Add(new ValidationMarker(typeof(TValue)));
            }
            else
            {
                buffer.Add(
                    _method(
                        context.Container,
                        (TParam1)args[0].Value,
                        (TParam2)args[1].Value,
                        (TParam3)args[2].Value,
                        (TParam4)args[3].Value,
                        (TParam5)args[4].Value));
            }
        }
    }

    // Six params

    [NoReflectionBaking]
    public class MethodProviderWithContainer<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TValue> : IProvider
    {
        readonly
#if !NET_4_6
            ModestTree.Util.
#endif
            Func<DiContainer, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TValue> _method;

        public MethodProviderWithContainer(
#if !NET_4_6
            ModestTree.Util.
#endif
            Func<DiContainer, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TValue> method)
        {
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
            return typeof(TValue);
        }

        public void GetAllInstancesWithInjectSplit(
            InjectContext context, List<TypeValuePair> args, out Action injectAction, List<object> buffer)
        {
            Assert.IsEqual(args.Count, 5);
            Assert.IsNotNull(context);

            Assert.That(typeof(TValue).DerivesFromOrEqual(context.MemberType));
            Assert.That(args[0].Type.DerivesFromOrEqual(typeof(TParam1)));
            Assert.That(args[1].Type.DerivesFromOrEqual(typeof(TParam2)));
            Assert.That(args[2].Type.DerivesFromOrEqual(typeof(TParam3)));
            Assert.That(args[3].Type.DerivesFromOrEqual(typeof(TParam4)));
            Assert.That(args[4].Type.DerivesFromOrEqual(typeof(TParam5)));
            Assert.That(args[5].Type.DerivesFromOrEqual(typeof(TParam6)));

            injectAction = null;
            if (context.Container.IsValidating)
            {
                // Don't do anything when validating, we can't make any assumptions on the given method
                buffer.Add(new ValidationMarker(typeof(TValue)));
            }
            else
            {
                buffer.Add(
                    _method(
                        context.Container,
                        (TParam1)args[0].Value,
                        (TParam2)args[1].Value,
                        (TParam3)args[2].Value,
                        (TParam4)args[3].Value,
                        (TParam5)args[4].Value,
                        (TParam6)args[5].Value));
            }
        }
    }

    // Ten params

    [NoReflectionBaking]
    public class MethodProviderWithContainer<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TValue> : IProvider
    {
        readonly
#if !NET_4_6
            ModestTree.Util.
#endif
            Func<DiContainer, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TValue> _method;

        public MethodProviderWithContainer(
#if !NET_4_6
            ModestTree.Util.
#endif
            Func<DiContainer, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TValue> method)
        {
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
            return typeof(TValue);
        }

        public void GetAllInstancesWithInjectSplit(
            InjectContext context, List<TypeValuePair> args, out Action injectAction, List<object> buffer)
        {
            Assert.IsEqual(args.Count, 10);
            Assert.IsNotNull(context);

            Assert.That(typeof(TValue).DerivesFromOrEqual(context.MemberType));
            Assert.That(args[0].Type.DerivesFromOrEqual(typeof(TParam1)));
            Assert.That(args[1].Type.DerivesFromOrEqual(typeof(TParam2)));
            Assert.That(args[2].Type.DerivesFromOrEqual(typeof(TParam3)));
            Assert.That(args[3].Type.DerivesFromOrEqual(typeof(TParam4)));
            Assert.That(args[4].Type.DerivesFromOrEqual(typeof(TParam5)));
            Assert.That(args[5].Type.DerivesFromOrEqual(typeof(TParam6)));
            Assert.That(args[6].Type.DerivesFromOrEqual(typeof(TParam7)));
            Assert.That(args[7].Type.DerivesFromOrEqual(typeof(TParam8)));
            Assert.That(args[8].Type.DerivesFromOrEqual(typeof(TParam9)));
            Assert.That(args[9].Type.DerivesFromOrEqual(typeof(TParam10)));

            injectAction = null;
            if (context.Container.IsValidating)
            {
                // Don't do anything when validating, we can't make any assumptions on the given method
                buffer.Add(new ValidationMarker(typeof(TValue)));
            }
            else
            {
                buffer.Add(
                    _method(
                        context.Container,
                        (TParam1)args[0].Value,
                        (TParam2)args[1].Value,
                        (TParam3)args[2].Value,
                        (TParam4)args[3].Value,
                        (TParam5)args[4].Value,
                        (TParam6)args[5].Value,
                        (TParam7)args[6].Value,
                        (TParam8)args[7].Value,
                        (TParam9)args[8].Value,
                        (TParam10)args[9].Value));
            }
        }
    }
}

