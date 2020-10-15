namespace QFramework.PackageKit
{
    public class LoginView : VerticalLayout
    {
        ControllerNode<PackageKitLoginApp> mController = ControllerNode<PackageKitLoginApp>.Allocate();

        public LoginView()
        {
            var usernameLine = new HorizontalLayout().AddTo(this);
            new LabelView("username:").AddTo(usernameLine);
            var username = new TextView("").AddTo(usernameLine);

            var passwordLine = new HorizontalLayout().AddTo(this);
            new LabelView("password:").AddTo(passwordLine);
            var password = new TextView("").PasswordMode().AddTo(passwordLine);

            EasyIMGUI.Button()
                .Label("登录")
                .OnClick(() =>
                {
                    mController.SendCommand(new LoginCommand(username.Content.Value, password.Content.Value));
                }).AddTo(this);

            EasyIMGUI.Button()
                .Label("注册")
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