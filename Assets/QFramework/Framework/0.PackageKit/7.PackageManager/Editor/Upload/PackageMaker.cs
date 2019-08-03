/****************************************************************************
 * Copyright (c) 2018.8 ~ 2019.1 liangxie
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

using System;
using System.IO;
using EGO.Framework;
using QF.Editor;
using QF.Extensions;
using UniRx;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace QF.PackageKit.Upload
{
	public class PackageMaker : QEditorWindow
	{
		private string mUploadResult = "";

		private PackageVersion mPackageVersion;

		private static void MakePackage()
		{
			var path = MouseSelector.GetSelectedPathOrFallback();

			if (path.IsNotNullAndEmpty())
			{
				if (Directory.Exists(path))
				{
					var installPath = string.Empty;

					if (path.EndsWith("/"))
					{
						installPath = path;
					}
					else
					{
						installPath = path + "/";
					}

					new PackageVersion
					{
						InstallPath = installPath,
						Version = "v0.0.0"
					}.Save();

					AssetDatabase.Refresh();
				}
			}
		}

		[MenuItem("Assets/@QPM - Publish Package", true)]
		static bool ValiedateExportPackage()
		{
			return User.Logined;
		}

		[MenuItem("Assets/@QPM - Publish Package", priority = 2)]
		public static void publishPackage()
		{
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				EditorUtility.DisplayDialog("Package Manager", "请连接网络", "确定");
				return;
			}

			var selectObject = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);

			if (selectObject == null || selectObject.Length > 1)
			{

				return;
			}

			if (!EditorUtility.IsPersistent(selectObject[0]))
			{

				return;
			}

			var path = AssetDatabase.GetAssetPath(selectObject[0]);

			if (!Directory.Exists(path))
			{

				return;
			}

			var mInstance = (PackageMaker) GetWindow(typeof(PackageMaker), true);

			mInstance.titleContent = new GUIContent(selectObject[0].name);
			
			mInstance.position = new Rect(Screen.width / 2, Screen.height / 2, 258, 500);

			mInstance.Show();
		}

		private VerticalLayout RootLayout = null;

		protected override void Init()
		{
			RootLayout = new VerticalLayout("box");
			UploadModel.Subject
				.StartWith(UploadModel.State)
				.Subscribe(state =>
				{
					if (state.Progress == UploadProgress.STATE_GENERATE_INIT)
					{
						RootLayout.Clear();
						
						// 当前版本号
						var versionLine = new HorizontalLayout().AddTo(RootLayout);
						new LabelView("当前版本号").Width(100).AddTo(versionLine);
						new LabelView(mPackageVersion.Version).Width(100).AddTo(versionLine);
						
						// 发布版本号 
						var publishedVertionLine = new HorizontalLayout().AddTo(RootLayout);
						new LabelView("发布版本号").Width(100).AddTo(publishedVertionLine);
						new TextView(mVersionText).Width(100).AddTo(publishedVertionLine)
							.Content.Bind(content=>mVersionText = content);


						var typeLine = new HorizontalLayout().AddTo(RootLayout);
						new LabelView("类型").Width(100).AddTo(typeLine);
						
						new EnumPopupView(mPackageVersion.Type).AddTo(typeLine)
							.ValueProperty.Bind(value=>mPackageVersion.Type = (PackageType) value);
						
						
						var accessRightLine = new HorizontalLayout().AddTo(RootLayout);

						new LabelView("权限").Width(100).AddTo(accessRightLine);
						
						new EnumPopupView(mPackageVersion.AccessRight).AddTo(accessRightLine)
							.ValueProperty.Bind(v=>mPackageVersion.AccessRight = (PackageAccessRight) v);

						new LabelView("发布说明:").Width(150).AddTo(RootLayout);

						new TextAreaView(mReleaseNote).Width(250).Height(300).AddTo(RootLayout)
							.Content.Bind(releaseNote => mReleaseNote = releaseNote);

						var docLine = new HorizontalLayout().AddTo(RootLayout);

						new LabelView("文档地址:").Width(52).AddTo(docLine);
						new TextView(mPackageVersion.DocUrl).Width(150).AddTo(docLine)
							.Content.Bind(value=>mPackageVersion.DocUrl = value);

						new ButtonView("Paste", () =>
						{
							mPackageVersion.DocUrl = GUIUtility.systemCopyBuffer;

						}).AddTo(docLine);


						if (User.Logined)
						{
							new ButtonView("发布", () =>
							{
								User.Save();

								if (mReleaseNote.Length < 2)
								{
									ShowErrorMsg("请输入版本修改说明");
									return;
								}

								if (!IsVersionValide(mVersionText))
								{
									ShowErrorMsg("请输入正确的版本号");
									return;
								}

								mPackageVersion.Version = mVersionText;
								mPackageVersion.Readme = new ReleaseItem(mVersionText, mReleaseNote, User.Username.Value,
									DateTime.Now);

								mPackageVersion.Save();

								AssetDatabase.Refresh();

								
								UploadModel.Effects.Publish(mPackageVersion,false);
							}).AddTo(RootLayout);

							new ButtonView("发布并删除本地", () => { }).AddTo(RootLayout);
						}
					}
					else
					{
						RootLayout.Clear();

						new CustomView(() =>
						{
							EditorGUI.LabelField(new Rect(100, 200, 200, 200), state.NoticeMessage, EditorStyles.boldLabel);
						}).AddTo(RootLayout);
					}
					
					if (state.Progress == UploadProgress.STATE_GENERATE_COMPLETE)
					{
						if (EditorUtility.DisplayDialog("上传结果", mUploadResult, "OK"))
						{
							AssetDatabase.Refresh();

							UploadModel.Dispatch("setProgress", UploadProgress.STATE_GENERATE_INIT);
							Close();
						}
					}
				});
		}
		
		
		private void OnEnable()
		{
			var selectObject = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);

			if (selectObject == null || selectObject.Length > 1)
			{

				return;
			}

			var packageFolder = AssetDatabase.GetAssetPath(selectObject[0]);

			var files = Directory.GetFiles(packageFolder, "PackageVersion.json", SearchOption.TopDirectoryOnly);

			if (files.Length <= 0)
			{
				MakePackage();
			}

			mPackageVersion = PackageVersion.Load(packageFolder);
			
		}

		public override void OnUpdate()
		{
			UploadModel.Update();
		}

		public override void OnClose()
		{
			UploadModel.Dispose();
		}

		public static bool IsVersionValide(string version)
		{
			if (version == null)
			{
				return false;
			}

			var t = version.Split('.');
			return t.Length == 3;
		}

		
		private string mVersionText = string.Empty;

		private string mReleaseNote = string.Empty;

		public override void OnGUI()
		{
			base.OnGUI();
			
			RootLayout.DrawGUI();
		}

		private static void ShowErrorMsg(string content)
		{
			EditorUtility.DisplayDialog("error", content, "OK");
		}
	}
}