using System;

namespace QFramework.CodeGen
{
    [Obsolete]
    public interface IClassTypeNode : IDiagramNodeItem
    {
        string ClassName { get; }
    }
}