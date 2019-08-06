using System.Collections.Generic;
using UnityEngine;

namespace QF.GraphDesigner
{
    /// <summary>
    /// Drawer 框架
    /// </summary>
    public interface IDrawer
    {
        GraphItemViewModel ViewModelObject { get; }
        Rect               Bounds          { get; set; }
        bool               Enabled         { get; set; }
        bool               IsSelected      { get; set; }
        bool               Dirty           { get; set; }
        string             ShouldFocus     { get; set; }
        void Draw(IPlatformDrawer platform, float scale);
        void Refresh(IPlatformDrawer platform);
        void OnLayout();
        void Refresh(IPlatformDrawer platform, Vector2 position, bool hardRefresh = true);
        int           ZOrder       { get; }
        List<IDrawer> Children     { get; set; }
        IDrawer       ParentDrawer { get; set; }

        void OnDeselecting();
        void OnSelecting();
        void OnDeselected();
        void OnSelected();
        void OnMouseExit(MouseEvent e);
        void OnMouseEnter(MouseEvent e);
        void OnMouseMove(MouseEvent e);
        void OnDrag(MouseEvent e);
        void OnMouseUp(MouseEvent e);
        void OnMouseDoubleClick(MouseEvent mouseEvent);
        void OnRightClick(MouseEvent mouseEvent);
        void OnMouseDown(MouseEvent mouseEvent);
    }
}