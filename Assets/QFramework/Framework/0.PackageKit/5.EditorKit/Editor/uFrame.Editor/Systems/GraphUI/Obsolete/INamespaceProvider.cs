namespace QF.GraphDesigner
{
    public interface INamespaceProvider
    {
        string GetNamespace(IDiagramNode node);
    }
}