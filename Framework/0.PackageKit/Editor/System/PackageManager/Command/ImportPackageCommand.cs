namespace QFramework.PackageKit.Command
{
    public class ImportPackageCommand : Command<PackageKit>
    {
        private readonly PackageRepository mPackageRepository;

        public ImportPackageCommand(PackageRepository packageRepository)
        {
            mPackageRepository = packageRepository;
        }

        public override void Execute()
        {
            PackageApplication.Container.Resolve<PackageKitWindow>().Close();

            SendCommand(new InstallPackage(mPackageRepository));
        }
    }
}