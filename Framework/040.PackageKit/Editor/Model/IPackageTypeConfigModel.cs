using System.Collections.Generic;

namespace QFramework.PackageKit.Model
{
    public interface IPackageTypeConfigModel : IModel
    {
        string GetFullTypeName(string typeName);
    }

    public class PackageTypeConfigModel : Model<PackageKit>, IPackageTypeConfigModel
    {
        
        private Dictionary<string, string> mTypeName2FullName = new Dictionary<string, string>()
        {
            {"fm", "Framework"},
            {"p", "Plugin"},
            {"s", "Shader"},
            {"agt", "Example/Demo"},
            {"master", "Master"},
        };

        public string GetFullTypeName(string typeName)
        {
            if (mTypeName2FullName.ContainsKey(typeName))
            {
                return mTypeName2FullName[typeName];
            }

            return typeName;
        }
    }
}