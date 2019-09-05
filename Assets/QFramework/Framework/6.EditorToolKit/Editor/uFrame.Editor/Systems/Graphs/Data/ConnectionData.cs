using System.Collections.Generic;
using System.Linq;
using Invert.Data;
using QF.Json;
using QF;

namespace QF.GraphDesigner
{
    public class ConnectionData : IJsonObject, IDataRecord, IDataRecordRemoved
    {

        private string _outputIdentifier;

        private string _inputIdentifier;
        private IConnectable _input;
        private IConnectable _output;

        public ConnectionData(string outputIdentifier, string inputIdentifier)
        {
            OutputIdentifier = outputIdentifier;
            InputIdentifier = inputIdentifier;
        }

        public ConnectionData()
        {
        }

        [JsonProperty, KeyProperty]
        public string OutputIdentifier
        {
            get { return _outputIdentifier; }
            set
            {
                this.Changed("OutputIdentifier",ref  _outputIdentifier, value);
                _output = null;
            }
        }

        [JsonProperty, KeyProperty]
        public string InputIdentifier
        {
            get { return _inputIdentifier; }
            set
            {
                
                this.Changed("InputIdentifier",ref _inputIdentifier, value);
                _input = null;
            }
        }

        public IGraphData Graph { get; set; }

        public IConnectable Output
        {
            get { return _output ?? (_output = GetOutput()); }
        }

        public IConnectable GetOutput(params IConnectableProvider[] ignore)
        {
          
            var result = Repository.GetById<IConnectable>(OutputIdentifier);
            if (result == null)
            {
                foreach (var item in Repository.AllOf<IConnectableProvider>())
                {
                    if (item.Identifier == InputIdentifier) continue;
                    if (ignore.Contains(item)) continue;
                    foreach (var child in item.Connectables)
                    {
                        if (child.Identifier == OutputIdentifier)
                        {
                            return child;
                        }
                    }
                }
            }
            return result;
        }

        public IConnectable Input
        {
            get {
                return _input ?? (_input = GetInput());
            }
        }

        public IConnectable GetInput(params IConnectableProvider[] ignore)
        {
            var result = Repository.GetById<IConnectable>(InputIdentifier);
            if (result == null)
            {
                foreach (var item in Repository.AllOf<IConnectableProvider>())
                {
                    if (item.Identifier == OutputIdentifier) continue;
                    if (ignore.Contains(item)) continue;
                    foreach (var child in item.Connectables)
                    {
                        if (child.Identifier == InputIdentifier)
                        {
                            return child;
                        }
                    }
                }
            }
            return result;
        }

        public void Remove()
        {
            Graph.RemoveConnection(Output, Input);
        }

        public void Serialize(JSONClass cls)
        {
            cls.Add("OutputIdentifier", OutputIdentifier ?? string.Empty);
            cls.Add("InputIdentifier", InputIdentifier ?? string.Empty);
        }

        public void Deserialize(JSONClass cls)
        {
            if (cls["InputIdentifier"] != null)
            {
                InputIdentifier = cls["InputIdentifier"].Value;
            }
            if (cls["OutputIdentifier"] != null)
            {
                OutputIdentifier = cls["OutputIdentifier"].Value;
            }
        }

        public IRepository Repository { get; set; }
        public string Identifier { get; set; }

        public bool Changed { get; set; }

        public IEnumerable<string> ForeignKeys
        {
            get
            {
                yield return InputIdentifier;
                yield return OutputIdentifier;
                
            }
        }

        public void RecordRemoved(IDataRecord record)
        {
            if (record == null) return;

            if (record.Identifier == OutputIdentifier || record.Identifier == InputIdentifier)
                Repository.Remove(this);

            if (OutputIdentifier != null && InputIdentifier != null)
            {
                if (InputIdentifier.StartsWith(record.Identifier) || OutputIdentifier.StartsWith(record.Identifier)) 
                    Repository.Remove(this);
            }
        }
    }    
}