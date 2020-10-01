
using QFramework.PackageKit.State;

namespace QFramework.PackageKit
{
    public class OpenRegisterViewCommand : Command<PackageKitLoginApp>
    {
        public override void Execute()
        {
            PackageKitLoginState.InLoginView.Value = false;
        }
    }
}