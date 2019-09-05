using UnityEngine;

namespace QF.GraphDesigner
{

    public interface IConnectorStyleSchema
    {
        object GetTexture(ConnectorSide side, ConnectorDirection direction, bool connected, Color tint = default(Color));

        IConnectorStyleSchema WithInputIcons(string emptyIcon, string filledIcon);
        IConnectorStyleSchema WithOutputIcons(string emptyIcon, string filledIcon);
        IConnectorStyleSchema WithTwoWayIcons(string emptyIcon, string filledIcon);
        IConnectorStyleSchema WithDefaultIcons();
        IConnectorStyleSchema WithPad(float left, float top, float right, float bottom);
        Rect Padding { get; set; }
    }
}
