using System.Collections.Generic;

namespace QFramework.PackageKit
{
    public class PackageTypeHelper
    {
        private static Dictionary<string, string> mTypeName2FullName = new Dictionary<string, string>()
        {
            {"fm", "Framework"},
            {"p", "Plugin"},
            {"s", "Shader"},
            {"agt", "Example/Demo"},
            {"master", "Master"},
        };

        public static string TryGetFullName(string typeName)
        {
            if (mTypeName2FullName.ContainsKey(typeName))
            {
                return mTypeName2FullName[typeName];
            }

            return typeName;
        }
    }
}