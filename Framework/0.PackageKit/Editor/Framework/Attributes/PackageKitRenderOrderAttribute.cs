using System;

namespace QFramework
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PackageKitRenderOrderAttribute : Attribute
    {
        public int Order { get; private set; }
        
        public PackageKitRenderOrderAttribute(int order)
        {
            Order = order;
        }
    }
}