using BindKit.ViewModels;

namespace QFramework.PackageKit
{
    public class PacakgeKitLoginViewModel : ViewModelBase
    {
        public PacakgeKitLoginViewModel()
        {
            TypeEventSystem.Register<LoginSucceed>(OnLoginSucceed);
        }


        private bool mInLoginView = true;

        public bool Logined
        {
            get { return User.Logined; }
            set { this.RaisePropertyChanged("Logined");}
        }

        public bool LoginViewVisible
        {
            get { return !User.Logined && mInLoginView; }
            set
            {
                this.RaisePropertyChanged("LoginViewVisible");
                mInLoginView = value;
            }
        }

        public bool RegisterViewVisible
        {
            get { return !User.Logined && !mInLoginView; }
            set
            {
                this.RaisePropertyChanged("RegisterViewVisible");
                mInLoginView = !value;
            }
        }

        void OnLoginSucceed(LoginSucceed loginSucceed)
        {
            Logined = true;
            LoginViewVisible = false;
            RegisterViewVisible = false;
        }

        public void Logout()
        {
            User.Clear();

            Logined = false;
            LoginViewVisible = true;
            RegisterViewVisible = false;
        }

        protected override void Dispose(bool disposing)
        {
            TypeEventSystem.UnRegister<LoginSucceed>(OnLoginSucceed);
        }
    }
}