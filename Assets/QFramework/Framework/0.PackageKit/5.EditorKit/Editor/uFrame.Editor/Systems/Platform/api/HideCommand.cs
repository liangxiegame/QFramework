namespace QF.GraphDesigner
{
    public class HideCommand : Command
    {
        public IDiagramNode[] Node { get; set; }
        public IGraphFilter Filter { get; set; }
    }
}