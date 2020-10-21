using System;
using System.Collections.Generic;
using System.Linq;

using System.Text.RegularExpressions;
using Invert.Data;
using UnityEngine;
namespace QFramework.CodeGen
{

    /// <summary>
    /// The base data class for all diagram nodes.
    /// </summary>
    public abstract class GraphNode : IDiagramNode
    {
        public int Order
        {
            get { return _order; }
            set
            {
                this.Changed("Order", ref _order, value);
            }
        }


        public override string ToString()
        {
            return Name;
        }


        private IGraphData _data;




        private string _identifier;

        private bool _isCollapsed;

        private string _name;

        private Rect _position;
        private string _graphId;
        private bool _isSelected;
        private bool _expanded;
        private IGraphData _graph;
        private NodeConfigBase _nodeConfig;
        private int _order;
        private string _comments;

        public IEnumerable<FlagItem> Flags
        {
            get { return Repository.All<FlagItem>().Where(p => p.ParentIdentifier == this.Identifier); }
        }

        public bool this[string flag]
        {
            get { return Flags.Any(p => p.Name == flag); }
            set
            {
                var f = Flags.FirstOrDefault(p => p.Name == flag);
                if (value == false)
                {
                    if (f != null)
                    {
                        Repository.Remove(f);
                    }
                }
                else
                {
                    if (f == null)
                    {
                        f = new FlagItem();
                        f.ParentIdentifier = this.Identifier;
                        f.Name = flag;
                        Repository.Add(f);
                    }
                }
            }
        }


        /// <summary>
        /// The items that should be persisted with this diagram node.
        /// </summary>
        public virtual IEnumerable<IDiagramNodeItem> PersistedItems
        {
            get
            {
                return Repository.AllOf<IDiagramNodeItem>().Where(p => p.NodeId == this.Identifier).OrderBy(p=>p.Order);
            }
        }

        public string GraphId
        {
            get { return _graphId; }
            set {
                this.Changed("GraphId", ref  _graphId, value);
                _graph = null;
            }
        }

        /// <summary>
        /// Gets the diagram file that this node belongs to
        /// </summary>
        public IGraphData Graph
        {
            get
            {
                if (string.IsNullOrEmpty(GraphId))
                {
                 
                    return null;
                }
                return _graph ?? (_graph = Repository.GetById<IGraphData>(GraphId));
            }
            set
            {
                if (value != null) GraphId = value.Identifier;
                _graph = value;
            }
        }


        public IRepository Repository { get; set; }

        public virtual string Identifier
        {
            get { return string.IsNullOrEmpty(_identifier) ? (_identifier = Guid.NewGuid().ToString()) : _identifier; }
            set { _identifier = value; }
        }

        public bool Changed { get; set; }


        public bool IsNewNode { get; set; }

        public virtual string Name
        {
            get
            {
                //if (this.Graph != null && this.Graph.RootFilter == this)
                //{
                //    return this.Graph.Name;
                //}
                return _name;
            }
            set
            {
                if (value == null) return;
                this.Changed("Name", ref  _name, Regex.Replace(value, "[^a-zA-Z0-9_.]+", ""));
            }
        }

        //[InspectorProperty]
        public virtual string Namespace
        {
            get
            {
                if (Graph == null) return string.Empty;
                return Graph.Namespace;
            }
            set
            {
                
            }
        }

        public string NodeId
        {
            get
            {
                return Identifier;
            }
            set
            {
                throw new Exception("Can't set node id on node. Property NodeId ");
            }
        }


        protected GraphNode()
        {
            IsNewNode = true;
        }


        public virtual void NodeRemoved(IDiagramNode nodeData)
        {
            foreach (var item in PersistedItems)
            {
                if (item != this)
                    item.NodeRemoved(nodeData);
            }
            this[nodeData.Identifier] = false;
        }
    }
}

