namespace QFramework.PackageKit
{
    public class PackageKitLoginApp : AbstractApp<IPackageLoginCommand>
    {

        protected override void ConfigureService(IQFrameworkContainer container)
        {
            container.RegisterInstance<IPackageLoginService>(new PacakgeLoginService());
        }
    }
}