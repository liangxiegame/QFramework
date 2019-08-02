using UnityEngine;

namespace QFramework.GraphDesigner.Systems.GraphUI
{
    public interface IOverlayDrawer
    {
        void Draw(Rect bouds);
        Rect CalculateBounds(Rect diagramRect);
    }
}