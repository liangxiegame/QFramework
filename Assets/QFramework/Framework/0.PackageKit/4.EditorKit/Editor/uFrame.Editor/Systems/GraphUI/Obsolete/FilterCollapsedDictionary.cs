using QFramework;
using QFramework.Json;

namespace QFramework.GraphDesigner
{

    public class FilterCollapsedDictionary : FilterDictionary<bool>
    {
        protected override JSONNode SerializeValue(bool value)
        {
            return new JSONData(value);
        }

        protected override bool DeserializeValue(JSONNode value)
        {
            return value.AsBool;
        }
    }
}