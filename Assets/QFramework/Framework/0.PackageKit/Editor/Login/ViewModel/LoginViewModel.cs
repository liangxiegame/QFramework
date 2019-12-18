using BindKit.ViewModels;
using UnityEngine;

namespace QFramework.PackageKit
{
    public class LoginViewModel : ViewModelBase
    {
        private string mUsername = string.Empty;

        [Inject]
        public IPackageLoginService Service { get; set; }
        
        public LoginViewModel()
        {
            PackageLoginApp.Container.Inject(this);
        }

        public string Username
        {
            get { return mUsername; }
            set { this.Set(ref mUsername, value, "Username"); }
        }

        private string mPassword = string.Empty;

        public string Password
        {
            get { return mPassword; }
            set { this.Set(ref mPassword, value, "Password"); }
        }

        public void Login()
        {
            Service.DoGetToken(mUsername, mPassword, token =>
            {
                User.Username.Value = mUsername;
                User.Password.Value = mPassword;
                User.Token.Value = token;
                User.Save();

                TypeEventSystem.Send(new LoginSucceed());
            });
        }

        public void Register()
        {
            Application.OpenURL("http://master.liangxiegame.com/user/register");
        }
    }
}