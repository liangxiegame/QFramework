using System;

namespace QFramework.CodeGeneration.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class)]
    public class EntityIndexAttribute : AbstractEntityIndexAttribute
    {
        public EntityIndexAttribute() : base(EntityIndexType.EntityIndex)
        {
        }
    }
}