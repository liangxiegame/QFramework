using System.Collections.Generic;

namespace QFramework.PackageKit.State
{
    public class PackageManagerState
    {
        public static Property<List<PackageRepository>> PackageRepositories =
            new Property<List<PackageRepository>>(new List<PackageRepository>());
        
        public static Property<int> CategoryIndex = new Property<int>(0);
        
        public static Property<List<string>> Categories = new Property<List<string>>();
        
        public static Property<int> AccessRightIndex = new Property<int>(0);
        
        public static Property<string> SearchKey = new Property<string>("");
    }
}