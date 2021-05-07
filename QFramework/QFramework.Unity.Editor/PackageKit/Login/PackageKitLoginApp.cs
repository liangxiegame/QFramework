namespace QFramework
{
    public class PackageKitLoginApp : Architecture<PackageKitLoginApp>
    {
        protected PackageKitLoginApp() {}
        
        protected override void OnSystemConfig()
        {
            
        }

        protected override void OnModelConfig()
        {
            RegisterModel<IPackageLoginService>(new PacakgeLoginService());
        }

        protected override void OnUtilityConfig()
        {

        }

        protected override void OnLaunch()
        {
        }
    }
}