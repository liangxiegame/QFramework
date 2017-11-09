using System;

namespace QFramework.CodeGeneration.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct)]
    public class UniqueAttribute : Attribute
    {
    }
}