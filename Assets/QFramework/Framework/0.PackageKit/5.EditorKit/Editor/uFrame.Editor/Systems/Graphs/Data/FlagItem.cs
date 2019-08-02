using System.Collections.Generic;
using Invert.Data;
using QF.Json;
using QF;

namespace QF.GraphDesigner
{
    public interface IFlagItem
    {
        string Name { get; }
        NodeColor Color { get; }
    }
    public class FlagItem : IDataRecord, IDataRecordRemoved,IFlagItem
    {
        private string _parentIdentifier;
        private string _name;
        public IRepository Repository { get; set; }
        public string Identifier { get; set; }
        public bool Changed { get; set; }

        public IEnumerable<string> ForeignKeys
        {
            get { yield return ParentIdentifier; }
        }

        [JsonProperty, KeyProperty]
        public string ParentIdentifier
        {
            get { return _parentIdentifier; }
            set
            {
                
                this.Changed("ParentIdentifier",ref _parentIdentifier,value);
            }
        }

        [JsonProperty, KeyProperty]
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                Changed = true;
            }
        }

        public NodeColor Color
        {
            get
            {
                return FlagSystem.FlagByName.ContainsKey(Name) ? FlagSystem.FlagByName[Name].Color : NodeColor.Green;
            }
        }

        public void RecordRemoved(IDataRecord record)
        {
            if (record.Identifier == ParentIdentifier)
            {
                Repository.Remove(this);
            }
        }
    }
}