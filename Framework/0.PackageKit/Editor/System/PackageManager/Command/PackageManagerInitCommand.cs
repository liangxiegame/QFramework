using System.Linq;
using QFramework.PackageKit.Model;
using QFramework.PackageKit.State;

namespace QFramework.PackageKit.Command
{
    public class PackageManagerInitCommand : IPackageManagerCommand
    {

        public void Execute()
        {
            var model = PackageKitArchitectureConfig.GetModel<IPackageManagerModel>();
            var server = PackageKitArchitectureConfig.GetModel<IPackageManagerServer>();
            var installedPackageVersionsModel = PackageKitArchitectureConfig.GetModel<IInstalledPackageVersionsConfigModel>();
            installedPackageVersionsModel.Reload();
            
            PackageManagerState.PackageRepositories.Value = model.Repositories.OrderBy(p => p.name).ToList();
            PackageKitArchitectureConfig.SendCommand<UpdateCategoriesFromModelCommand>();
            
            server.GetAllRemotePackageInfoV5((list, categories) =>
            {
                model.Repositories = list.OrderBy(p=>p.name).ToList();
                PackageManagerState.PackageRepositories.Value = model.Repositories;
                PackageKitArchitectureConfig.SendCommand<UpdateCategoriesFromModelCommand>();
            });
        }
    }
}