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

namespace QFramework
{
    public class RegisterView : VerticalLayout
    {
        ControllerNode<PackageKitLoginApp> mControllerNode = ControllerNode<PackageKitLoginApp>.Allocate();

        public RegisterView()
        {
            var usernameLine = new HorizontalLayout().AddTo(this); 
            
            EasyIMGUI.Label().Text("username:").AddTo(usernameLine);
            EasyIMGUI.TextField().AddTo(usernameLine);

            var passwordLine = new HorizontalLayout().AddTo(this);

            EasyIMGUI.Label().Text("password:").AddTo(passwordLine);

            EasyIMGUI.TextField().PasswordMode().AddTo(passwordLine);

            EasyIMGUI.Button()
                .Text("注册")
                .OnClick(() => { })
                .AddTo(this);

            EasyIMGUI.Button()
                .Text("返回注册")
                .OnClick(() => { mControllerNode.SendCommand(new OpenRegisterViewCommand()); })
                .AddTo(this);
        }

        protected override void OnDisposed()
        {
            mControllerNode = null;
        }
    }
}