using QFramework.GraphDesigner;

namespace QFramework.GraphDesigner
{
    public class ContextMenus : DiagramPlugin,
        IShowContextMenu
    {
        public void Show(MouseEvent evt, params object[] objects)
        {
            var ui = InvertApplication.Container.Resolve<ContextMenuUI>();
            Signal<IContextMenuQuery>(_ => _.QueryContextMenu(ui, evt, objects));

            ui.Go();
        }
    }
}