namespace QFramework.PackageKit.Command
{
    public class ImportPackageCommand : IPackageManagerCommand
    {
        private readonly PackageRepository mPackageRepository;

        public ImportPackageCommand(PackageRepository packageRepository)
        {
            mPackageRepository = packageRepository;
        }
        
        public void Execute()
        {
            PackageApplication.Container.Resolve<PackageKitWindow>().Close();
                    
            InstallPackage.Do(mPackageRepository);
        }
    }
}