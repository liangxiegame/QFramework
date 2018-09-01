/****************************************************************************
 * Copyright (c) 2018.8 liangxie
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
using EditorCoroutines;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace QFramework
{
	public class PackageMaker : EditorWindow
	{
		private const byte STATE_GENERATE_INIT      = 0;
		private const byte STATE_GENERATE_PACKING   = 1;
		private const byte STATE_GENERATE_UPLOADING = 2;
		private const byte STATE_GENERATE_COMPLETE  = 3;

		private byte mGenerateState = STATE_GENERATE_INIT;

		private string mUploadResult = "";

		private PackageVersion mPackageVersion;

		private bool mHasConfigFile;

		[MenuItem("Assets/@QPM - Publish Package", priority = 2)]
		public static void ExportPTPlugin()
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

			if (files.Length > 0)
			{

				mHasConfigFile = true;

				mPackageVersion = PackageVersion.Load(packageFolder);
			}
			else
			{

				mPackageVersion = new PackageVersion() {InstallPath = packageFolder};

				mHasConfigFile = false;
			}

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

				case STATE_GENERATE_PACKING:

					GotoUploading();

					break;
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

		private void GotoPacking()
		{
			mGenerateState = STATE_GENERATE_PACKING;
			noticeMessage = "插件导出中,请稍后...";
		}

		private void GotoUploading()
		{
			noticeMessage = "插件上传中,请稍后...";
			mUploadResult = null;
			mGenerateState = STATE_GENERATE_UPLOADING;
			Upload();
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
			if (mHasConfigFile)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label("当前版本号", GUILayout.Width(100));
				GUILayout.Label(mPackageVersion.Version, GUILayout.Width(100));
				GUILayout.EndHorizontal();
			}

			GUILayout.BeginHorizontal();
			GUILayout.Label("发布版本号", GUILayout.Width(100));
			mVersionText = GUILayout.TextField(mVersionText, GUILayout.Width(100));
			GUILayout.EndHorizontal();
			GUILayout.Label("发布说明:", GUILayout.Width(150));
			mReleaseNote = GUILayout.TextArea(mReleaseNote, GUILayout.Width(250), GUILayout.Height(300));


			User.Username = EditorGUIUtils.GUILabelAndTextField("username:", User.Username);
			User.Password = EditorGUIUtils.GUILabelAndTextField("password:", User.Password);

			if (GUILayout.Button("发布"))
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
				mPackageVersion.Readme = new ReleaseItem(mVersionText, mReleaseNote, SystemInfo.deviceName,
					DateTime.Now.ToString("yyyy-MM-dd"));


				mPackageVersion.Save();

				AssetDatabase.Refresh();

				GotoPacking();
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


		private void Upload()
		{
			this.StartCoroutine(UploadPackage.DoUpload(User.Username, User.Password, mPackageVersion, () =>
			{
				mUploadResult = "上传成功";
				GotoComplete();
			}));
		}

		private static void ShowErrorMsg(string content)
		{
			EditorUtility.DisplayDialog("error", content, "OK");
		}
	}
}