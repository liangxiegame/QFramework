using EGO.Framework;
using UniRx;

namespace QF.Editor
{
    public class LoginView : VerticalLayout
    {
        public string Username = "";
        public string Password = "";

        public LoginView()
        {
            AccountModel.Subject
                .StartWith(AccountModel.State)
                .Subscribe(state =>
                {
                    this.Clear();

                    var usernameLine = new HorizontalLayout().AddTo(this);
                    new LabelView("username:").AddTo(usernameLine);
                    new TextView(Username)
                        .AddTo(usernameLine)
                        .Content.Bind(username => Username = username);

                    var passwordLine = new HorizontalLayout().AddTo(this);

                    new LabelView("password:").AddTo(passwordLine);
                    new TextView("").PasswordMode().AddTo(passwordLine)
                        .Content.Bind(password => Password = password);

                    new ButtonView("登录", () =>
                    {
                        AccountModel.Effects.Login(Username,Password);
                    }).AddTo(this);

                    new ButtonView("注册", () => { AccountModel.Dispatch("setInLoginView", false); })
                        .AddTo(this);
                });
        }
    }
}