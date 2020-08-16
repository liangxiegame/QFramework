using QFramework.PackageKit.Command;

namespace QFramework.PackageKit
{
    public class PackageManagerConfig : AbstractArchitectureConfig<IPackageManagerCommand,PackageManagerConfig>
    {
        protected override void OnSystemConfig(IQFrameworkContainer systemLayer)
        {
            
        }

        protected override void OnModelConfig(IQFrameworkContainer modelLayer)
        {
            InstalledPackageVersions.Reload();
            modelLayer.RegisterInstance<IPackageManagerModel>(new PackageManagerModel());
            modelLayer.RegisterInstance<IPackageManagerServer>(new PackageManagerServer());
        }

        protected override void OnUtilityConfig(IQFrameworkContainer utilityLayer)
        {
            
        }
    }
}