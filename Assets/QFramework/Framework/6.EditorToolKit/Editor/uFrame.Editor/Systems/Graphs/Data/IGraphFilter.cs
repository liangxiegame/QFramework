using System.Collections.Generic;
using Invert.Data;

namespace QF.GraphDesigner
{
    public interface IGraphFilter : IDataRecord
    {
        bool ImportedOnly { get; }
        bool IsExplorerCollapsed { get; set; }

        string Name { get; set; }
        bool UseStraightLines { get; }
        IEnumerable<IDiagramNode> FilterNodes { get; }
        IEnumerable<IFilterItem> FilterItems { get; }
        bool AllowExternalNodes { get; }
    }
}