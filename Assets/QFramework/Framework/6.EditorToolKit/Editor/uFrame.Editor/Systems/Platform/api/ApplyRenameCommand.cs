namespace QF.GraphDesigner
{
    public class ApplyRenameCommand : Command
    {
        public IDiagramNode Item { get; set; }
        public string Old { get; set; }
        public string Name { get; set; }
    }
}