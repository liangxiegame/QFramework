namespace QFramework.PackageKit
{
    public class LoginView : VerticalLayout
    {
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
                PackageKitLoginApp.Send(new LoginCommand(username.Content.Value,password.Content.Value));
            });

            registerBtn.OnClick.AddListener(() =>
            {
                PackageKitLoginApp.Send<OpenRegisterWebsiteCommand>();
            });
        }

        protected override void OnDisposed()
        {
        }
    }
}