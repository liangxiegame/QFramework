using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Invert.Data;
using QF.Json;
using QF;

namespace QF.GraphDesigner.Unity.KoinoniaSystem.Data
{
    public class UFramePackageDescriptor : IDataRecord 
    {
        [JsonProperty]
        public string Id { get; set; }

        [JsonProperty]
        public string Title { get; set; }
        [JsonProperty]

        public string Description { get; set; }
[JsonProperty]

        public UFramePackageManagementType ManagementType { get; set; }
[JsonProperty]

        public string ProjectIconUrl { get; set; }

//        public string Code; // done using based on Title            
//        public string Slug { get; set; }
        [JsonProperty]
        public List<string> RevisionIds { get; set; }

        [JsonProperty]

        public string RepositoryUrl { get; set; }
[JsonProperty]
        
        public bool IsPublic { get; set; }
[JsonProperty]
        
        public PackageState State { get; set; }

        public override bool Equals(object obj)
        {
            var package = obj as UFramePackageDescriptor;
            if (package != null) return package.Id == Id;
            return false;
        }

        public IRepository Repository { get; set; }

        public string Identifier
        {
            get { return Id; }
            set { }
        }

        public bool Changed { get; set; }
        public IEnumerable<string> ForeignKeys {
            get { yield break; }
        }

        [JsonProperty]
        public DateTime CacheExpireTime { get; set; }
    }
}
