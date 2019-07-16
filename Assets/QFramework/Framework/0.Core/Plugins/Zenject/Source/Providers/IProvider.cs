using System;
using System.Collections.Generic;

namespace Zenject
{
    // The given InjectContext values here should always be non-null
    public interface IProvider
    {
        bool TypeVariesBasedOnMemberType
        {
            get;
        }

        bool IsCached
        {
            get;
        }

        Type GetInstanceType(InjectContext context);

        // Return an instance which might be not yet injected to.
        // injectAction should handle the actual injection
        // This way, providers that call CreateInstance() can store the instance immediately,
        // and then return that if something gets created during injection that refers back
        // to the newly created instance
        void GetAllInstancesWithInjectSplit(
            InjectContext context, List<TypeValuePair> args, out Action injectAction, List<object> instances);
    }
}
