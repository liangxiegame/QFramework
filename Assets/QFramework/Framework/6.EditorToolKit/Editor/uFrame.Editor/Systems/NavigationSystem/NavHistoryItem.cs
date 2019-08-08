using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QF.GraphDesigner;
using Invert.Data;
using QF.Json;
using QF;
using UnityEngine;

namespace QF.GraphDesigner
{
    public class NavHistoryItem : IDataRecord, IItem
    {
        private string _filterIdentifier;
        private string _itemIdentifier;
        private string _title;
        private bool _isActive;
        private DateTime _time;
        private string _workspaceId;
        private string _graphId;
        private Vector2 _scroll;
        public string Identifier { get; set; }
        public IRepository Repository { get; set; }
        public bool Changed { get; set; }
        public IEnumerable<string> ForeignKeys { get { yield break; } }


        [JsonProperty]
        public DateTime Time
        {
            get { return _time; }
            set { this.Changed("Time", ref _time, value); }
        }

        [JsonProperty]
        public bool IsActive
        {
            get { return _isActive; }
            set { this.Changed("IsActive", ref _isActive, value); }
        }

        [JsonProperty]
        public string FilterId
        {
            get { return _filterIdentifier; }
            set { this.Changed("FilterId", ref _filterIdentifier, value); }
        }

        [JsonProperty]
        public string ItemId
        {
            get { return _itemIdentifier; }
            set { this.Changed("ItemId", ref _itemIdentifier, value); }
        }   
        
        [JsonProperty]
        public string WorkspaceId
        {
            get { return _workspaceId; }
            set { this.Changed("WorkspaceId", ref _workspaceId, value); }
        } 
        
        [JsonProperty]
        public string GraphId
        {
            get { return _graphId; }
            set { this.Changed("GraphId", ref _graphId, value); }
        }

        [JsonProperty]
        public Vector2 Scroll
        {
            get { return _scroll; }
            set { this.Changed("Scroll", ref _scroll, value); }

        } 

        public IDiagramNodeItem Item
        {
            get
            {
                return Repository.GetById<IDiagramNode>(ItemId);
            }
        }

        public IGraphFilter Filter
        {
            get
            {
                return Repository.GetById<IGraphFilter>(FilterId);
            }
        }    
        
        public Workspace Workspace
        {
            get
            {
                return Repository.GetSingle<Workspace>(WorkspaceId);
            }
        }

        public IGraphData Graph
        {
            get { return Repository.GetById<IGraphData>(GraphId); }
        }

        public string Title
        {
            get {
                if (string.IsNullOrEmpty(_title))
                {
                    _title = "Rename";
                    if (Filter != null)
                    {
                        _title +=  string.Format("{0} @ {1}", Filter.Name, Graph.Name);
                    }
                }
                return _title;
            }
        }

        public string Group { get; private set; }
        public string SearchTag { get; private set; }
        public string Description { get; set; }
    }

}
