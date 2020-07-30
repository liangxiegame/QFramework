
using QFramework.PackageKit.State;

namespace QFramework.PackageKit
{
    public class OpenRegisterViewCommand : IPackageLoginCommand
    {
        public void Execute()
        {
            PackageKitLoginState.InLoginView.Value = false;
        }
    }
}