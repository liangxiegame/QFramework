namespace QFramework.CodeGen
{
    public interface ISelectable : IGraphItem
    {
        bool IsSelected { get; set; }
    }
}