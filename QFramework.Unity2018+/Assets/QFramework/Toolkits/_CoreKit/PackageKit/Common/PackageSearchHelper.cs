using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
namespace QFramework
{
    internal class PackageSearchHelper
    {
        internal static List<PackageRepository>  Search(string key,List<PackageRepository> packageRepositories)
        {
            var categoryIndex = PackageKit.CategoryIndex.Value;
            var categories = PackageKit.Categories.Value;
            var accessRightIndex = PackageKit.AccessRightIndex.Value;
            
            return packageRepositories
                .Where(p => p.name.ToLower().Contains(key))
                .Where(p=>categoryIndex == 0 || p.type.ToString() == categories[categoryIndex])
                .Where(p=>accessRightIndex == 0 || 
                          accessRightIndex == 1 && p.accessRight == "public" ||
                          accessRightIndex == 2 && p.accessRight == "private"
                )
                .OrderBy(p=>p.name)
                .ToList();
        }
    }
}
#endif