using System.Linq;
using QFramework.PackageKit.State;

namespace QFramework.PackageKit.Command
{
    public class UpdateCategoriesFromModelCommand : IPackageManagerCommand
    {
        [Inject] public PackageManagerModel Model { get; set; }
        
        public void Execute()
        {
            var categories = Model.Repositories.Select(p => p.type).Distinct()
                .Select(t => PackageTypeHelper.TryGetFullName(t))
                .ToList();
            categories.Insert(0, "all");
            PackageManagerState.Categories.Value = categories;
        }
    }
}