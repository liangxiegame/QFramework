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

using EGO.Framework;
using UniRx;
using UnityEngine;
using VerticalLayout = EGO.Framework.VerticalLayout;

namespace QF.Editor
{
    public class PackageLogin : VerticalLayout, IPackageKitView
    {
        public IQFrameworkContainer Container { get; set; }

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
            var expendLayout = new TreeNode(false,LocaleText.UserInfo)
                .AddTo(this);

            var verticalLyaout = new VerticalLayout("box");

            expendLayout.Add2Spread(verticalLyaout);

            AccountModel.Subject
                .StartWith(AccountModel.State)
                .Subscribe(state =>
                {
                    verticalLyaout.Clear();

                    if (state.Logined)
                    {
                        new ButtonView("注销", () =>
                        {
                            AccountModel.Effects.Logout();
                        }).AddTo(verticalLyaout);
                    }
                    else
                    {
                        if (state.InLoginView)
                        {
                            new LoginView().AddTo(verticalLyaout);
                        }
                        else
                        {
                            new RegisterView().AddTo(verticalLyaout);
                        }
                    }
                });
        }

        void IPackageKitView.OnUpdate()
        {
            AccountModel.Update();
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
            AccountModel.Dispose();
        }
    }
}