using System;
using System.Collections.Generic;
using ModestTree;
using Zenject.Internal;

namespace Zenject
{
    public static class IProviderExtensions
    {
        static readonly List<TypeValuePair> EmptyArgList = new List<TypeValuePair>();

        public static void GetAllInstancesWithInjectSplit(
            this IProvider creator, InjectContext context, out Action injectAction, List<object> buffer)
        {
            creator.GetAllInstancesWithInjectSplit(
                context, EmptyArgList, out injectAction, buffer);
        }

        public static void GetAllInstances(
            this IProvider creator, InjectContext context, List<object> buffer)
        {
            creator.GetAllInstances(context, EmptyArgList, buffer);
        }

        public static void GetAllInstances(
            this IProvider creator, InjectContext context, List<TypeValuePair> args, List<object> buffer)
        {
            Assert.IsNotNull(context);

            Action injectAction;
            creator.GetAllInstancesWithInjectSplit(context, args, out injectAction, buffer);

            if (injectAction != null)
            {
                injectAction.Invoke();
            }
        }

        public static object TryGetInstance(
            this IProvider creator, InjectContext context)
        {
            return creator.TryGetInstance(context, EmptyArgList);
        }

        public static object TryGetInstance(
            this IProvider creator, InjectContext context, List<TypeValuePair> args)
        {
            var allInstances = ZenPools.SpawnList<object>();

            try
            {
                creator.GetAllInstances(context, args, allInstances);

                if (allInstances.Count == 0)
                {
                    return null;
                }

                Assert.That(allInstances.Count == 1,
                    "Provider returned multiple instances when one or zero was expected");

                return allInstances[0];
            }
            finally
            {
                ZenPools.DespawnList(allInstances);
            }
        }

        public static object GetInstance(
            this IProvider creator, InjectContext context)
        {
            return creator.GetInstance(context, EmptyArgList);
        }

        public static object GetInstance(
            this IProvider creator, InjectContext context, List<TypeValuePair> args)
        {
            var allInstances = ZenPools.SpawnList<object>();

            try
            {
                creator.GetAllInstances(context, args, allInstances);

                Assert.That(allInstances.Count > 0,
                    "Provider returned zero instances when one was expected when looking up type '{0}'", context.MemberType);

                Assert.That(allInstances.Count == 1,
                    "Provider returned multiple instances when only one was expected when looking up type '{0}'", context.MemberType);

                return allInstances[0];
            }
            finally
            {
                ZenPools.DespawnList(allInstances);
            }
        }
    }
}
