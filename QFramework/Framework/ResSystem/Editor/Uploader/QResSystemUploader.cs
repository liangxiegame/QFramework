/****************************************************************************
 * Copyright (c) 2017 wanzhenyu@putao.com
 * Copyright (c) 2017 liangxie
 * 
 * http://liangxiegame.com
 * https://github.com/liangxiegame/QFramework
 * https://github.com/liangxiegame/QSingleton
 * https://github.com/liangxiegame/QChain
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

using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Collections.Generic;

namespace QFramework.ResSystem
{
	public class QResSystemUploader : LoomEditorWindow
	{
		static string mDataPath;

		[MenuItem("PuTaoTool/Framework/ResSystem/Uploader")]
		public static void ExecuteAssetBundle()
		{
			QResSystemUploader window = (QResSystemUploader) GetWindow(typeof(QResSystemUploader), true);
			Debug.Log(Screen.width + " screen width*****");
			window.position = new Rect(100, 100, 500, 400);
			mDataPath = Application.dataPath;
			Debug.Log(mDataPath + ":::::dataPath");
			window.Show();
		}

		private bool mIsOnline = false;

		private string mGameName = "";

		//		private string localPath = "";
		private string mAppVersion = "";

		private bool mIsAndroid = false;
		private bool mIsIOS = true;

		void OnGUI()
		{
			GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();
			GUILayout.Label("game name");
			mGameName = EditorGUILayout.TextField(mGameName);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("app version");
			mAppVersion = EditorGUILayout.TextField(mAppVersion);
			GUILayout.EndHorizontal();

//			GUILayout.BeginHorizontal ();
//			GUILayout.Label ("local path");
//			localPath = EditorGUILayout.TextField (localPath);
//			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal();
			bool result1 = GUILayout.Toggle(mIsAndroid, "Android");
			bool result2 = GUILayout.Toggle(mIsIOS, "iOS");
			if (result1 != mIsAndroid)
			{
				if (result1)
				{
					mIsAndroid = true;
					mIsIOS = false;
				}
				else
				{
					mIsAndroid = false;
					mIsIOS = true;
				}
			}
			else if (result2 != mIsIOS)
			{
				if (result2)
				{
					mIsAndroid = false;
					mIsIOS = true;
				}
				else
				{
					mIsAndroid = true;
					mIsIOS = false;
				}
			}

			GUILayout.EndHorizontal();

			//isOnline = GUILayout.Toggle (isOnline, "online");

			if (GUILayout.Button("Upload"))
			{
				this.RunAsync(Upload);

			}
			GUILayout.EndVertical();

		}

		public class FTPLoginInfo
		{
			public string hostName;
			public string userName;
			public string password;
			public string port;
			public string remoteRefPath;
		}

		private FTPLoginInfo loginInfoInner = new FTPLoginInfo()
		{
//			hostName = @"10.1.11.29",
//			port = "22",
//			userName = "root",
//			password = "putao123",
//			remoteRefPath = "/data/web/static.putao.com/source/"

			hostName = @"ftp://10.1.11.39",
			port = "22",
			userName = "wanzhengyu",
			password = "game@putao.com",
			remoteRefPath = ""
		};

		private FTPLoginInfo loginInfoOnline = new FTPLoginInfo()
		{
			hostName = @"ftp://183.131.150.200",
			userName = "game",
			password = "az15eXdxSl7jE",
			remoteRefPath = ""
		};

		public FTPLoginInfo GetLoginInfo(bool isOnline)
		{
			if (isOnline)
			{
				return loginInfoOnline;
			}
			return loginInfoInner;
		}

		private const string key_assetbundle_uploader_gamename = "key_assetbundle_uploader_gamename";
		private const string key_assetbundle_uploader_localpath = "key_assetbundle_uploader_localpath";
		private const string key_assetbundle_uploader_appversion = "key_assetbundle_uploader_appversion";
		private const string key_assetbundle_uploader_isios = "key_assetbundle_uploader_isios";

		public void OnDisable()
		{

			EditorPrefs.SetString(key_assetbundle_uploader_gamename, mGameName);
//			EditorPrefs.SetString (key_assetbundle_uploader_localpath, localPath);
			EditorPrefs.SetString(key_assetbundle_uploader_appversion, mAppVersion);
			EditorPrefs.SetBool(key_assetbundle_uploader_isios, mIsIOS);
			Debug.Log("disable *******");
			mDataPath = Application.dataPath;
		}

		public void OnEnable()
		{

			mGameName = EditorPrefs.GetString(key_assetbundle_uploader_gamename, "");
//			localPath = EditorPrefs.GetString (key_assetbundle_uploader_localpath, "");
			mAppVersion = EditorPrefs.GetString(key_assetbundle_uploader_appversion, "");
			mIsIOS = EditorPrefs.GetBool(key_assetbundle_uploader_isios, true);
			mIsAndroid = !mIsIOS;

			mDataPath = Application.dataPath;


		}


		public static void ListFiles(FileSystemInfo info, ref List<string> dirs, ref List<string> fls)
		{
			if (!info.Exists)
				return;

			DirectoryInfo dir = info as DirectoryInfo;
			if (dir == null)
				return;
			Debug.Log(dir.FullName + ":dir****");
			dirs.Add(dir.FullName);
			FileSystemInfo[] files = dir.GetFileSystemInfos();
			for (int i = 0; i < files.Length; i++)
			{
				FileInfo file = files[i] as FileInfo;
				if (file != null)
				{
					if (file.Extension != ".meta")
					{
						Debug.Log(file.FullName);
						fls.Add(file.FullName);
					}

				}
				else
				{
					ListFiles(files[i], ref dirs, ref fls);
				}
			}
		}


		private string GetRemoteResVersionDir()
		{
			return "version_" + mAppVersion.Replace(".", "");
		}



		private void Upload()
		{

			if (string.IsNullOrEmpty(mAppVersion))
			{
				this.QueueOnMainThread(() =>
				{
					EditorUtility.DisplayDialog("error", "please input appversion", "ok");
				});
				return;
			}

			if (string.IsNullOrEmpty(mGameName))
			{
				this.QueueOnMainThread(() =>
				{
					EditorUtility.DisplayDialog("error", "please input game name", "ok");
				});
				return;
			}

			FTPLoginInfo loginInfo = GetLoginInfo(mIsOnline);

			IFTPInterface sftpHelper;
			if (mIsOnline)
			{
				sftpHelper = new FTPClient(loginInfo.hostName, loginInfo.userName, loginInfo.password);

			}
			else
			{
//				sftpHelper = new SFTPHelper (loginInfo.hostName, loginInfo.port, loginInfo.userName, loginInfo.password);
				sftpHelper = new FTPClient(loginInfo.hostName, loginInfo.userName, loginInfo.password);

			}

			this.QueueOnMainThread(() =>
			{
				EditorUtility.DisplayProgressBar("upload assetbundles to ftp server", "conneting to  ftp server ...", 0);
			});

			bool result = sftpHelper.Connect();
			if (!result)
			{
				this.QueueOnMainThread(() =>
				{
					EditorUtility.ClearProgressBar();
					EditorUtility.DisplayDialog("error", " connect failed \n please check your network", "ok");
				});
				return;
			}



			this.QueueOnMainThread(() =>
			{
				EditorUtility.DisplayProgressBar("upload assetbundles to ftp server", "checking server content...", 0);
			});
			string remoteGameDir = loginInfo.remoteRefPath + this.mGameName;
			string destRemoteDir = remoteGameDir + "/" + GetRemoteResVersionDir();
			if (sftpHelper.Exist(remoteGameDir, GetRemoteResVersionDir()))
			{

				string timestamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm");
				sftpHelper.Rename(destRemoteDir, destRemoteDir + "_" + timestamp);
			}
			this.QueueOnMainThread(() =>
			{

				EditorUtility.DisplayProgressBar("upload assetbundles to ftp server", "checking server finish ...", 0);
			});

			sftpHelper.MakeDir(destRemoteDir);
			string abDirectory = mDataPath + "/StreamingAssets/AssetBundles/iOS";
			if (!mIsIOS)
			{
				abDirectory = mDataPath + "/StreamingAssets/AssetBundles/Android";
			}

			if (!Directory.Exists(abDirectory))
			{
				this.QueueOnMainThread(() =>
				{
					EditorUtility.ClearProgressBar();
					EditorUtility.DisplayDialog("error", "can not find local assetbundles", "ok");
				});

				return;
			}
			this.QueueOnMainThread(() =>
			{
				EditorUtility.DisplayProgressBar("upload assetbundles to ftp server", "start upload content...", 0);
			});



			DirectoryInfo theFolder = new DirectoryInfo(abDirectory);
			Debug.Log(theFolder.FullName + ":HHHHHHH");

			List<string> dirs = new List<string>();
			List<string> files = new List<string>();
			ListFiles(theFolder, ref dirs, ref files);

			string premark = abDirectory + "/";
			Debug.Log(premark + ":premark>>>>>>>>>");
			string failedFile = null;
			int totalCount = dirs.Count + files.Count;
			int counter = 0;
			foreach (string dir in dirs)
			{
				if (dir.Length > premark.Length)
				{
					string finalDir = dir.Remove(0, premark.Length);
					result = sftpHelper.MakeDir(destRemoteDir + "/" + finalDir);
					counter++;
					this.QueueOnMainThread(() =>
					{
						EditorUtility.DisplayProgressBar("create directory on ftp server", "complete " + counter + "/" + totalCount,
							counter * 1.0f / totalCount);
					});
					if (!result)
					{
						failedFile = finalDir;
						break;
					}
				}
			}
			foreach (string filePath in files)
			{
				if (filePath.Length > premark.Length)
				{
					string finalFile = filePath.Remove(0, premark.Length);
					result = sftpHelper.Upload(filePath, destRemoteDir + "/" + finalFile);
					counter++;
					this.QueueOnMainThread(() =>
					{
						EditorUtility.DisplayProgressBar("upload assetbundles to ftp server", "complete " + counter + "/" + totalCount,
							counter * 1.0f / totalCount);
					});
					if (!result)
					{
						failedFile = finalFile;
						break;
					}
				}
			}



//			string[] localFiles = Directory.GetFiles (abDirectory, "*").Where (t => {
//				if (t.ToLower ().EndsWith (".meta")||t.ToLower().EndsWith(".manifest")) {
//					return false;
//				}
//				return true;
//			}).ToArray ();
//
//			int count = 0;
//			string failedFile = null;
//			foreach (var file in localFiles) {
//				this.QueueOnMainThread (() => {
//					EditorUtility.DisplayProgressBar ("upload assetbundles to ftp server", "complete " + count + "/" + localFiles.Length, count * 1.0f / localFiles.Length);
//				});
//
//				Debug.Log ("******upload file :" + Path.GetFileName (file));
//				result = sftpHelper.Upload (file, destRemoteDir + "/" + Path.GetFileName (file));
//				Debug.Log ("******uplaod result is : " + result);
//				if (!result) {
//					failedFile = file;
//					break;
//				}
//				count++;
//			}

			sftpHelper.Disconnect();

			this.QueueOnMainThread(() =>
			{
				EditorUtility.ClearProgressBar();
			});


//			if (string.IsNullOrEmpty (failedFile)) {
//				this.QueueOnMainThread (() => {
//					EditorUtility.DisplayDialog ("success", "upload assetbundles success!", "ok");
//				});
//
//			} else {
//				this.QueueOnMainThread (() => {
//					EditorUtility.DisplayDialog ("error", "upload failed :" + Path.GetFileName (failedFile), "ok");
//				});
//			}
		}

	}
}