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
    public class PackageKitLoginView : VerticalLayout, IPackageKitView,IController
    {
        public IQFrameworkContainer Container { get; set; }


        public bool Ignore { get; private set; }

        public bool Enabled
        {
            get { return true; }
        }

        DisposableList mDisposableList = new DisposableList();

        public void Init(IQFrameworkContainer container)
        {
            EasyIMGUI.Label().Text("账户信息").FontSize(12).Parent(this);

            var boxLayout = new VerticalLayout("box").Parent(this);
            
            var logoutBtn = EasyIMGUI.Button().Text("注销")
                .Visible(PackageKitLoginState.Logined.Value)
                .Parent(boxLayout);

            var loginView = new LoginView()
                .Self(self => self.Visible = PackageKitLoginState.LoginViewVisible.Value)
                .Parent(boxLayout);

            var registerView = new RegisterView()
                .Self(self => self.Visible = PackageKitLoginState.RegisterViewVisible.Value)
                .Parent(boxLayout);

            PackageKitLoginState.Logined.Bind(value => { logoutBtn.Visible = value; }).AddToDisposeList(mDisposableList);

            
            logoutBtn.OnClick(this.SendCommand<LogoutCommand>);

            PackageKitLoginState.LoginViewVisible.Bind(value => { loginView.Visible = value; }).AddToDisposeList(mDisposableList);


            PackageKitLoginState.RegisterViewVisible.Bind(value => { registerView.Visible = value; })
                .AddToDisposeList(mDisposableList);
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
            mDisposableList.Dispose();
            mDisposableList = null;
        }

        public new void OnShow()
        {
            
        }

        public new void OnHide()
        {
        }

        public IArchitecture GetArchitecture()
        {
            return PackageKitLoginApp.Interface;
        }
    }
}