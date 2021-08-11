using System.Collections.Generic;

namespace QFramework
{
    public class PanelCodeInfo
    {
        public          string                     GameObjectName;
        public          Dictionary<string, string> DicNameToFullName = new Dictionary<string, string>();
        public readonly List<BindInfo>             BindInfos         = new List<BindInfo>();
        public readonly List<ElementCodeInfo>      ElementCodeDatas  = new List<ElementCodeInfo>();

        public string              Identifier  { get; set; }
        public bool                Changed     { get; set; }
        public IEnumerable<string> ForeignKeys { get; private set; }
    }
}