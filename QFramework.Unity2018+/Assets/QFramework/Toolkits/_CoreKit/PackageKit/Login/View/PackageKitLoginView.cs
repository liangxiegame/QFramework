/****************************************************************************
 * Copyright (c) 2016 ~ 2025 liangxiegame UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
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
    [PackageKitRenderOrder("Account")]
    internal class AccountView : VerticalLayout, IPackageKitView, IController, IUnRegisterList
    {
        public EditorWindow EditorWindow { get; set; }


        public Type Type { get; } = typeof(AccountView);

        public void Init()
        {
            EasyIMGUI.Label().Text("账户信息").FontSize(12).Parent(this);

            var boxLayout = new VerticalLayout("box").Parent(this);

            var logoutBtn = EasyIMGUI.Button().Text("注销")
                .Visible(PackageKitLoginState.Logined.Value)
                .Parent(boxLayout)
                .OnClick(this.SendCommand<LogoutCommand>);

            var loginView = new LoginView()
                .Self(self => self.Visible = PackageKitLoginState.LoginViewVisible.Value)
                .Parent(boxLayout);

            var registerView = new RegisterView()
                .Self(self => self.Visible = PackageKitLoginState.RegisterViewVisible.Value)
                .Parent(boxLayout);

            PackageKitLoginState.Logined.Register(value => logoutBtn.Visible = value).AddToUnregisterList(this);
            
            PackageKitLoginState.LoginViewVisible.Register(value => loginView.Visible = value)
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

        public IArchitecture GetArchitecture() => PackageKitLoginApp.Interface;

        public List<IUnRegister> UnregisterList { get; } = new List<IUnRegister>();
    }
}
#endif