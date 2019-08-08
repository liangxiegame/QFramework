using System.Collections.Generic;
using Invert.Data;
using QF.Json;
using QF;

namespace QF.GraphDesigner
{
    public class uFrameDatabaseConfig : IDataRecord, IGraphConfiguration, IAlwaysGenerate
    {
        private string _codeOutputPath;
        private string _ns;
        private int _majorVersion;
        private int _minorVersion;
        private int _buildVersion;
        private int _buildNumber;
        public IRepository Repository { get; set; }
        public string Identifier { get; set; }
        public bool Changed { get; set; }
        public IEnumerable<string> ForeignKeys { get { yield break; } }

        public string Title { get; set; }
        public string Description { get; set; }

        [JsonProperty]
        public string CodeOutputPath
        {
            get { return _codeOutputPath; }
            set { this.Changed("CodeOutputPath", ref _codeOutputPath, value); }
        }

        [JsonProperty]
        public string Namespace
        {
            get { return _ns; }
            set { this.Changed("Namespace", ref _ns, value); }
        }

        public string Group { get { return Title; } }
        public string SearchTag { get { return Title; } }
        
        //       [JsonProperty]
        public bool IsCurrent { get; set; }
        public string FullPath { get; set; }
        public IRepository Database { get; set; }

        [JsonProperty]
        public int MajorVersion
        {
            get { return _majorVersion; }
            set { this.Changed("MajorVersion", ref _majorVersion, value); }
        }

        [JsonProperty]
        public int MinorVersion
        {
            get { return _minorVersion; }
            set { this.Changed("MinorVersion", ref _minorVersion, value); }
        }

        [JsonProperty]
        public int BuildVersion
        {
            get { return _buildVersion; }
            set { this.Changed("BuildVersion", ref _buildVersion, value); }
        }
        [JsonProperty]
        public int BuildNumber
        {
            get { return _buildNumber; }
            set { this.Changed("BuildNumber", ref _buildNumber, value); }
        }
    }
}