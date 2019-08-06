using Invert.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QF.Json;

namespace QF.GraphDesigner
{
    public class InvertGraph : IGraphData, IItem, IJsonTypeResolver, IDataRecordRemoved, ITreeItem
    {
        private List<IDiagramNode> _nodes = new List<IDiagramNode>();

        private string _identifier;

        //private FilterState _filterState = new FilterState();

        private IGraphFilter         _rootFilter;
        private bool                 _errors;
        private List<ConnectionData> _connections;
        private string               _ns;
        private IGraphFilter[]       _filterStack;
        private string               _rootFilterId;
        private bool                 _expanded;
        private uFrameDatabaseConfig _config;
        private bool                 _isDirty;


        public InvertGraph()
        {
        }

        public IEnumerable<IGraphItem> AllGraphItems
        {
            get
            {
                foreach (var node in NodeItems)
                {
                    yield return node;
                    foreach (var item in node.GraphItems)
                        yield return item;
                }
            }
        }


        public IGraphFilter[] FilterStack
        {
            get
            {
                return _filterStack ?? (_filterStack = Repository.All<FilterStackItem>()
                           .Where(p => p.GraphId == this.Identifier)
                           .OrderByDescending(p => p.Index)
                           .Select(p => p.Filter).ToArray()
                       );
            }
            set { _filterStack = value; }
        }

        [JsonProperty]
        public bool IsDirty
        {
            get { return _isDirty; }
            set { this.Changed("IsDirty", ref _isDirty, value); }
        }

        public void PushFilter(IGraphFilter filter)
        {
            var filterStack = new FilterStackItem();
            filterStack.GraphId = this.Identifier;
            filterStack.FilterId = filter.Identifier;
            filterStack.Index = FilterStack.Count();

            Repository.Add(filterStack);
            // Reset the lazy filter stack
            _filterStack = null;


        }

        public void PopFilter()
        {
            if (FilterStack.Length > 0)
                Repository.RemoveAll<FilterStackItem>(p =>
                    p.GraphId == this.Identifier && p.FilterId == CurrentFilter.Identifier);

            _filterStack = null;
        }

        public void PopToFilter(IGraphFilter filter1)
        {
            if (filter1 == null)
            {

                Repository.RemoveAll<FilterStackItem>(p => p.GraphId == this.Identifier && p.FilterId != RootFilterId);

            }
            else
            {
                foreach (var item in FilterStack)
                {
                    if (item != filter1)
                    {
                        var item1 = item;
                        Repository.RemoveAll<FilterStackItem>(p =>
                            p.GraphId == this.Identifier && p.FilterId == item1.Identifier);
                    }
                }
            }

            // Reset the lazy filter stack
            _filterStack = null;
        }

        public void PopToFilterById(string filterId)
        {
            PopToFilter(FilterStack.FirstOrDefault(p => p.Identifier == filterId));
        }

        public IGraphFilter CurrentFilter
        {
            get
            {
                if (FilterStack.Length < 1)
                {
                    return RootFilter;
                }

                return FilterStack.First();
            }
        }


        [JsonProperty]
        public string Identifier
        {
            get { return string.IsNullOrEmpty(_identifier) ? (_identifier = Guid.NewGuid().ToString()) : _identifier; }
            set { _identifier = value; }
        }

        public bool Changed { get; set; }

        public IEnumerable<string> ForeignKeys
        {
            get { yield return RootFilterId; }
        }



        //#if UNITY_EDITOR

        public string SystemPath
        {
            get
            {
                return
                    null; // Path.Combine(Application.dataPath, AssetPath.Substring(7)).Replace("\\", "/").ToLower(); 
            }
            set { }
        }

        //[JsonProperty]
        public string Name
        {
            get
            {
                if (Repository != null && RootFilter != null)
                {
                    return RootFilter.Name;
                }

                return "Problem with:" + this.Identifier;
            }
            set
            {
                if (Repository != null)
                {
                    RootFilter.Name = value;
                }
            }
        }

