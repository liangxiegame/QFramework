namespace QFramework.PackageKit
{
    public class RegisterView : VerticalLayout
    {
        ControllerNode<PackageKitLoginApp> mControllerNode = ControllerNode<PackageKitLoginApp>.Allocate();

        public RegisterView()
        {
            var usernameLine = new HorizontalLayout().AddTo(this); 
            
            EasyIMGUI.Label().Text("username:").AddTo(usernameLine);
            new TextView("").AddTo(usernameLine);

            var passwordLine = new HorizontalLayout().AddTo(this);

            EasyIMGUI.Label().Text("password:").AddTo(passwordLine);

            new TextView("").PasswordMode().AddTo(passwordLine);

            EasyIMGUI.Button()
                .Text("注册")
                .OnClick(() => { })
                .AddTo(this);

            EasyIMGUI.Button()
                .Text("返回注册")
                .OnClick(() => { mControllerNode.SendCommand(new OpenRegisterViewCommand()); })
                .AddTo(this);
        }

        protected override void OnDisposed()
        {
            mControllerNode = null;
        }
    }
}