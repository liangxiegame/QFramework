namespace QFramework.CodeGen
{
    public static class GraphDataExtensions
    {
        public static IDiagramNode RelatedNode(this ITypedItem item)
        {
            var gt = item as GenericTypedChildItem;
            if (gt != null)
            {
                return gt.RelatedTypeNode as IDiagramNode;
            }
            
            return item.Repository.GetById<IDiagramNode>(item.RelatedType);
        }
    }
}