#if UNITY_EDITOR
namespace QFramework
{
    internal class LoginView : VerticalLayout,IController
    {

        public LoginView()
        {
            var usernameLine = EasyIMGUI.Horizontal().Parent(this);
            EasyIMGUI.Label().Text("username:").Parent(usernameLine);
            var username = EasyIMGUI.TextField().Parent(usernameLine);

            var passwordLine = EasyIMGUI.Horizontal().Parent(this);
            EasyIMGUI.Label().Text("password:").Parent(passwordLine);
            var password = EasyIMGUI.TextField().PasswordMode().Parent(passwordLine);

            EasyIMGUI.Button()
                .Text("登录")
                .OnClick(() => { this.SendCommand(new LoginCommand(username.Content.Value, password.Content.Value)); })
                .Parent(this);

            EasyIMGUI.Button()
                .Text("注册")
                .OnClick(() => { this.SendCommand<OpenRegisterWebsiteCommand>(); })
                .Parent(this);
        }

        protected override void OnDisposed()
        {

        }

        public IArchitecture GetArchitecture()
        {
            return PackageKitLoginApp.Interface;
        }
    }
}
#endif