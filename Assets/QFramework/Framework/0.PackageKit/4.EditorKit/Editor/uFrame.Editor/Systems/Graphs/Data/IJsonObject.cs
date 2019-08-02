using QFramework.Json;

namespace QFramework
{
    public interface IJsonObject
    {
        void Serialize(JSONClass cls);
        void Deserialize(JSONClass cls);
    }
}