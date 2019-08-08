using System.Collections.Generic;
using System.ComponentModel;

namespace QF.GraphDesigner
{
    public interface IInhertable : IDiagramNode
    {
        [Browsable(false)]
        GenericInheritableNode BaseNode { get; }
        [Browsable(false)]
        IEnumerable<GenericInheritableNode> BaseNodes { get; }
        [Browsable(false)]
        IEnumerable<GenericInheritableNode> DerivedNodes { get; }
    }
}