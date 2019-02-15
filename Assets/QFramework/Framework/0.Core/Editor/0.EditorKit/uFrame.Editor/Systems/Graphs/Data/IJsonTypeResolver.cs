using System;

namespace QFramework
{
    public interface IJsonTypeResolver
    {
        Type FindType(string clrTypeString);
    }
}