using System;

namespace Unidux.Util
{
    public interface ISerializeFactory
    {
        object Deserialize(byte[] raw, Type type);
        byte[] Serialize(object raw);
    }
}