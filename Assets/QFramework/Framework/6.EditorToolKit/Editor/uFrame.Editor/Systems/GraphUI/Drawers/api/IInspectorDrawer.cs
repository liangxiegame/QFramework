namespace QF.GraphDesigner
{
    public interface IInspectorDrawer : IDrawer
    {
        void DrawInspector(IPlatformDrawer platformDrawer);
    }
}