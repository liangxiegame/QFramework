using System.Runtime.Serialization;
using UnityEngine;

namespace Unidux.Util
{
    public class Vector3SerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context)
        {
            Vector3 v = (Vector3) obj;
            info.AddValue("x", v.x);
            info.AddValue("y", v.y);
            info.AddValue("z", v.z);
        }

        public object SetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context,
            ISurrogateSelector selector)
        {
            Vector3 v = (Vector3) obj;
            v.x = (float) info.GetValue("x", typeof(float));
            v.y = (float) info.GetValue("y", typeof(float));
            v.z = (float) info.GetValue("z", typeof(float));
            obj = v;

            return obj;
        }
    }
}