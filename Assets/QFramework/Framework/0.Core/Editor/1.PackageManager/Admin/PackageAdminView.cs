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

using Invert.Common.UI;
using UnityEditorUI;
using UnityEngine;

namespace QFramework.Editor
{
	public class PackageAdminView : GUIView, IPackageKitView
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

		private RootLayout mRootLayout = null;

		public void Init(IQFrameworkContainer container)
		{
			mRootLayout = new RootLayout();
		}

		bool inRegisterView = false;

		public override void OnGUI()
		{
			base.OnGUI();

				
			mRootLayout.OnGUI();

			if (GUIHelpers.DoToolbarEx("User Info"))
			{

				GUILayout.BeginVertical("box");

				if (User.Token.Value.IsNullOrEmpty())
				{
					User.Username.Value = EditorGUIUtils.GUILabelAndTextField("username:", User.Username.Value);
					User.Password.Value = EditorGUIUtils.GUILabelAndPasswordField("password:", User.Password.Value);

					if (!inRegisterView && GUILayout.Button("登录"))
					{
						GetTokenAction.DoGetToken(User.Username.Value, User.Password.Value, token =>
						{
							User.Token.Value = token;
							User.Save();
						});
					}

					if (!inRegisterView && GUILayout.Button("注册"))
					{
						inRegisterView = true;
					}

					if (inRegisterView)
					{
						if (GUILayout.Button("注册"))
						{

						}

						if (GUILayout.Button("返回注册"))
						{
							inRegisterView = false;
						}
					}
				}
				else
				{
					if (GUILayout.Button("注销"))
					{
						User.Clear();
					}
				}

				GUILayout.EndVertical();
			}
		}
	}
}