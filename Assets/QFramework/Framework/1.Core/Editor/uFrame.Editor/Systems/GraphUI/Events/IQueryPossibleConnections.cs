using UnityEngine;

namespace QF.GraphDesigner
{
    public interface IQueryPossibleConnections
    {
        void QueryPossibleConnections(SelectionMenu menu,DiagramViewModel diagramViewModel,
            ConnectorViewModel startConnector,
            Vector2 mousePosition);
    }
}