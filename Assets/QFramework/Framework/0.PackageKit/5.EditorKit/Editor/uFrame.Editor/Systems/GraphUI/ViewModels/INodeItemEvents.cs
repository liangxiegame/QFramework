namespace QF.GraphDesigner
{
    public interface INodeItemEvents
    {
        void Renamed(IDiagramNodeItem nodeItem, string editText, string name);
    }
}