using System.IO;

namespace QFramework
{
    public static class Preferences
    {
        public const string PATH = "QFramework.properties";

        public static bool HasProperties()
        {
            return File.Exists(PATH);
        }

        public static Properties LoadProperties()
        {
            return new Properties(File.ReadAllText(PATH));
        }

        public static void SaveProperties(Properties properties)
        {
            File.WriteAllText(PATH, properties.ToString());
        }
    }
}