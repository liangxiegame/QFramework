using System;

namespace QF.GraphDesigner
{
    [Obsolete]
    public interface IClassTypeNode : IDiagramNodeItem
    {
        string ClassName { get; }
    }
}