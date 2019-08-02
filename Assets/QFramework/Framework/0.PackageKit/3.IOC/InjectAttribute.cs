using System;


namespace QF

{
    /// <summary>
    /// Used by the injection container to determine if a property or field should be injected.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InjectAttribute : Attribute
    {
        public InjectAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public InjectAttribute()
        {
        }
    }

}