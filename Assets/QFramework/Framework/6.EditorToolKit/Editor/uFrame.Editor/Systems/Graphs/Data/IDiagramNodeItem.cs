using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using QF.GraphDesigner;
using Invert.Data;
using QF;

namespace QF.GraphDesigner
{
    public interface IDiagramNodeItem : ISelectable, IItem, IConnectable, IDataRecord
    {
        bool Precompiled { get; set; }
        string Name { get; set; }
        string Highlighter { get; }
        string FullLabel { get; }
        bool IsSelectable { get;}
        GraphNode Node { get; set; }
        
        /// <summary>
        /// Is this node currently in edit mode/ rename mode.
        /// </summary>
        bool IsEditing { get; set; }

        bool this[string flag] { get; set; }

        string Namespace { get; }
        string NodeId { get; set; }


        //void Remove(IDiagramNode diagramNode);
        void Rename(IDiagramNode data, string name);
        void NodeRemoved(IDiagramNode nodeData);
        void NodeItemRemoved(IDiagramNodeItem nodeItem);
        void NodeAdded(IDiagramNode data);
        void NodeItemAdded(IDiagramNodeItem data);
        void Validate(List<ErrorInfo> info);
        ErrorInfo[] Errors { get; set; }
        int Order { get; set; }
        IEnumerable<IFlagItem> DisplayedFlags { get; }
    }
}