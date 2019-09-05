using System;
using System.Collections.Generic;
using Invert.Data;
using QF.Json;
using QF;

namespace QF.GraphDesigner
{
    public class TransactionItem : IDataRecord
    {
        private string _dataRecordId;
        private UndoType _type;
        private string _recordType;
        private string _data;
        private string _name;
        private string _group;
        public IRepository Repository { get; set; }
        public string Identifier { get; set; }
        public bool Changed { get; set; }

        public IEnumerable<string> ForeignKeys
        {
            get { yield break; }
        }

        [JsonProperty]
        public DateTime Time { get; set; }

        [JsonProperty]
        public string Group
        {
            get { return _group; }
            set
            {
                _group = value;
                Changed = true;
            }
        }

        [JsonProperty]
        public string DataRecordId
        {
            get { return _dataRecordId; }
            set
            {
                _dataRecordId = value;
                Changed = true;
            }
        }

        [JsonProperty]
        public UndoType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                Changed = true;
            }
        }

        [JsonProperty]
        public string RecordType
        {
            get { return _recordType; }
            set
            {
                _recordType = value;
                Changed = true;
            }
        }

        [JsonProperty]
        public string Data
        {
            get { return _data; }
            set
            {
                _data = value;
                Changed = true;
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                Changed = true;
            }
        }
    }
}