namespace QFramework.PackageKit
{
    public class RegisterView : VerticalLayout
    {
        public RegisterView()
        {
            var usernameLine = new HorizontalLayout().AddTo(this);
            new LabelView("username:").AddTo(usernameLine);
            new TextView("").AddTo(usernameLine);

            var passwordLine = new HorizontalLayout().AddTo(this);

            new LabelView("password:").AddTo(passwordLine);

            new TextView("").PasswordMode().AddTo(passwordLine);

            new ButtonView("注册", () => { }).AddTo(this);


            new ButtonView("返回注册", () => { TypeEventSystem.Send<IPackageLoginCommand>(new OpenRegisterView()); })
                .AddTo(this);
        }
        
    }
}