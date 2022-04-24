namespace Photon.Voice.Unity
{
    using System.Linq;

    // this was added for backwards compatibility
    public static class AudioInEnumeratorEx
    {
        public static bool IDIsValid(this IDeviceEnumerator en, int id)
        {
            return en.Any(d => d.IDInt == id);
        }

        public static string NameAtIndex(this IDeviceEnumerator en, int index)
        {
            return en.ElementAtOrDefault(index).Name;
        }

        public static int IDAtIndex(this IDeviceEnumerator en, int index)
        {
            if (index >= 0 && index < en.Count())
            {
                return en.ElementAt(index).IDInt;
            }
            return -1;
        }
    }
}

