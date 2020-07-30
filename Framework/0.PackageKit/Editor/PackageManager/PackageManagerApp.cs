using QFramework.PackageKit.Command;

namespace QFramework.PackageKit
{
    public class PackageManagerApp : AbstractApp<IPackageManagerCommand>
    {
        protected override void ConfigureService(IQFrameworkContainer container)
        {
            InstalledPackageVersions.Reload();

            // 注册好 model
            var model = new PackageManagerModel
            {
                Repositories = PackageInfosRequestCache.Get().PackageRepositories
            };

            Container.RegisterInstance(model);

            Container.Register<IPackageManagerServer, PackageManagerServer>();
        }
    }
}