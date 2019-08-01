using System.Runtime.Serialization;
using UnityEngine;

namespace Unidux.Util
{
    public class ColorSerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context)
        {
            Color v = (Color) obj;
            info.AddValue("a", v.a);
            info.AddValue("r", v.r);
            info.AddValue("g", v.g);
            info.AddValue("b", v.b);
        }

        public object SetObjectData(
            object obj,
            SerializationInfo info,
            StreamingContext context,
            ISurrogateSelector selector)
        {
            Color v = (Color) obj;
            v.a = (float) info.GetValue("a", typeof(float));
            v.r = (float) info.GetValue("r", typeof(float));
            v.g = (float) info.GetValue("g", typeof(float));
            v.b = (float) info.GetValue("b", typeof(float));
            obj = v;

            return obj;
        }
    }
}