/****************************************************************************
 * Copyright (c) 2019.2 liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

using QFramework;

namespace QF.Editor
{
    public class PackageLoginViewUpdateEvent
    {
        public bool Logined     { get; set; }
        public bool InLoginView { get; set; }
    }

    public interface IPackageLoginCommand
    {
        void Execute();
    }

    public class LoginOutCommand : IPackageLoginCommand
    {
        [Inject]
        public PackageLoginModel Model { get; set; }

        public void Execute()
        {
            User.Clear();
            
            TypeEventSystem.Send(new PackageLoginViewUpdateEvent()
            {
                Logined = Model.Logined,
                InLoginView = Model.InLoginView
            });
        }
    }
    
    public class OpenRegisterView : IPackageLoginCommand
    {
        [Inject]
        public PackageLoginModel Model { get; set; }
        
        public void Execute()
        {
            Model.InLoginView = false;
            
            TypeEventSystem.Send(new PackageLoginViewUpdateEvent()
            {
                Logined = Model.Logined,
                InLoginView = Model.InLoginView
            });
        }
    }


    public class LoginCommand : IPackageLoginCommand
    {
        private string mUsername { get; set; }
        private string mPassword { get; set; }

        [Inject]
        public PackageLoginModel Model { get; set; }

        public LoginCommand(string username, string password)
        {
            mUsername = username;
            mPassword = password;
        }

        public void Execute()
        {
            GetTokenAction.DoGetToken(mUsername, mPassword, token =>
            {
                User.Username.Value = mUsername;
                User.Password.Value = mPassword;
                User.Token.Value = token;
                User.Save();

                TypeEventSystem.Send(new PackageLoginViewUpdateEvent()
                {
                    Logined = Model.Logined,
                    InLoginView = Model.InLoginView
                });
            });
        }
    }

    public interface IPackageLoginServer
    {
        
    }

    public class PackageLoginServer
    {
        public void Login(string username, string password)
        {

        }
        
    }
    
    public class PackageLoginApp
    {
        private IQFrameworkContainer mContainer { get; set; }

        public PackageLoginApp()
        {
            mContainer = new QFrameworkContainer();

            mContainer.RegisterInstance(new PackageLoginModel());
            
            mContainer.Register<IPackageLoginServer,PackageLoginServer>();

            TypeEventSystem.Register<IPackageLoginCommand>(OnCommandExecute);
        }

        void OnCommandExecute(IPackageLoginCommand command)
        {
            mContainer.Inject(command);
            command.Execute();
        }

        public void Dispose()
        {
            TypeEventSystem.UnRegister<IPackageLoginCommand>(OnCommandExecute);

            mContainer.Clear();
            mContainer = null;
        }
    }

    public class PackageLoginModel
    {
        public bool InLoginView = true;

        public bool Logined
        {
            get { return User.Logined; }
        }
    }

    public class PackageLoginStartUpCommand : IPackageLoginCommand
    {
        [Inject]
        public PackageLoginModel Model { get; set; }


        public void Execute()
        {
            TypeEventSystem.Send(new PackageLoginViewUpdateEvent()
            {
                InLoginView = Model.InLoginView,
                Logined = Model.Logined
            });
        }
    }


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

            mRefreshLayout = new VerticalLayout("box");

            expendLayout.Add2Spread(mRefreshLayout);

            TypeEventSystem.Register<PackageLoginViewUpdateEvent>(OnRefresh);

            TypeEventSystem.Send<IPackageLoginCommand>(new PackageLoginStartUpCommand());
        }

        private VerticalLayout mRefreshLayout;


        void OnRefresh(PackageLoginViewUpdateEvent updateEvent)
        {
            mRefreshLayout.Clear();

            if (updateEvent.Logined)
            {
                new ButtonView("注销", () =>
                    {
                        this.PushCommand(() =>
                        {
                            TypeEventSystem.Send<IPackageLoginCommand>(new LoginOutCommand()); 
                            
                        });
                    }).AddTo(mRefreshLayout);
            }
            else
            {
                if (updateEvent.InLoginView)
                {
                    new LoginView().AddTo(mRefreshLayout);
                }
                else
                {
                    new RegisterView().AddTo(mRefreshLayout);
                }
            }
        }

        void IPackageKitView.OnUpdate()
        {
        }

        public void OnGUI()
        {
            this.DrawGUI();
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
            TypeEventSystem.UnRegister<PackageLoginViewUpdateEvent>(OnRefresh);
            mPackageLoginApp.Dispose();
            mPackageLoginApp = null;
        }
    }
}