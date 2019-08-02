using EGO.Framework;
using UniRx;

namespace QF.Editor
{
    public class RegisterView : VerticalLayout
    {
        public RegisterView()
        {
            AccountModel.Subject.StartWith(AccountModel.State)
                .Subscribe(state =>
                {
                    this.Clear();

                    var usernameLine = new HorizontalLayout().AddTo(this);
                    new EGO.Framework.LabelView("username:").AddTo(usernameLine);
                    new TextView("").AddTo(usernameLine);

                    var passwordLine = new HorizontalLayout().AddTo(this);

                    new EGO.Framework.LabelView("password:").AddTo(passwordLine);
                    
                    new TextView("").PasswordMode().AddTo(passwordLine);

                    new ButtonView("注册", () => { }).AddTo(this);
                    new ButtonView("返回注册", () => { AccountModel.Dispatch("setLoginView", true); })
                        .AddTo(this);
                });
        }
    }
}