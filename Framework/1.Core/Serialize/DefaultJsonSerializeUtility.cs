using UnityEngine;

namespace QFramework
{
    public class DefaultJsonSerializeUtility : IJsonSerializeUtility
    {
        public string SerializeJson<T>(T obj) where T : class
        {
            return JsonUtility.ToJson(obj, true);
        }

        public T DeserializeJson<T>(string json) where T : class
        {
            return JsonUtility.FromJson<T>(json);
        }
    }
}