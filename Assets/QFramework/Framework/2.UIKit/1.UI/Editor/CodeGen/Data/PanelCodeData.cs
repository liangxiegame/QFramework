using System.Collections.Generic;
using Invert.Data;

namespace QFramework
{
    public class PanelCodeData : IDataRecord
    {
        public          string                     PanelName;
        public          Dictionary<string, string> DicNameToFullName = new Dictionary<string, string>();
        public readonly List<MarkedObjInfo>        MarkedObjInfos    = new List<MarkedObjInfo>();
        public readonly List<ElementCodeData>      ElementCodeDatas  = new List<ElementCodeData>();

        public string              Identifier  { get; set; }
        public IRepository         Repository  { get; set; }
        public bool                Changed     { get; set; }
        public IEnumerable<string> ForeignKeys { get; private set; }
    }
}