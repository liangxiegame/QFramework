namespace QFramework.GraphDesigner
{
    public interface ISelectable : IGraphItem
    {
        bool IsSelected { get; set; }
    }
}