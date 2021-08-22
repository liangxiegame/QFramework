using System.Runtime.Serialization;
using UnityEngine;

namespace Unidux.Util
{
    public class Vector4SerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context)
        {
            Vector4 v = (Vector4) obj;
            info.AddValue("x", v.x);
            info.AddValue("y", v.y);
            info.AddValue("z", v.z);
            info.AddValue("w", v.w);
        }

        public object SetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context,
            ISurrogateSelector selector)
        {
            Vector4 v = (Vector4) obj;
            v.x = (float) info.GetValue("x", typeof(float));
            v.y = (float) info.GetValue("y", typeof(float));
            v.z = (float) info.GetValue("z", typeof(float));
            v.w = (float) info.GetValue("w", typeof(float));
            obj = v;

            return obj;
        }
    }
}