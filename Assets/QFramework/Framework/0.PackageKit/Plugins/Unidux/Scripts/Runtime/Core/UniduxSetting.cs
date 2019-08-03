using Unidux.Util;

namespace Unidux
{
    public static class UniduxSetting
    {
        public static ISerializeFactory Serializer = new JsonUtilSerializeFactory();
    }
}