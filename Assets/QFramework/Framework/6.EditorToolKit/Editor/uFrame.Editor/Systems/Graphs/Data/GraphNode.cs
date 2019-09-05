using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using System.Text.RegularExpressions;
using QF.GraphDesigner;
using Invert.Data;
using QF.Json;
using QF;
using UnityEngine;
namespace QF.GraphDesigner
{

    /// <summary>
    /// The base data class for all diagram nodes.
    /// </summary>
    public abstract class GraphNode : IDiagramNode, IGraphFilter, IDataRecordRemoved, ITreeItem
    {
        public virtual IEnumerable<FilterItem> FilterLocations
        {
            get { return Repository.All<FilterItem>().Where(p => p.NodeId == this.Identifier); }
        }
        [JsonProperty]
        public int Order
        {
            get { return _order; }
            set
            {
                this.Changed("Order", ref _order, value);
            }
        }

        public IEnumerable<IFlagItem> DisplayedFlags
        {
            get
            {
                yield break;
            }
        }

        public virtual IFilterItem FilterLocation
        {
            get { return FilterLocations.FirstOrDefault(); }
        }

        public virtual IGraphFilter Filter
        {
            get
            {
                var fl = FilterLocation;
                if (fl == null) return null;
                return fl.Filter;
            }
        }
        public virtual TType InputFrom<TType>()
        {
            return this.InputsFrom<TType>().FirstOrDefault();
        }

        public virtual IEnumerable<TType> InputsFrom<TType>()
        {
            var filterItem = this as IFilterItem;
            if (filterItem != null)
            {
                foreach (var item in Inputs)
                {
                    var outId = item.OutputIdentifier;
                    var output = filterItem.Filter.AllGraphItems().FirstOrDefault(p => p is TType && p.Identifier == outId);

                    if (output != null)
                    {
                        yield return (TType)output;
                    }
                }
            }

            foreach (var item in Inputs.Select(p => p.GetOutput(this as IConnectableProvider)).OfType<TType>())
                yield return item;
        }

        public virtual IEnumerable<TType> OutputsTo<TType>()
        {
            var filterItem = this as IFilterItem;
            if (filterItem != null)
            {
                foreach (var item in Outputs)
                {
                    var inputIdentifier = item.InputIdentifier;
                    var input = filterItem.Filter.AllGraphItems().FirstOrDefault(p => p is TType && p.Identifier == inputIdentifier);

                    if (input != null)
                    {
                        yield return (TType)input;
                    }
                }
            }

            foreach (var item in Outputs.Select(p => p.GetInput(this as IConnectableProvider)).OfType<TType>())
                yield return item;
        }

        public virtual TType OutputTo<TType>()
        {
            return this.OutputsTo<TType>().FirstOrDefault();
        }

        public override string ToString()
        {
            return Name;
        }

        public IEnumerable<ConnectionData> Inputs
        {
            get
            {
                foreach (var connectionData in Repository.All<ConnectionData>())
                {
                    if (connectionData.InputIdentifier == this.Identifier)
                    {
                        yield return connectionData;
                    }
                }
            }
        }

        public IEnumerable<ConnectionData> Outputs
        {
            get
            {

                foreach (var connectionData in Repository.All<ConnectionData>())
                {
                    if (connectionData.OutputIdentifier == this.Identifier)
                    {
                        yield return connectionData;
                    }
                }
            }
        }

        public virtual bool AllowInputs
        {
            get { return true; }
        }

        public virtual bool AllowOutputs
        {
            get { return true; }
        }

        public virtual string InputDescription { get { return null; } }
        public virtual string OutputDescription { get { return null; } }

        public virtual bool AllowMultipleInputs
        {
            get { return true; }
        }

        public virtual bool AllowMultipleOutputs
        {
            get { return true; }
        }
#if !SERVER
        public virtual Color Color
        {
            get { return CachedStyles.GetColor(NodeConfig.GetColor(this)); }
        }
#endif
        public virtual NodeConfigBase NodeConfig
        {
            get
            {
                return _nodeConfig ?? (
                  _nodeConfig = InvertApplication.Container.GetNodeConfig(GetType()) as NodeConfigBase);
            }
        }

        public virtual void OnConnectionApplied(IConnectable output, IConnectable input)
        {

        }

        public virtual bool CanOutputTo(IConnectable input)
        {
            if (!AllowMultipleOutputs && this.Outputs.Any())
            {
                return false;
            }
            return true;
        }

        public virtual bool CanInputFrom(IConnectable output)
        {
            if (!AllowMultipleInputs && this.Inputs.Any())
            {
                return false;
            }
            return true;
        }

