using QFramework.PackageKit.State;

namespace QFramework.PackageKit
{
    public class LogoutCommand : Command<PackageKitLoginApp>
    {
        public override void Execute()
        {
            User.Clear();

            PackageKitLoginState.Logined.Value = false;
            PackageKitLoginState.LoginViewVisible.Value = true;
            PackageKitLoginState.RegisterViewVisible.Value = false;
            
        }
    }
}