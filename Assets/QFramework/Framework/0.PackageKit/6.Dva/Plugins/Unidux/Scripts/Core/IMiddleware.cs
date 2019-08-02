using System;

namespace Unidux
{
    public delegate Func<Func<object, object>, Func<object, object>> Middleware(IStoreObject store);
}