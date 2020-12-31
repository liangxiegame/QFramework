using UnityEngine;

namespace QFramework
{
    public class DefaultJsonSerializer : IJsonSerializer
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