using System.Collections.Generic;

namespace QFramework
{
    public class ElementCodeData
    {
        public          MarkedObjInfo              MarkedObjInfo;
        public          string                     BehaviourName;
        public          Dictionary<string, string> DicNameToFullName = new Dictionary<string, string>();
        public readonly List<MarkedObjInfo>        MarkedObjInfos    = new List<MarkedObjInfo>();
        public readonly List<ElementCodeData>      ElementCodeDatas  = new List<ElementCodeData>();
    }

}