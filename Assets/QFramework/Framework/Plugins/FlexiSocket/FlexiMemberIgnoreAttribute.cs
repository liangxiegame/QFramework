using System;

namespace QF
{
    /// <summary>
    /// Attributed public members will not be serialized
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class FlexiMemberIgnoreAttribute : Attribute
    {
        
    }
}