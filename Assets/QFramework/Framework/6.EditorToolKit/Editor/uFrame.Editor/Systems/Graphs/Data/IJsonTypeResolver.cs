using System;

namespace QF
{
    public interface IJsonTypeResolver
    {
        Type FindType(string clrTypeString);
    }
}