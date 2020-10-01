using System.IO;
using UnityEditor;
using UnityEngine;

namespace QFramework.PackageKit.Command
{
    public class UpdatePackageCommand : Command<PackageKit>
    {
        public UpdatePackageCommand(PackageRepository packageRepository)
        {
            mPackageRepository = packageRepository;
        }

        private readonly PackageRepository mPackageRepository;

        public override void Execute()
        {
            var path = Application.dataPath.Replace("Assets", mPackageRepository.installPath);

            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            RenderEndCommandExecuter.PushCommand(() =>
            {
                AssetDatabase.Refresh();

                PackageApplication.Container.Resolve<PackageKitWindow>().Close();

                SendCommand(new InstallPackage(mPackageRepository));
            });
        }
    }
}