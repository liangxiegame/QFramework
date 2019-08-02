using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QF.Json;
using QF;

namespace QF.GraphDesigner.Unity.KoinoniaSystem.Data
{
    public class UFramePackagePreviewDescriptor
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
        public string ProjectPreviewIconUrl { get; set; }
        [JsonProperty]
        public string LatestPublicVersionTag { get; set; }

    }
}
