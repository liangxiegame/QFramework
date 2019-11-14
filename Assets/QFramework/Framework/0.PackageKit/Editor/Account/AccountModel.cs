using System;
using QF.DVA;
using Unidux;

namespace QF.Editor
{
    [Serializable]
    public class State : DvaState
    {
        public bool InLoginView = true;

        public bool Logined = false;
    }

    public class AccountModel : DvaModelEditor<AccountModel, State>
    {
        protected override string Namespace
        {
            get { return "account"; }
        }

        protected override State InitialState
        {
            get
            {
                return new State()
                {
                    Logined = User.Logined
                };
            }
        }

        public class Effects
        {
            public static void Login(string username, string password)
            {
                GetTokenAction.DoGetToken(username, password, token =>
                {
                    User.Username.Value = username;
                    User.Password.Value = password;
                    User.Token.Value = token;
                    User.Save();
                    Dispatch("setLogined", true);
                });
            }

            public static void Logout()
            {
                User.Clear();
                Dispatch("setLogined", false);
            }
        }

        public override State Reduce(State state, DvaAction action)
        {
            switch (action.Type)
            {
                case "setInLoginView":
                    state.InLoginView = (bool) action.Payload;
                    break;
                case "setLogined":
                    state.Logined = (bool) action.Payload;
                    break;
            }

            return state;
        }
    }
}