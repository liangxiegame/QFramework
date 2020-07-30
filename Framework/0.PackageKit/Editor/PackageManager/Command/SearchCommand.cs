using System.Linq;
using QFramework.PackageKit.State;

namespace QFramework.PackageKit.Command
{
    public class SearchCommand : IPackageManagerCommand
    {
        private readonly string mKey;
        
        [Inject] public PackageManagerModel Model { get; set; }
        
        public SearchCommand(string key)
        {
            mKey = key.ToLower();
        }

        public void Execute()
        {
            var categoryIndex = PackageManagerState.CategoryIndex.Value;
            var categories = PackageManagerState.Categories.Value;
            var accessRightIndex = PackageManagerState.AccessRightIndex.Value;
            
            var repositories = Model
                .Repositories
                .Where(p => p.name.ToLower().Contains(mKey))
                .Where(p=>categoryIndex == 0 || p.type.ToString() == categories[categoryIndex])
                .Where(p=>accessRightIndex == 0 || 
                          accessRightIndex == 1 && p.accessRight == "public" ||
                          accessRightIndex == 2 && p.accessRight == "private"
                )
                .OrderBy(p=>p.name)
                .ToList();

            PackageManagerState.PackageRepositories.Value = repositories;
        }
    }
}