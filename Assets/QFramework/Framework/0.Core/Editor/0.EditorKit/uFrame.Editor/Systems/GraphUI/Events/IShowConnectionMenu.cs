using UnityEngine;

namespace QFramework.GraphDesigner
{
    public interface IShowConnectionMenu
    {
        void Show(DiagramViewModel diagramViewModel, ConnectorViewModel startConnector,Vector2 position);
    }
}