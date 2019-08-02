using System;

namespace QFramework.GraphDesigner
{
    [Obsolete]
    public interface IClassTypeNode : IDiagramNodeItem
    {
        string ClassName { get; }
    }
}