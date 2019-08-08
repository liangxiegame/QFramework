namespace QF.GraphDesigner
{
    public interface INodeDrawer : IDrawer
    {
        DiagramDrawer Diagram { get; set; }
        //Type CommandsType { get; }
        DiagramNodeViewModel ViewModel { get; set; }
    }
}