        //#else
        //    public string SystemPath
        //    {
        //        get { return GraphFileInfo.FullName; }
        //        set
        //        {
        //            GraphFileInfo = new FileInfo(value);
        //        }
        //    }

        //    public string Name
        //    {
        //        get { return System.IO.Path.GetFileNameWithoutExtension(GraphFileInfo.Name); }
        //        set
        //        {
        //            if (GraphFileInfo == null)
        //            {
        //                GraphFileInfo = new FileInfo(value + ".ufgraph");
        //            }
        //            else
        //            {
        //                GraphFileInfo = new FileInfo(System.IO.Path.Combine(GraphFileInfo.Directory.FullName, value + ".ufgraph"));
        //            }
        //        }
        //    }
        //#endif

        public string SystemDirectory
        {
            get { return Path.GetDirectoryName(SystemPath); }
        }

        [JsonProperty]
        public string Version { get; set; }

        public int RefactorCount { get; set; }

        public virtual IEnumerable<IDiagramNode> NodeItems
        {
            get
            {
                // TODO 2.0 Handler RootFilter
                //if (RootFilter is IDiagramNode && !Nodes.Contains(RootFilter as IDiagramNode))
                //    yield return RootFilter as IDiagramNode;

                foreach (var item in Repository.AllOf<IDiagramNode>().Where(p => p.GraphId == this.Identifier))
                    yield return item;
            }
        }

        //public FilterState FilterState
        //{
        //    get { return _filterState; }
        //    set { _filterState = value; }
        //}

        [JsonProperty, KeyProperty]
        public string RootFilterId
        {
            get { return _rootFilterId; }
            set { this.Changed("RootFilterId", ref _rootFilterId, value); }
        }

        public virtual IGraphFilter RootFilter
        {
            get
            {

                if (_rootFilter != null)
                {
                    return _rootFilter;
                }

                if (!string.IsNullOrEmpty(RootFilterId))
                {
                    _rootFilter = Repository.GetById<IGraphFilter>(RootFilterId);
                    var asNode = _rootFilter as IDiagramNode;
                    if (asNode != null)
                    {
                        asNode.GraphId = this.Identifier;
                    }

                    if (_rootFilter == null)
                    {
                        _rootFilter = CreateDefaultFilter(RootFilterId);
                    }

                }
                else
                {
                    RootFilter = CreateDefaultFilter();
                }

                return _rootFilter;
            }
            set
            {
                _rootFilter = value;
                if (value != null)
                {
                    RootFilterId = value.Identifier;
                }

                var asNode = value as IDiagramNode;
                if (asNode != null)
                {
                    asNode.GraphId = this.Identifier;
                    asNode.Name = this.Name;
                }
            }
        }

        public string Title
        {
            get { return Name; }
        }

        public string Group
        {
            get { return "Graphs"; }
        }

        public string SearchTag
        {
            get { return Name; }
        }

        public string Description { get; set; }

        public bool CodeGenDisabled { get; set; }

        public IRepository Repository { get; set; }

        [Obsolete]
        public List<IDiagramNode> Nodes
        {
            get { return _nodes; }
            set { _nodes = value; }
        }

        public uFrameDatabaseConfig Config
        {
            get { return _config ?? (_config = Repository.GetSingle<uFrameDatabaseConfig>()); }
        }

        [InspectorProperty("All the code generated for this graph will be placed inside this namespace.")]
        public string Namespace
        {
            get { return Config.Namespace; }
            set { Config.Namespace = value; }
        }

        public bool Errors
        {
            get { return _errors; }
            set { _errors = value; }
        }

        public Exception Error { get; set; }

#if UNITY_EDITOR

        public string AssetPath
        {
            get { throw new NotImplementedException(); }
        }

        public string AssetDirectory
        {
            get { return System.IO.Path.GetDirectoryName(AssetPath); }
        }

#endif


        public void RemoveItem(IDiagramNodeItem nodeItem)
        {
        }

        public void AddItem(IDiagramNodeItem item)
        {
        }

        public void Save()
        {
            //File.WriteAllText(Path, Serialize().ToString());
        }




