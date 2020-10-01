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
            
            var loginBtn = new ButtonView("登录").AddTo(this);
            var registerBtn = new ButtonView("注册").AddTo(this);

            loginBtn.OnClick.AddListener(() =>
            {
                mController.SendCommand(new LoginCommand(username.Content.Value,password.Content.Value));
            });

            registerBtn.OnClick.AddListener(() =>
            {
                mController.SendCommand<OpenRegisterWebsiteCommand>();
            });
        }

        protected override void OnDisposed()
        {
            mController.Recycle2Cache();
            mController = null;
        }
    }
}