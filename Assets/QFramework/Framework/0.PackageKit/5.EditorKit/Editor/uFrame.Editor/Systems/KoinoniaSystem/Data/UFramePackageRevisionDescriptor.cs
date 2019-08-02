using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Invert.Data;
using QF.Json;
using QF;

namespace QF.GraphDesigner.Unity.KoinoniaSystem.Data
{
    public class UFramePackageRevisionDescriptor : IDataRecord
    {
        private List<UFramePackageRevisionDescriptor> _dependenciesIds;
        private List<UFramePackageRevisionDescriptor> _dependentRevisionsIds;

        [JsonProperty]
        public string Id { get; set; }        
        
        [JsonProperty]
        public string PackageId { get; set; }
        
        [JsonProperty]
        public string SnapshotUri { get; set; }
        
        [JsonProperty]
        public string VersionTag { get; set; }

        [JsonProperty]
        public List<UFramePackageRevisionDescriptor> DependentRevisionsIds
        {
            get { return _dependentRevisionsIds ?? (_dependentRevisionsIds = new List<UFramePackageRevisionDescriptor>()); }
            set { _dependentRevisionsIds = value; }
        }

        [JsonProperty]
        public List<UFramePackageRevisionDescriptor> DependenciesIds
        {
            get { return _dependenciesIds ?? (_dependenciesIds = new List<UFramePackageRevisionDescriptor>()); }
            set { _dependenciesIds = value; }
        }

        [JsonProperty]
        public bool IsPublic { get; set; }

        [JsonProperty]
        public DateTime CacheExpireTime { get; set; }
   

        public IRepository Repository { get; set; }
        public string Identifier { get; set; }
        public bool Changed { get; set; }
        public IEnumerable<string> ForeignKeys { get { yield break; } }
    }
}
