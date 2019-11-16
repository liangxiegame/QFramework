using QFramework;
using UniRx;
using UnityEngine;

namespace QF.Editor
{
    public class LoginView : VerticalLayout
    {
        public string Username = "";
        public string Password = "";

        public LoginView()
        {
            var usernameLine = new HorizontalLayout().AddTo(this);
            new LabelView("username:").AddTo(usernameLine);
            new TextView(Username)
                .AddTo(usernameLine)
                .Content.Bind(username => Username = username);

            var passwordLine = new HorizontalLayout().AddTo(this);

            new LabelView("password:").AddTo(passwordLine);
            new TextView("").PasswordMode().AddTo(passwordLine)
                .Content.Bind(password => Password = password);

            new ButtonView("登录",
                    () =>
                    {
                        this.PushCommand(() =>
                            {
                                TypeEventSystem.Send<IPackageLoginCommand>(new LoginCommand(Username, Password));
                            });
                    })
                .AddTo(this);

            new ButtonView("注册", () =>
                {
                    Application.OpenURL("http://master.liangxiegame.com/user/register");
                })
                .AddTo(this);
        }

    }
}