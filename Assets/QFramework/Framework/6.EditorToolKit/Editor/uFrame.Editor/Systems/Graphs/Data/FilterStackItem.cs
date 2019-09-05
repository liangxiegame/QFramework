using System.Collections.Generic;
using Invert.Data;
using QF.Json;
using QF;

namespace QF.GraphDesigner
{
    public class FilterStackItem : IDataRecord, IDataRecordRemoved
    {
        private string _graphId;
        private string _filterId;
        private int _index;

        public IRepository Repository { get; set; }
        public string Identifier { get; set; }

        public bool Changed { get; set; }

        public IEnumerable<string> ForeignKeys
        {
            get
            {
                yield return GraphId;
                yield return FilterId;
            }
        }

        [JsonProperty, KeyProperty]
        public string GraphId
        {
            get { return _graphId; }
            set
            {
                this.Changed("GraphId", ref  _graphId, value);
            }
        }

        [JsonProperty, KeyProperty]
        public string FilterId
        {
            get { return _filterId; }
            set
            {
                this.Changed("FilterId", ref _filterId, value);
            }
        }

        public IGraphData Graph
        {
            get { return Repository.GetById<IGraphData>(GraphId); }
        }

        public IGraphFilter Filter
        {
            get { return Repository.GetById<IGraphFilter>(FilterId); }
        }

        [JsonProperty]
        public int Index
        {
            get { return _index; }
            set
            {
                this.Changed("Index", ref _index, value);
            }
        }

        public void RecordRemoved(IDataRecord record)
        {
            if (record.Identifier == GraphId || record.Identifier == FilterId)
            {
                Repository.Remove(this);
            }
        }
    }
}