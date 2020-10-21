using System.ComponentModel;

namespace QFramework.CodeGen
{
    public class GenericInheritableNode : GenericNode
    {
        public override ITypeInfo BaseTypeInfo
        {
            get
            {
                if (BaseNode != null)
                {
                    return BaseNode;
                }
                return null;
            }
        }

        [Browsable(false)]
        public virtual GenericInheritableNode BaseNode
        {
            get
            {
                return null;
            }
            set { throw new System.NotImplementedException(); }
        }
    }
}