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


            var bindingSet = BindKit.CreateBindingSet(this,new LoginViewModel() );

            bindingSet.Bind(username.Content)
                .For(v => v.Value, v => v.OnValueChanged)
                .To(vm => vm.Username);

            bindingSet.Bind(password.Content)
                .For(v => v.Value, v => v.OnValueChanged)
                .To(vm => vm.Password);

            bindingSet.Bind(loginBtn)
                .For(v=>v.OnClick)
                .To(vm=>vm.Login);

            bindingSet.Bind(registerBtn)
                .For(v => v.OnClick)
                .To(vm => vm.Register);
            
            bindingSet.Build();
        }

        protected override void OnDisposed()
        {
            BindKit.ClearBindingSet(this);
        }
    }
}