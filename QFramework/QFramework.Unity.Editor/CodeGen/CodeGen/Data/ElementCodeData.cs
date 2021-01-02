using System.Collections.Generic;

namespace QFramework
{
    public class ElementCodeInfo
    {
        public          BindInfo                   BindInfo;
        public          string                     BehaviourName;
        public          Dictionary<string, string> DicNameToFullName = new Dictionary<string, string>();
        public readonly List<BindInfo>             BindInfos         = new List<BindInfo>();
        public readonly List<ElementCodeInfo>      ElementCodeDatas  = new List<ElementCodeInfo>();
    }
}