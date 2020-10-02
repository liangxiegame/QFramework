namespace QFramework.CodeGen
{
    public static class GraphDataExtensions
    {
        //public static IEnumerable<IDiagramNode> FilterItems(this IGraphData designerData, INodeRepository repository)
        //{
        //    return designerData.CurrentFilter.FilterItems(repository);
        //}



    

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