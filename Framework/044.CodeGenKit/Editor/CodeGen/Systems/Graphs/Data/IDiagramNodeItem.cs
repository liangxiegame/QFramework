namespace QFramework.CodeGen
{
    public interface IDiagramNodeItem : IConnectable
    {
        string Name { get; }


        string Namespace { get; }
        string NodeId { get; }


        //void Remove(IDiagramNode diagramNode);
        void NodeRemoved(IDiagramNode nodeData);

        int Order { get; }
    }
}