        public virtual void OnOutputConnectionRemoved(IConnectable input)
        {

        }

        public virtual void OnInputConnectionRemoved(IConnectable output)
        {

        }

        public virtual void OnConnectedToInput(IConnectable input)
        {

        }

        public virtual void OnConnectedFromOutput(IConnectable output)
        {

        }



        private IGraphData _data;




        private string _identifier;

        private bool _isCollapsed;

        private Vector2 _location = new Vector2(45f, 45f);

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


        [JsonProperty, InspectorProperty("Comments associated with this node.", InspectorType.TextArea)]
        public virtual string Comments
        {
            get { return _comments; }
            set { this.Changed("Comments", ref  _comments, value); }
        }

        [Browsable(false)]
        public virtual bool IsValid
        {
            get { return Validate().Count < 1; }
        }

        /// <summary>
        /// Gets a list of errors about this node or its children
        /// </summary>
        /// <returns></returns>
        public List<ErrorInfo> Validate()
        {
            var list = new List<ErrorInfo>();
            Validate(list);
            return list;
        }

        /// <summary>
        /// Validates this node decorating a list of errors
        /// </summary>
        /// <param name="errors"></param>
        public virtual void Validate(List<ErrorInfo> errors)
        {
            
       
            //foreach (var child in this.PersistedItems)
            //{
            //    child.Validate(errors);
            //}

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
            set
            {

            }
        }
        [Browsable(false)]
        public virtual IEnumerable<IGraphItem> GraphItems
        {
            get { return PersistedItems.OfType<IGraphItem>(); }
        }
        [Browsable(false)]
        public Type CurrentType
        {
            get
            {
                return InvertApplication.FindType(FullName);
            }
        }

        [Browsable(false)]
        public Vector2 DefaultLocation
        {
            get { return _location; }
        }

        [JsonProperty, KeyProperty]
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
        public virtual IGraphData Graph
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
        [Obsolete]
        public bool Dirty { get; set; }


        //[Browsable(false)]
        //public FlagsDictionary Flags
        //{
        //    get { return _flags ?? (_flags = new FlagsDictionary()); }
        //    set { _flags = value; }
        //}
        [Browsable(false)]
        public string FullLabel { get { return Name; } }
        //[InspectorProperty]
        public virtual string FullName
        {
            get
            {
                if (!string.IsNullOrEmpty(Namespace))
                    return string.Format("{0}.{1}", Namespace, Name);

                return Name;
            }
            set { }
        }

  
        public Rect HeaderPosition { get; set; }

        public string Highlighter { get { return null; } }

        public IRepository Repository { get; set; }

        [InspectorProperty]
        public virtual string Identifier
        {
            get { return string.IsNullOrEmpty(_identifier) ? (_identifier = Guid.NewGuid().ToString()) : _identifier; }
            set { _identifier = value; }
        }

        public bool Changed { get; set; }

        public virtual IEnumerable<string> ForeignKeys
        {
            get { yield return GraphId; }
        }


        public virtual bool ImportedOnly
        {
            get { return true; }
        }

        public bool IsExplorerCollapsed { get; set; }

        public virtual string InfoLabel
        {
            get
            {
                return null;
            }
        }

  
        public bool IsEditing { get; set; }

        public bool IsExternal
        {
            get
            {
                return Repository.AllOf<IDiagramNode>().All(p => p.Identifier != Identifier);
            }
        }

        public bool IsNewNode { get; set; }

