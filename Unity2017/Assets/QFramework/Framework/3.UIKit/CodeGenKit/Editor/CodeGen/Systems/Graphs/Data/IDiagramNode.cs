using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using Invert.Data;

namespace QFramework.CodeGen
{

    public interface IDiagramNode : IDiagramNodeItem, IDataHeirarchy
    {
        /// <summary>
        /// The items that should be persisted with this diagram node.
        /// </summary>
        IEnumerable<IDiagramNodeItem> PersistedItems { get; }
    }
}