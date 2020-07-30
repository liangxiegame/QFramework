using QFramework.PackageKit.State;

namespace QFramework.PackageKit
{
    public class LoginCommand : IPackageLoginCommand
    {
        private readonly string mUsername;
        private readonly string mPassword;

        public LoginCommand(string username, string password)
        {
            mUsername = username;
            mPassword = password;
        }

        [Inject]
        public IPackageLoginService Service { get; set; }
        
        public void Execute()
        {
            Service.DoGetToken(mUsername, mPassword, token =>
            {
                User.Username.Value = mUsername;
                User.Password.Value = mPassword;
                User.Token.Value = token;
                User.Save();

                PackageKitLoginState.Logined.Value = true;
                PackageKitLoginState.LoginViewVisible.Value = false;
                PackageKitLoginState.RegisterViewVisible.Value = false;
            });
        }
    }
}