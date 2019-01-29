namespace QFramework.GraphDesigner
{
    public class PullNodeCommand : Command, IFileSyncCommand
    {
        public IDiagramNode[] Node { get; set; }
    }
}