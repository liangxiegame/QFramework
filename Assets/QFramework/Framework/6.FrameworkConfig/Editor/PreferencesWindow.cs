/****************************************************************************
 * 2017 ~ 2018.5 liangxie
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

namespace QFramework
{
	using System;
	using System.IO;
	using System.Net.Security;
	using System.Security.Cryptography.X509Certificates;
	using UniRx;
	using UnityEngine;
	using UnityEditor;
	
	public class PreferencesWindow : EditorWindow
	{
		[MenuItem(FrameworkMenuItems.Preferences,false,FrameworkMenuItemsPriorities.Preferences)]
		private static void Open()
		{
			var frameworkConfigEditorWindow = (PreferencesWindow) GetWindow(typeof(PreferencesWindow), true);
			frameworkConfigEditorWindow.titleContent = new GUIContent("QFramework Settings");
			frameworkConfigEditorWindow.CurSettingData = FrameworkSettingData.Load();
			frameworkConfigEditorWindow.Show();
			OnShow();
		}

		private const string URL_GITHUB_API_LATEST_RELEASE =
			"https://api.github.com/repos/liangxiegame/QFramework/releases/latest";

		private const string URL_GITHUB_ISSUE = "https://github.com/liangxiegame/QFramework/issues/new";
		
		[MenuItem(FrameworkMenuItems.CheckForUpdates,false,FrameworkMenuItemsPriorities.CheckForUpdates)]
		static void requestLatestRelease()
		{
		}

		[MenuItem(FrameworkMenuItems.Feedback, false, FrameworkMenuItemsPriorities.Feedback)]
		private static void Feedback()
		{
			Application.OpenURL(URL_GITHUB_ISSUE);
		}


		public  static bool MyRemoteCertificateValidationCallback(System.Object sender,
			X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			bool isOk = true;
			// If there are errors in the certificate chain,
			// look at each error to determine the cause.
			if (sslPolicyErrors != SslPolicyErrors.None)
			{
				foreach (var chainStatus in chain.ChainStatus)
				{
					if (chainStatus.Status == X509ChainStatusFlags.RevocationStatusUnknown) {
						continue;
					}
					chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
					chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
					chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan (0, 1, 0);
					chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
					bool chainIsValid = chain.Build ((X509Certificate2)certificate);
					if (!chainIsValid) {
						isOk = false;
						break;
					}
				}
			}
			return isOk;
		}

		static bool trustSource(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}

		private bool mHasNewVersion = false;

		private static void OnShow()
		{
			ObservableWWW.Get(URL_GITHUB_API_LATEST_RELEASE).Subscribe(response =>
			{
				var latestedVersionInfo = FrameworkVersionInfo.ParseLatest(response);
				latestedVersionInfo.Assets.ForEach(asset =>
				{
					Log.I(asset.Name);
					if (asset.Name.StartsWith("QFramework"))
					{
						var version = asset.Name.Replace("QFramework_v", string.Empty).Replace(".unitypackage", string.Empty);
						var versionChars = version.Split('.');
						int versionCode = 0;

						versionChars.ForEach(versionChar =>
						{
							versionCode *= 100;
							versionCode += versionChar.ToInt();
						});
						
						Log.I(versionCode);
						// 版本比较
						ObservableWWW.GetAndGetBytes(asset.BrowserDownloadUrl).Subscribe(bytes =>
						{
							File.WriteAllBytes(Application.dataPath + "/" + asset.Name, bytes);
							
							AssetDatabase.ImportPackage(Application.dataPath + "/" + asset.Name,true);							
						});
					}
				});
			}, e =>
			{
				Log.E(e);
			});
		}

		public FrameworkSettingData CurSettingData;

		private void OnGUI() 
		{
			CurSettingData.Namespace = EditorGUIUtils.GUILabelAndTextField ("Namespace", CurSettingData.Namespace);
			CurSettingData.UIScriptDir = EditorGUIUtils.GUILabelAndTextField ("UI Script Generate Dir", CurSettingData.UIScriptDir);
			CurSettingData.UIPrefabDir = EditorGUIUtils.GUILabelAndTextField ("UI Prefab Dir", CurSettingData.UIPrefabDir);

			if (GUILayout.Button ("Apply")) 
			{
				CurSettingData.Save ();
			}

			if (mHasNewVersion)
			{
				if (GUILayout.Button("Download New Version"))
				{
					
				}
			}
		}
	}
}

//public enum UpdateState {
//        UpToDate,
//        UpdateAvailable,
//        AheadOfLatestRelease,
//        NoConnection
//    }
//
//    public class UpdateInfo
//    {
//
//        public UpdateState updateState
//        {
//            get { return _updateState; }
//        }
//
//        public readonly string localVersionString;
//        public readonly string remoteVersionString;
//
//        readonly UpdateState _updateState;
//
//        public UpdateInfo(string localVersionString, string remoteVersionString)
//        {
//            this.localVersionString = localVersionString.Trim();
//            this.remoteVersionString = remoteVersionString.Trim();
//
//            if (remoteVersionString != string.Empty)
//            {
//                var localVersion = new Version(localVersionString);
//                var remoteVersion = new Version(remoteVersionString);
//
//                switch (remoteVersion.CompareTo(localVersion))
//                {
//                    case 1:
//                        _updateState = UpdateState.UpdateAvailable;
//                        break;
//                    case 0:
//                        _updateState = UpdateState.UpToDate;
//                        break;
//                    case -1:
//                        _updateState = UpdateState.AheadOfLatestRelease;
//                        break;
//                }
//            }
//            else
//            {
//                _updateState = UpdateState.NoConnection;
//            }
//        }
//    }
//
//    public static class CheckForUpdates 
//    {
//        const string URL_GITHUB_API_LATEST_RELEASE = "https://api.github.com/repos/sschmid/Entitas-CSharp/releases/latest";
//        const string URL_GITHUB_RELEASES = "https://github.com/sschmid/Entitas-CSharp/releases";
//        const string URL_ASSET_STORE = "https://www.assetstore.unity3d.com/#!/content/87638";
//
//        [MenuItem(EntitasMenuItems.check_for_updates, false, EntitasMenuItemPriorities.check_for_updates)]
//        public static void DisplayUpdates() {
//            var info = GetUpdateInfo();
//            displayUpdateInfo(info);
//        }
//
//        public static UpdateInfo GetUpdateInfo()
//        {
////            var localVersion = GetLocalVersion();
//            var remoteVersion = GetRemoteVersion();
////            return new UpdateInfo(localVersion, remoteVersion);
//            return null;
//        }
//
////        public static string GetLocalVersion()
////        {
////            return EntitasResources.GetVersion();
////        }
//
//        public static string GetRemoteVersion() {
//            string latestRelease = null;
//            try {
//                latestRelease = requestLatestRelease();
//            } catch(Exception) {
//                latestRelease = string.Empty;
//            }
//
//            return parseVersion(latestRelease);
//        }
//

//
//        static string parseVersion(string response) {
//            const string versionPattern = @"(?<=""tag_name"":"").*?(?="")";
//            return Regex.Match(response, versionPattern).Value;
//        }
//
//        static void displayUpdateInfo(UpdateInfo info) {
//            switch (info.updateState) {
//                case UpdateState.UpdateAvailable:
//                    if (EditorUtility.DisplayDialog("Entitas Update",
//                            string.Format("A newer version of Entitas is available!\n\n" +
//                            "Currently installed version: {0}\n" +
//                            "New version: {1}", info.localVersionString, info.remoteVersionString),
//                            "Show in Unity Asset Store",
//                            "Cancel"
//                        )) {
//                        Application.OpenURL(URL_ASSET_STORE);
//                    }
//                    break;
//                case UpdateState.UpToDate:
//                    EditorUtility.DisplayDialog("Entitas Update",
//                        "Entitas is up to date (" + info.localVersionString + ")",
//                        "Ok"
//                    );
//                    break;
//                case UpdateState.AheadOfLatestRelease:
//                    if (EditorUtility.DisplayDialog("Entitas Update",
//                            string.Format("Your Entitas version seems to be newer than the latest release?!?\n\n" +
//                            "Currently installed version: {0}\n" +
//                            "Latest release: {1}", info.localVersionString, info.remoteVersionString),
//                            "Show in Unity Asset Store",
//                            "Cancel"
//                        )) {
//                        Application.OpenURL(URL_ASSET_STORE);
//                    }
//                    break;
//                case UpdateState.NoConnection:
//                    if (EditorUtility.DisplayDialog("Entitas Update",
//                            "Could not request latest Entitas version!\n\n" +
//                            "Make sure that you are connected to the internet.\n",
//                            "Try again",
//                            "Cancel"
//                        )) {
//                        DisplayUpdates();
//                    }
//                    break;
//            }
//        }
//

//    }