namespace QFramework.CodeGen
{
    public interface ITypedItem : IDiagramNodeItem
    {
        string RelatedType { get; set; }
        string RelatedTypeName { get; }
        void RemoveType();
    }
}