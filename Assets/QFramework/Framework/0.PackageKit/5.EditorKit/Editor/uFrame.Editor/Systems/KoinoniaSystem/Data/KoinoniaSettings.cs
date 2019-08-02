using System;
using System.Collections.Generic;
using Invert.Data;
using QF.Json;
using QF;

namespace QF.GraphDesigner.Unity.KoinoniaSystem
{
    public class KoinoniaSettings : IDataRecord
    {
        private List<string> _packagesToUninstall;
        private List<string> _packagesToInstall;

        [JsonProperty]
        public string AccessToken { get; set; }

        [JsonProperty]
        public DateTime AccessTokenExpirationDate { get; set; }

        public IRepository Repository { get; set; }
        public string Identifier { get; set; }
        public bool Changed { get; set; }
        public IEnumerable<string> ForeignKeys { get { yield break; }}

        [JsonProperty]
        public List<string> PackagesToUninstall
        {
            get { return _packagesToUninstall ?? (_packagesToUninstall = new List<string>()); }
            set { _packagesToUninstall = value; }
        }

        [JsonProperty]
        public List<string> PackagesToInstall
        {
            get { return _packagesToInstall ?? (_packagesToInstall = new List<string>()); }
            set { _packagesToInstall = value; }
        }
    }
}