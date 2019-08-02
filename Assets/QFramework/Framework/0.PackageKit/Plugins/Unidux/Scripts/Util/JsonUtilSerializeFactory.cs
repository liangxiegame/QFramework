using System;
using System.Text;
using UnityEngine;

namespace Unidux.Util
{
    public class JsonUtilSerializeFactory : ISerializeFactory
    {
        public object Deserialize(byte[] raw, Type type)
        {
            var text = Encoding.UTF8.GetString(raw);
            return JsonUtility.FromJson(text, type);
        }

        public byte[] Serialize(object raw)
        {
            var text = JsonUtility.ToJson(raw);
            return Encoding.UTF8.GetBytes(text);
        }
    }
}