        public virtual IGraphFilter CreateDefaultFilter(string identifier = null)
        {
            return null;
        }

        public void Initialize()
        {

        }

        [Obsolete]
        public void AddNode(IDiagramNode data)
        {

        }

        [Obsolete]
        public void RemoveNode(IDiagramNode node, bool removePositionData = true)
        {

        }


        public string Directory
        {
            get { return System.IO.Path.GetDirectoryName(SystemPath); }
        }

        public void AddConnection(IConnectable output, IConnectable input)
        {


            if (!output.AllowMultipleOutputs)
            {
                ClearOutput(output);
            }

            if (!input.AllowMultipleInputs)
            {
                ClearInput(input);
            }

            var connect = new ConnectionData();
            connect.OutputIdentifier = output.Identifier;
            connect.InputIdentifier = input.Identifier;
            Repository.Add(connect);
            output.OnConnectedToInput(input);
            input.OnConnectedFromOutput(output);
        }

        public void AddConnection(string output, string input)
        {
            if (output == null) throw new ArgumentNullException("output");
            if (input == null) throw new ArgumentNullException("input");

            var connect = new ConnectionData();
            connect.OutputIdentifier = output;
            connect.InputIdentifier = input;
            Repository.Add(connect);
        }

        /// <summary>
        /// Removes a connection from this graph
        /// </summary>
        /// <param name="output">The output of the connection.</param>
        /// <param name="input">The input of the connection.</param>
        public void RemoveConnection(IConnectable output, IConnectable input)
        {
            if (output == null) throw new ArgumentNullException("output");
            if (input == null) throw new ArgumentNullException("input");
            Repository.RemoveAll<ConnectionData>(p =>
                p != null && p.OutputIdentifier == output.Identifier && p.InputIdentifier == input.Identifier);

            output.OnOutputConnectionRemoved(input);
            input.OnInputConnectionRemoved(output);
            //        ConnectedItems.RemoveAll(p => p.OutputIdentifier == output.Identifier && p.InputIdentifier == input.Identifier);
        }

        /// <summary>
        /// Removes all connections from an output
        /// </summary>
        /// <param name="output"></param>
        public void ClearOutput(IConnectable output)
        {
            Repository.RemoveAll<ConnectionData>(_ => _.OutputIdentifier == output.Identifier);
            //ConnectedItems.RemoveAll(p => p.OutputIdentifier == output.Identifier);
        }

        /// <summary>
        /// Removes all connections to an input
        /// </summary>
        /// <param name="input"></param>
        public void ClearInput(IConnectable input)
        {
            Repository.RemoveAll<ConnectionData>(_ => _.InputIdentifier == input.Identifier);
            //        ConnectedItems.RemoveAll(p => p.InputIdentifier == input.Identifier);
        }



        public bool Precompiled { get; set; }



        public void CleanUpDuplicates()
        {
            foreach (var nodes in Nodes.GroupBy(p => p.Identifier).ToArray())
            {
                if (nodes.Count() > 1)
                {
                    var identifier = nodes.First();
                    Nodes.Remove(identifier);
                }
            }
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
            foreach (var child in this.NodeItems)
            {
                child.Validate(errors);
            }
        }



        public virtual Type FindType(string clrTypeString)
        {
            var name = clrTypeString.Split(',').FirstOrDefault();
            if (name != null)
            {
                return InvertApplication.FindType(name);
            }

            return null;
            return null;
        }

        public void RecordRemoved(IDataRecord record)
        {
            if (CurrentFilter == record)
            {
                PopFilter();
            }
        }

        public IItem ParentItem
        {
            get { return null; }
        }

        public virtual IEnumerable<IItem> Children
        {
            get { return NodeItems.OfType<IItem>(); }
        }

        [JsonProperty]
        public bool Expanded
        {
            get { return _expanded; }
            set { this.Changed("Expanded", ref _expanded, value); }
        }

        public IEnumerable<IDataRecord> ChildRecords
        {
            get { return NodeItems.OfType<IDataRecord>(); }
        }
    }
}