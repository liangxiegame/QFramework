namespace QFramework
{
    public class LoginView : VerticalLayout
    {
        ControllerNode<PackageKitLoginApp> mController = ControllerNode<PackageKitLoginApp>.Allocate();

        public LoginView()
        {
            var usernameLine = new HorizontalLayout().AddTo(this);
            EasyIMGUI.Label().Text("username:").AddTo(usernameLine);
            var username = EasyIMGUI.TextField().AddTo(usernameLine);

            var passwordLine = new HorizontalLayout().AddTo(this);
            EasyIMGUI.Label().Text("password:").AddTo(passwordLine);
            var password = EasyIMGUI.TextField().PasswordMode().AddTo(passwordLine);

            EasyIMGUI.Button()
                .Text("登录")
                .OnClick(() => { mController.SendCommand(new LoginCommand(username.Content.Value, password.Content.Value)); })
                .AddTo(this);

            EasyIMGUI.Button()
                .Text("注册")
                .OnClick(() => { mController.SendCommand<OpenRegisterWebsiteCommand>(); })
                .AddTo(this);
        }

        protected override void OnDisposed()
        {
            mController.Recycle2Cache();
            mController = null;
        }
    }
}