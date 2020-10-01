using System.Linq;
using QFramework.PackageKit.Model;
using QFramework.PackageKit.State;

namespace QFramework.PackageKit.Command
{
    public class UpdateCategoriesFromModelCommand : Command<PackageKit>
    {
        public override void Execute()
        {
            var model = GetModel<IPackageManagerModel>();

            var packageTypeConfigModel = GetModel<IPackageTypeConfigModel>();
            var categories = model.Repositories.Select(p => p.type).Distinct()
                .Select(t => packageTypeConfigModel.GetFullTypeName(t))
                .ToList();
            categories.Insert(0, "all");
            PackageManagerState.Categories.Value = categories;
        }
    }
}