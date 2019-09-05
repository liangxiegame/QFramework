using QF.GraphDesigner;

namespace QF.GraphDesigner
{
    public interface IGraphWindow
    {
        DiagramViewModel DiagramViewModel { get; }
        float Scale { get; set; }
        DiagramDrawer DiagramDrawer { get; set; }
        void RefreshContent();
        void ProjectChanged(Workspace project);
    }
}