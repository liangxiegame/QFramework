using System.Linq;
using QFramework.PackageKit.State;

namespace QFramework.PackageKit.Command
{
    public class UpdateCategoriesFromModelCommand : IPackageManagerCommand
    {
        public void Execute()
        {
            var model = PackageManagerConfig.GetModel<IPackageManagerModel>();
            
            var categories = model.Repositories.Select(p => p.type).Distinct()
                .Select(t => PackageTypeHelper.TryGetFullName(t))
                .ToList();
            categories.Insert(0, "all");
            PackageManagerState.Categories.Value = categories;
        }
    }
}