using System.Runtime.Serialization;
using UnityEngine;

namespace Unidux.Util
{
    public class Vector2SerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context)
        {
            Vector2 v2 = (Vector2) obj;
            info.AddValue("x", v2.x);
            info.AddValue("y", v2.y);
        }

        public object SetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context,
            ISurrogateSelector selector)
        {
            Vector2 v2 = (Vector2) obj;
            v2.x = (float) info.GetValue("x", typeof(float));
            v2.y = (float) info.GetValue("y", typeof(float));
            obj = v2;

            return obj;
        }
    }
}