using System.Linq;
using QFramework.PackageKit.State;

namespace QFramework.PackageKit.Command
{
    public class PackageManagerInitCommand : IPackageManagerCommand
    {
        [Inject]
        public PackageManagerModel Model { get; set; }
        
        [Inject]
        public IPackageManagerServer Server { get; set; }
        
        public void Execute()
        {
            PackageManagerState.PackageRepositories.Value = Model.Repositories.OrderBy(p => p.name).ToList();

            PackageManagerApp.Send<UpdateCategoriesFromModelCommand>();
            
            Server.GetAllRemotePackageInfoV5((list, categories) =>
            {
                Model.Repositories = list.OrderBy(p=>p.name).ToList();
                PackageManagerState.PackageRepositories.Value = Model.Repositories;
                PackageManagerApp.Send<UpdateCategoriesFromModelCommand>();
            });
        }
    }
}