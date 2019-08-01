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
using UniRx;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace QFramework.Editor
{
	public class PackageMaker : EditorWindow
	{
		private const byte STATE_GENERATE_INIT      = 0;
		private const byte STATE_GENERATE_UPLOADING = 2;
		private const byte STATE_GENERATE_COMPLETE  = 3;

		private byte mGenerateState = STATE_GENERATE_INIT;

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


			EditorApplication.update += Update;

		}

		private void OnDisable()
		{
			EditorApplication.update -= Update;
		}

		private void Update()
		{
			switch (mGenerateState)
			{
				case STATE_GENERATE_COMPLETE:

					if (EditorUtility.DisplayDialog("上传结果", mUploadResult, "OK"))
					{
						AssetDatabase.Refresh();
						mGenerateState = STATE_GENERATE_INIT;
						Close();
					}

					break;
			}
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

		private void GotoComplete()
		{
			mGenerateState = STATE_GENERATE_COMPLETE;
		}

		private string noticeMessage = "";

		private void DrawNotice()
		{
			EditorGUI.LabelField(new Rect(100, 200, 200, 200), noticeMessage, EditorStyles.boldLabel);
		}

		private string mVersionText = string.Empty;

		private string mReleaseNote = string.Empty;

		

		private void DrawInit()
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("当前版本号", GUILayout.Width(100));
			GUILayout.Label(mPackageVersion.Version, GUILayout.Width(100));
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("发布版本号", GUILayout.Width(100));
			mVersionText = GUILayout.TextField(mVersionText, GUILayout.Width(100));
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("类型", GUILayout.Width(100));

			mPackageVersion.Type = (PackageType) EditorGUILayout.EnumPopup(mPackageVersion.Type);

			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("权限", GUILayout.Width(100));

			mPackageVersion.AccessRight = (PackageAccessRight) EditorGUILayout.EnumPopup(mPackageVersion.AccessRight);
			GUILayout.EndHorizontal();

			GUILayout.Label("发布说明:", GUILayout.Width(150));
			mReleaseNote = GUILayout.TextArea(mReleaseNote, GUILayout.Width(250), GUILayout.Height(300));

			GUILayout.BeginHorizontal();
			GUILayout.Label("文档地址:",GUILayout.Width(52));
			mPackageVersion.DocUrl = GUILayout.TextField(mPackageVersion.DocUrl, GUILayout.Width(150));
			if (GUILayout.Button("Paste"))
			{
				mPackageVersion.DocUrl = GUIUtility.systemCopyBuffer;
			}
			GUILayout.EndHorizontal();

			if (User.Token.Value.IsNotNullAndEmpty())
			{
				var clicked = false;
				var deleteLocal = false;

				if (GUILayout.Button("发布"))
				{
					clicked = true;
				}

				if (GUILayout.Button("发布并删除本地"))
				{
					clicked = true;
					deleteLocal = true;
				}

				if (clicked)
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

					noticeMessage = "插件导出中,请稍后...";

					Observable.NextFrame().Subscribe(_ =>
					{
						noticeMessage = "插件上传中,请稍后...";
						mUploadResult = null;
						mGenerateState = STATE_GENERATE_UPLOADING;
						UploadPackage.DoUpload(mPackageVersion, () =>
						{
							if (deleteLocal)
							{
								Directory.Delete(mPackageVersion.InstallPath, true);
								AssetDatabase.Refresh();
							}

							mUploadResult = "上传成功";
							GotoComplete();
						});
					});
				}
			}
		}

		public void OnGUI()
		{
			switch (mGenerateState)
			{
				case STATE_GENERATE_INIT:
					DrawInit();
					break;
				default:
					DrawNotice();
					break;
			}
		}

		private static void ShowErrorMsg(string content)
		{
			EditorUtility.DisplayDialog("error", content, "OK");
		}
	}
}