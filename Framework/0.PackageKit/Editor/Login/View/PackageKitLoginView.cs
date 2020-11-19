/****************************************************************************
 * Copyright (c) 2020.10 liangxie
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
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

using System.ComponentModel;

namespace QFramework
{
    [DisplayName("账户")]
    [PackageKitGroup("QFramework")]
    [PackageKitRenderOrder(int.MaxValue)]
    public class PackageKitLoginView : VerticalLayout, IPackageKitView
    {
        public IQFrameworkContainer Container { get; set; }

        ControllerNode<PackageKitLoginApp> mControllerNode = ControllerNode<PackageKitLoginApp>.Allocate();

        public bool Ignore { get; private set; }

        public bool Enabled
        {
            get { return true; }
        }

        DisposableList mDisposableList = new DisposableList();

        public void Init(IQFrameworkContainer container)
        {
            EasyIMGUI.Label().Text("账户信息").FontSize(12).AddTo(this);

            var boxLayout = new VerticalLayout("box").AddTo(this);
            
            var logoutBtn = EasyIMGUI.Button().Text("注销")
                .Visible(PackageKitLoginState.Logined.Value)
                .AddTo(boxLayout);

            var loginView = new LoginView()
                .Do(self => self.Visible = PackageKitLoginState.LoginViewVisible.Value)
                .AddTo(boxLayout);

            var registerView = new RegisterView()
                .Do(self => self.Visible = PackageKitLoginState.RegisterViewVisible.Value)
                .AddTo(boxLayout);

            PackageKitLoginState.Logined.Bind(value => { logoutBtn.Visible = value; }).AddTo(mDisposableList);

            
            logoutBtn.OnClick(mControllerNode.SendCommand<LogoutCommand>);

            PackageKitLoginState.LoginViewVisible.Bind(value => { loginView.Visible = value; }).AddTo(mDisposableList);


            PackageKitLoginState.RegisterViewVisible.Bind(value => { registerView.Visible = value; })
                .AddTo(mDisposableList);
        }

        public void OnUpdate()
        {
        }

        void IPackageKitView.OnGUI()
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
            mControllerNode.Recycle2Cache();
            mControllerNode = null;

            mDisposableList.Dispose();
            mDisposableList = null;
        }

        public void OnShow()
        {
            
        }

        public void OnHide()
        {
        }
    }
}