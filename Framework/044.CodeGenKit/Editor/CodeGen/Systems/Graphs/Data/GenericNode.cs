using System.ComponentModel;

namespace QFramework.CodeGen
{

    [Browsable(false)]
    public class GenericNode : GraphNode, ITypeInfo
    {
        public virtual ITypeInfo BaseTypeInfo
        {
            get { return null; }
        }

        public bool HasAttribute()
        {
            if (BaseTypeInfo == null)
            {
                return false;
            }
            return BaseTypeInfo.HasAttribute();
        }
    }
}