using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace QF.GraphDesigner
{
    public class GenericInheritableNode : GenericNode, IInhertable
    {
        public override bool AllowMultipleOutputs
        {
            get { return true; }
        }

        public override bool AllowMultipleInputs
        {
            get { return false; }
        }

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
                return this.InputsFrom<GenericInheritableNode>().FirstOrDefault(p => p.GetType() == this.GetType());
            }
            set { throw new System.NotImplementedException(); }
        }

        [Browsable(false)]
        public IEnumerable<GenericInheritableNode> BaseNodes
        {
            get
            {
                var baseType = BaseNode;
                while (baseType != null)
                {
                    yield return baseType;
                    baseType = baseType.BaseNode;
                }
            }
        }

        public IEnumerable<IDiagramNodeItem> ChildItemsWithInherited
        {
            get { return BaseNodesWithThis.SelectMany(p => p.PersistedItems); }
        }

        [Browsable(false)]
        public IEnumerable<GenericInheritableNode> BaseNodesWithThis
        {
            get
            {
                yield return this;
                var baseType = BaseNode;
                while (baseType != null)
                {
                    yield return baseType;
                    baseType = baseType.BaseNode;
                }
            }
        }
        [Browsable(false)]
        public IEnumerable<GenericInheritableNode> DerivedNodes
        {
            get
            {
                var derived = Repository.AllOf<GenericInheritableNode>().Where(p => p.BaseNode == this);
                foreach (var derivedItem in derived)
                {
                    yield return derivedItem;
                    foreach (var another in derivedItem.DerivedNodes)
                    {
                        yield return another;
                    }
                }
            }
        }
        //[Browsable(false)]
        ////[InputSlot("Base Class", OrderIndex = -1)]
        //public BaseClassReference BaseReference
        //{
        //    get { return _baseReference ?? (_baseReference = new BaseClassReference() { Node = this }); }
        //    set { _baseReference = value; }
        //}

        public override bool CanInputFrom(IConnectable output)
        {
            return base.CanInputFrom(output);
        }

        public override bool CanOutputTo(IConnectable input)
        {
            if (input == this) return false;
            if (this.GetType() != input.GetType()) return false;
            if (BaseNodes.Any(p => p == input)) return false;

            return base.CanOutputTo(input);
        }

        //public override bool ValidateInput(IDiagramNodeItem a, IDiagramNodeItem b)
        //{
        //    if (b is GenericInheritableNode && b.GetType() == a.GetType())
        //    {
        //        if (a.GetType() != b.GetType()) return false;
        //    }

        //    return base.ValidateInput(a, b);
        //}

        //public override bool ValidateOutput(IDiagramNodeItem a, IDiagramNodeItem b)
        //{
        //    if (b is GenericInheritableNode && b.GetType() == a.GetType())
        //    {
        //        if (BaseNodes.Any(p => p == b)) return false;

        //        if (a == b) return false; // Can't inherit from the same item
        //        if (a.GetType() != b.GetType()) return false; // Can't inherit from another type    
        //    }

        //    return base.ValidateOutput(a, b);
        //}
    }
}