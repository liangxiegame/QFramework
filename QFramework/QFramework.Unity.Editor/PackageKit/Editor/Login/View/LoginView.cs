namespace QFramework
{
    public class LoginView : VerticalLayout
    {
        ControllerNode<PackageKitLoginApp> mController = ControllerNode<PackageKitLoginApp>.Allocate();

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
                .OnClick(() => { mController.SendCommand(new LoginCommand(username.Content.Value, password.Content.Value)); })
                .Parent(this);

            EasyIMGUI.Button()
                .Text("注册")
                .OnClick(() => { mController.SendCommand<OpenRegisterWebsiteCommand>(); })
                .Parent(this);
        }

        protected override void OnDisposed()
        {
            mController.Recycle2Cache();
            mController = null;
        }
    }
}