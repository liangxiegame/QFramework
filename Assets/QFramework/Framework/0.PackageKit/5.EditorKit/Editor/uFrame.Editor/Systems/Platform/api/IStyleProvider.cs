namespace QF.GraphDesigner
{
    public interface IStyleProvider
    {
        object GetImage(string name);
        object GetStyle(InvertStyles name);
        object GetFont(string fontName);

        INodeStyleSchema GetNodeStyleSchema(NodeStyle name);
        IConnectorStyleSchema GetConnectorStyleSchema(ConnectorStyle name);
        IBreadcrumbsStyleSchema GetBreadcrumbStyleSchema(BreadcrumbsStyle name);
    }
}