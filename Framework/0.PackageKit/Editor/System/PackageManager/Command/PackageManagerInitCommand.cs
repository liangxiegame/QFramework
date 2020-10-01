using System.Linq;
using QFramework.PackageKit.Model;
using QFramework.PackageKit.State;

namespace QFramework.PackageKit.Command
{
    public class PackageManagerInitCommand : Command<PackageKit>
    {

        public override void Execute()
        {
            var model = GetModel<IPackageManagerModel>();
            var server = GetModel<IPackageManagerServer>();
            var installedPackageVersionsModel = GetModel<IInstalledPackageVersionsConfigModel>();
            installedPackageVersionsModel.Reload();
            
            PackageManagerState.PackageRepositories.Value = model.Repositories.OrderBy(p => p.name).ToList();
            SendCommand<UpdateCategoriesFromModelCommand>();
            
            server.GetAllRemotePackageInfoV5((list, categories) =>
            {
                model.Repositories = list.OrderBy(p=>p.name).ToList();
                PackageManagerState.PackageRepositories.Value = model.Repositories;
                SendCommand<UpdateCategoriesFromModelCommand>();
            });
        }
    }
}