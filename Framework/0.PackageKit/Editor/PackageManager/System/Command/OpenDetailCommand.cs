using UnityEngine;

namespace QFramework.PackageKit.Command
{
    public class OpenDetailCommand : IPackageManagerCommand
    {
        private readonly PackageRepository mPackageRepository;

        public OpenDetailCommand(PackageRepository packageRepository)
        {
            mPackageRepository = packageRepository;
        }

        public void Execute()
        {
            Application.OpenURL("https://qframework.cn/package/detail/" + mPackageRepository.id);
        }
    }
}