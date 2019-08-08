using QF.Json;

namespace QF
{
    public interface IJsonObject
    {
        void Serialize(JSONClass cls);
        void Deserialize(JSONClass cls);
    }
}