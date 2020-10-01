namespace QFramework.PackageKit
{
    public class PackageKitLoginApp : ArchitectureConfig<PackageKitLoginApp>
    {
        protected PackageKitLoginApp() {}
        
        protected override void OnSystemConfig(IQFrameworkContainer systemLayer)
        {
            
        }

        protected override void OnModelConfig(IQFrameworkContainer modelLayer)
        {
            modelLayer.RegisterInstance<IPackageLoginService>(new PacakgeLoginService());
        }

        protected override void OnUtilityConfig(IQFrameworkContainer utilityLayer)
        {

        }

        protected override void OnLaunch()
        {
        }
    }
}