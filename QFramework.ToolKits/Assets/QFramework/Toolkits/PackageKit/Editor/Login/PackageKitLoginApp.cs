namespace QFramework
{
    internal class PackageKitLoginApp : Architecture<PackageKitLoginApp>
    {
        protected override void Init()
        {
            RegisterModel<IPackageLoginService>(new PacakgeLoginService());
        }
    }
}