        public bool IsSelectable { get { return true; } }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                //this.Changed("IsSelected", _isSelected, value);
                _isSelected = value;
                this.Changed = true;
            }
        }

        public virtual IEnumerable<IDiagramNodeItem> DisplayedItems
        {
            get { return PersistedItems; }
        }

        public abstract string Label { get; }

        public bool Precompiled
        {
            get;
            set;
        }

        [JsonProperty, InspectorProperty]
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

        public virtual bool UseStraightLines
        {
            get { return false; }
        }

        public virtual IEnumerable<IDiagramNode> FilterNodes
        {
            get
            {
                foreach (var item in this.Repository.All<FilterItem>().Where(p => p.FilterId == Identifier))
                {
                    var node = item.Node;
                    if (node == null)
                    {
                        this.Repository.Remove(item);
                        InvertApplication.Log(string.Format("Filter item node is null {0}, Cleaning..", item.NodeId));
                        continue;
                    }
                    //if (item == null) continue;
                    yield return node;
                }
            }
        }

        public virtual  IEnumerable<IFilterItem> FilterItems
        {
            get
            {
                if (!FilterExtensions.AllowedFilterNodes.ContainsKey(GetType())) yield break;
                var found = false;
                foreach (FilterItem p in Repository.All<FilterItem>())
                {
                    if (p.FilterId == Identifier && p.NodeId == Identifier)
                    {
                        found = true;
                    }
                    if (p.FilterId == Identifier) yield return p;

                }
                if (!found && FilterExtensions.AllowedFilterNodes.ContainsKey(GetType()))
                {
                    var filterItem = new FilterItem();
                    filterItem.FilterId = Identifier;
                    filterItem.NodeId = Identifier;
                    Repository.Add(filterItem);
                    yield return filterItem;
                }
            }
        }

        public virtual bool AllowExternalNodes
        {
            get { return true; }
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
                return this.Identifier;
            }
            set
            {
                throw new Exception("Can't set node id on node. Property NodeId ");
            }
        }

        [Browsable(false)]
        public virtual GraphNode Node
        {
            get
            {
                return this;
            }
            set
            {
            }
        }

        public virtual string OldName
        {
            get;
            set;
        }


        public string Group
        {
            get
            {
                if (Graph == null) return Name;
                return Graph.Name;
            }
        }

        [Browsable(false)]
        public string SearchTag { get { return Name; } }

        public virtual string Description { get; set; }

        [Browsable(false)]
        public virtual bool ShouldRenameRefactor { get { return true; } }


        [Browsable(false)]
        public virtual string SubTitle { get { return string.Empty; } }
        [Browsable(false)]
        public virtual string Title { get { return Name; } }

        protected GraphNode()
        {
            IsNewNode = true;
        }

        public virtual void BeginEditing()
        {

            OldName = Name;
            IsEditing = true;
        }

        public void BeginRename()
        {
            BeginEditing();
        }


        public bool EnsureUniqueNames { get; set; }
        public virtual bool EndEditing()
        {
            IsEditing = false;
            return true;
        }

        public virtual CodeTypeReference GetFieldType(ITypedItem itemData)
        {
            var tRef = new CodeTypeReference(this.Name);
            //tRef.TypeArguments.Add(this.Name);
            return tRef;
        }

        public virtual void NodeAddedInFilter(IDiagramNode newNodeData)
        {

        }


        public virtual void NodeItemRemoved(IDiagramNodeItem diagramNodeItem)
        {


        }

        public virtual void NodeAdded(IDiagramNode data)
        {

        }

        public virtual void NodeItemAdded(IDiagramNodeItem data)
        {
   
        }


        public virtual CodeTypeReference GetPropertyType(ITypedItem itemData)
        {
            return new CodeTypeReference(this.Name);
        }

        public virtual void MoveItemDown(IDiagramNodeItem nodeItem)
        {
        }

        public virtual void MoveItemUp(IDiagramNodeItem nodeItem)
        {

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

        void IDiagramNodeItem.NodeItemRemoved(IDiagramNodeItem nodeItem)
        {

            NodeItemRemoved(nodeItem);

        }

        public virtual void RefactorApplied()
        {

        }

        public void Remove(IDiagramNode diagramNode)
        {
            Repository.Remove(diagramNode);
        }

        public virtual void RemoveFromDiagram()
        {
            //Data.RefactorCount--;
        }


        public void Rename(IDiagramNode data, string name)
        {
            Rename(name);
        }

        public virtual void Rename(string newName)
        {
            Name = newName;
        }



        public void RemoveItem(IDiagramNodeItem item)
        {

        }
        
        public virtual string RelatedType
        {
            get { return this.Identifier; }
            set
            {

            }
        }
        
        public virtual string RelatedTypeName
        {
            get { return this.Name; }
            set { throw new NotImplementedException(); }
        }

        public void RemoveType()
        {

        }

        public string AssemblyQualifiedName { get; set; }

        public CodeTypeReference GetFieldType()
        {
            return new CodeTypeReference(this.Name);
        }

        public CodeTypeReference GetPropertyType()
        {
            return new CodeTypeReference(this.Name);
        }

        public ErrorInfo[] Errors { get; set; }

        public virtual void RecordRemoved(IDataRecord record)
        {
            if (record.Identifier == GraphId)
                Repository.Remove(this);

        }

        public IItem ParentItem
        {
            get { return this.Graph; }
        }

        public IEnumerable<IItem> Children
        {
            get { return FilterItems.Select(p=>p.Node).OfType<IItem>().Except(new []{this}); }
        }

        [JsonProperty]
        public virtual bool Expanded
        {
            get { return _expanded; }
            set
            {
                this.Changed("Expanded", ref _expanded, value);
            }
        }

        public IEnumerable<IDataRecord> ChildRecords
        {
	        get { return this.GraphItems.OfType<IDataRecord>(); }
        }
    }
}

