namespace QF.GraphDesigner
{
    public class ScaffoldGraph : InvertGraph
    {
        public ScaffoldGraph BeginNode<TNode>(string name) where TNode : GenericNode, new()
        {
            CurrentNode = new TNode()
            {
                Name = name
            };
            AddNode(CurrentNode);
            return this;
        }

        public ScaffoldGraph AddItem<TNodeItem>(string name, out TNodeItem nodeItem, string type = null)
            where TNodeItem : class, IDiagramNodeItem, new()
        {
            var item = Repository.Create<TNodeItem>();
            item.Name = name;
            item.Node = CurrentNode;
            if (type != null)
            {
                var typedItem = item as ITypedItem;
                typedItem.RelatedType = type;
            }

            nodeItem = item;
            return this;
        }

        public ScaffoldGraph AddItem<TNodeItem>(string name, string type = null)
            where TNodeItem : class, IDiagramNodeItem, new()
        {
            var item = Repository.Create<TNodeItem>();
            item.Name = name;
            item.Node = CurrentNode;

            if (type != null)
            {
                var typedItem = item as ITypedItem;
                typedItem.RelatedType = type;
            }

            return this;
        }

        public GenericNode CurrentNode { get; set; }

        public GenericNode EndNode()
        {
            return CurrentNode;
        }
    }
}