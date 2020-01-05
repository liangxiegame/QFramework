namespace QFramework.PackageKit
{
   public class PackageLoginView : VerticalLayout, IPackageKitView
    {
        public IQFrameworkContainer Container { get; set; }

        PackageLoginApp mPackageLoginApp = new PackageLoginApp();

        public int RenderOrder
        {
            get { return 3; }
        }

        public bool Ignore { get; private set; }

        public bool Enabled
        {
            get { return true; }
        }

        public void Init(IQFrameworkContainer container)
        {
            var expendLayout = new TreeNode(false, LocaleText.UserInfo)
                .AddTo(this);

            var boxLayout = new VerticalLayout("box");

            expendLayout.Add2Spread(boxLayout);
            
            var logoutBtn = new ButtonView("注销").AddTo(boxLayout);

            var loginView = new LoginView().AddTo(boxLayout);
            var registerView = new RegisterView().AddTo(boxLayout);

            
            var bindingSet = BindKit.CreateBindingSet(this, new PacakgeKitLoginViewModel());
            
            bindingSet.Bind(logoutBtn).For(v => v.Visible).To(vm => vm.Logined).OneWay();
            bindingSet.Bind(logoutBtn).For(v => v.OnClick).To(vm => vm.Logout);
            bindingSet.Bind(loginView).For(v=>v.Visible).To(vm=>vm.LoginViewVisible).OneWay();
            bindingSet.Bind(registerView).For(v => v.Visible).To(vm => vm.RegisterViewVisible).OneWay();
            
            bindingSet.Build();
        }
        

        void IPackageKitView.OnUpdate()
        {
        }

        public void OnGUI()
        {
            DrawGUI();
        }


        public class LocaleText
        {
            public static string UserInfo
            {
                get { return Language.IsChinese ? "用户信息" : "User Info"; }
            }
        }

        public void OnDispose()
        {
            mPackageLoginApp.Dispose();
            mPackageLoginApp = null;
        }
    }
}