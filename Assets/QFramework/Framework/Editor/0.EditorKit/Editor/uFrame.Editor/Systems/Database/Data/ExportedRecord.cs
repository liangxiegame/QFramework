using QFramework;
using QFramework.Json;

namespace Invert.Data
{
    public class ExportedRecord
    {
        [JsonProperty]
        public string Identifier { get; set; }
        [JsonProperty]
        public string Data { get; set; }
    }
}