using System.Collections.Generic;
using Invert.Data;
using UnityEngine;

namespace QFramework.CodeGen
{
    public class FilterItem : IDataRecordRemoved, IFilterItem
    {
        private bool _collapsed;
        private string _nodeId;
        private string _filterId;
        private Vector2 _position;
        public IRepository Repository { get; set; }

        public string Identifier { get; set; }

        public bool Changed { get; set; }
        public IEnumerable<string> ForeignKeys {
            get
            {
                yield return NodeId;
                yield return FilterId;
            }
        }

        public bool Collapsed
        {
            get { return _collapsed; }
            set {
              
                this.Changed("Collapsed",ref _collapsed, value);
            }
        }

        public string NodeId
        {
            get { return _nodeId; }
            set {
                this.Changed("NodeId",ref _nodeId, value);
            }
        }

        public string FilterId
        {
            get { return _filterId; }
            set {
                this.Changed("FilterId", ref _filterId, value);
            }
        }

        public IDiagramNode Node
        {
            get
            {
                return Repository.GetById<IDiagramNode>(NodeId);
            }
        }
        public IGraphFilter Filter
        {
            get
            {
                return Repository.GetById<IGraphFilter>(FilterId);
            }
        }

        public Vector2 Position
        {
            get { return _position; }
            set {

                _position = new Vector2(value.x < 0 ? 0 : value.x, value.y < 0 ? 0 : value.y);
                Changed = true;
            }
        }

        public void RecordRemoved(IDataRecord record)
        {
            if (NodeId == record.Identifier || FilterId == record.Identifier)
                Repository.Remove(this);
            
        }
    }
}