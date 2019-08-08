using UnityEngine;

namespace QF.GraphDesigner.Systems.GraphUI
{
    public interface IOverlayDrawer
    {
        void Draw(Rect bouds);
        Rect CalculateBounds(Rect diagramRect);
    }
}