using Invert.Data;
using UnityEngine;

namespace QF.GraphDesigner
{
    public interface IFilterItem : IDataRecord
    {
        bool Collapsed { get; set; }
        string NodeId { get; set; }
        string FilterId { get; set; }
        IDiagramNode Node { get; }
        IGraphFilter Filter { get; }
        Vector2 Position { get; set; }
    }
}