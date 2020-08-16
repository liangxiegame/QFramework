namespace QFramework
{
    public interface IJsonSerializeUtility : IUtility
    {
        string SerializeJson<T>(T obj) where T : class;

        T DeserializeJson<T>(string json) where T : class;
    }
}