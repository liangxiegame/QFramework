namespace QFramework.CodeGen
{
    public interface IDiagramNodeItem : ISelectable, IConnectable
    {
        string Name { get; set; }
        string FullLabel { get; }
        GraphNode Node { get; set; }
        
        /// <summary>
        /// Is this node currently in edit mode/ rename mode.
        /// </summary>
        bool IsEditing { get; set; }


        string Namespace { get; }
        string NodeId { get; set; }


        //void Remove(IDiagramNode diagramNode);
        void Rename(IDiagramNode data, string name);
        void NodeRemoved(IDiagramNode nodeData);

        ErrorInfo[] Errors { get; set; }
        int Order { get; set; }
    }
}