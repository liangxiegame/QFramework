using System;

namespace QFramework.CodeGeneration.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct)]
    public class CustomPrefixAttribute : Attribute
    {
        public readonly string prefix;

        public CustomPrefixAttribute(string prefix)
        {
            this.prefix = prefix;
        }
    }
}