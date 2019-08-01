namespace QFramework.GraphDesigner
{
    public interface INamespaceProvider
    {
        string GetNamespace(IDiagramNode node);
    }
}