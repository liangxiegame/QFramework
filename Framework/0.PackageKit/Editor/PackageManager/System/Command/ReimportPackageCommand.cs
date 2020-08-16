using System.IO;
using UnityEditor;
using UnityEngine;

namespace QFramework.PackageKit.Command
{
    public class ReimportPackageCommand : IPackageManagerCommand
    {
        private readonly PackageRepository mPackageRepository;

        public ReimportPackageCommand(PackageRepository mPackageRepository)
        {
            this.mPackageRepository = mPackageRepository;
        }

        public void Execute()
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

                InstallPackage.Do(mPackageRepository);
            });
        }
    }
}