using UnityEngine;

namespace QFramework.PackageKit.Command
{
    public class OpenDetailCommand : Command<PackageKit>
    {
        private readonly PackageRepository mPackageRepository;

        public OpenDetailCommand(PackageRepository packageRepository)
        {
            mPackageRepository = packageRepository;
        }

        public override void Execute()
        {
            Application.OpenURL("https://qframework.cn/package/detail/" + mPackageRepository.id);
        }
    }
}