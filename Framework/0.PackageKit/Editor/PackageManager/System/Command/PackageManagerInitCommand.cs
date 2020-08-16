using System.Linq;
using QFramework.PackageKit.State;

namespace QFramework.PackageKit.Command
{
    public class PackageManagerInitCommand : IPackageManagerCommand
    {

        public void Execute()
        {
            var model = PackageManagerConfig.GetModel<IPackageManagerModel>();
            var server = PackageManagerConfig.GetModel<IPackageManagerServer>();
            
            PackageManagerState.PackageRepositories.Value = model.Repositories.OrderBy(p => p.name).ToList();
            PackageManagerConfig.SendCommand<UpdateCategoriesFromModelCommand>();
            
            server.GetAllRemotePackageInfoV5((list, categories) =>
            {
                model.Repositories = list.OrderBy(p=>p.name).ToList();
                PackageManagerState.PackageRepositories.Value = model.Repositories;
                PackageManagerConfig.SendCommand<UpdateCategoriesFromModelCommand>();
            });
        }
    }
}