namespace QFramework.CodeGen
{
    public interface IDiagramNodeItem : IConnectable
    {
        string Name { get; set; }


        string Namespace { get; }
        string NodeId { get; set; }


        //void Remove(IDiagramNode diagramNode);
        void NodeRemoved(IDiagramNode nodeData);

        int Order { get; set; }
    }
}