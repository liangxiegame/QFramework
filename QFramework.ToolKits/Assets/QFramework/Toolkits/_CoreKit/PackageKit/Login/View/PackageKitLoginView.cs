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

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;

namespace QFramework
{
    [DisplayName("账户")]
    [DisplayNameCN("账户")]
    [DisplayNameEN("Account(OnlyCN)")]
    [PackageKitGroup("QFramework")]
    [PackageKitRenderOrder(int.MaxValue)]
    internal class PackageKitLoginView : VerticalLayout, IPackageKitView, IController, IUnRegisterList
    {
        public EditorWindow EditorWindow { get; set; }


        public Type Type { get; } = typeof(PackageKitLoginView);

        public void Init()
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

            PackageKitLoginState.Logined.Register(value => { logoutBtn.Visible = value; }).AddToUnregisterList(this);


            logoutBtn.OnClick(this.SendCommand<LogoutCommand>);

            PackageKitLoginState.LoginViewVisible.Register(value => { loginView.Visible = value; })
                .AddToUnregisterList(this);


            PackageKitLoginState.RegisterViewVisible.Register(value => { registerView.Visible = value; })
                .AddToUnregisterList(this);
        }

        public void OnUpdate()
        {
        }

        void IPackageKitView.OnGUI()
        {
            DrawGUI();
        }

        public void OnWindowGUIEnd()
        {
        }


        public void OnDispose()
        {
            this.UnRegisterAll();
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

        public List<IUnRegister> UnregisterList { get; } = new List<IUnRegister>();
    }
}
#endif