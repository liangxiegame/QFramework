namespace QF.GraphDesigner
{
    public interface ISelectable : IGraphItem
    {
        bool IsSelected { get; set; }
    }
}