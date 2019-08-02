using System.Linq;
using Invert.Common;
using QF.GraphDesigner;
using UnityEngine;

public class SelectionRectHandler : Drawer, IInputHandler
{
    public override int ZOrder
    {
        get { return 100; }
    }

    public DiagramViewModel ViewModel
    {
        get { return DataContext as DiagramViewModel; }
        set { DataContext = value; }
    }

    public SelectionRectHandler(DiagramViewModel diagram)
    {
        ViewModel = diagram;
    }

    public override void OnMouseDoubleClick(MouseEvent mouseEvent)
    {
        mouseEvent.Cancel();
    }

    public override void OnMouseDown(MouseEvent mouseEvent)
    {
        mouseEvent.Cancel();
    }

    public override void OnMouseMove(MouseEvent e)
    {
        if (e.IsMouseDown && e.MouseButton == 0)
            SelectionRect = CreateSelectionRect(e.MouseDownPosition, e.MousePosition);
    }

    public Rect SelectionRect { get; set; }

    public static Rect CreateSelectionRect(Vector2 start, Vector2 current)
    {
        if (current.x > start.x)
        {
            if (current.y > start.y)
            {
                return new Rect(start.x, start.y,
                    current.x - start.x, current.y - start.y);
            }
            else
            {
                return new Rect(
                    start.x, current.y, current.x - start.x, start.y - current.y);
            }
        }
        else
        {
            if (current.y > start.y)
            {
                // x is less and y is greater
                return new Rect(
                    current.x, start.y, start.x - current.x, current.y - start.y);
            }
            else
            {
                // both are less
                return new Rect(
                    current.x, current.y, start.x - current.x, start.y - current.y);
            }
        }
    }

    public override void OnMouseUp(MouseEvent mouseEvent)
    {
        SelectionRect = new Rect(0f, 0f, 0f, 0f);
        mouseEvent.Cancel();
    }




    public override void Draw(IPlatformDrawer platform, float scale)
    {
        base.Draw(platform, scale);
        if (ViewModel == null) return;
        if (ViewModel.GraphItems == null) return;
        if (SelectionRect.width > 20 && SelectionRect.height > 20)
        {
            foreach (var item in ViewModel.GraphItems.OfType<DiagramNodeViewModel>())
            {
                item.IsSelected = SelectionRect.Scale(scale).Overlaps(item.Bounds.Scale(scale));
            }
#if UNITY_EDITOR
            platform.DrawStretchBox(SelectionRect.Scale(scale), CachedStyles.BoxHighlighter4, 12);
#else
            platform.DrawStretchBox(SelectionRect.Scale(scale), CachedStyles.BoxHighlighter4, 0);
#endif
        }
    }
}