/****************************************************************************
 * Copyright (c) 2017 maoling@putao.com
 * Copyright (c) 2018.3 liangxie
 ****************************************************************************/

/// <summary>
/// Project config editor window.
/// </summary>

using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UniRx;

namespace QFramework
{
	using UnityEngine;
	using UnityEditor;
	
	public class PreferencesWindow : EditorWindow
	{
		[MenuItem("QFramework/FrameworkConfig")]
		static void Open() 
		{
			PreferencesWindow frameworkConfigEditorWindow = (PreferencesWindow)GetWindow(typeof(PreferencesWindow),true);
			frameworkConfigEditorWindow.titleContent = new  GUIContent("QFrameworkConfig");
			frameworkConfigEditorWindow.CurConfigData = QFrameworkConfigData.Load ();
			frameworkConfigEditorWindow.Show ();
			frameworkConfigEditorWindow.OnShow();
		}

		private const string URL_GITHUB_API_LATEST_RELEASE =
			"https://api.github.com/repos/liangxiegame/QFramework/releases/latest";
		
		[MenuItem(FrameworkMenuItems.CheckForUpdates,false,FrameworkMenuItemsPriorities.CheckForUpdates)]
		static void requestLatestRelease()
		{

//			var www = new WWW(URL_GITHUB_API_LATEST_RELEASE);
//			EditorCoroutine.Add(() => www.isDone, () =>
//			{
//				if (!string.IsNullOrEmpty(www.error)) Debug.Log("WWW failed: " + www.error);
//				else
//				{
//					Log.E(www.error);
//				}
//				Debug.Log("WWW result : " + www.text);
//			});
			
			
//			using(UnityWebRequest www = UnityWebRequest.Get(URL_GITHUB_API_LATEST_RELEASE)) {
//				www.Send();
// 
//				while (www.responseCode == -1) {
//					//do something, or nothing while blocking
//				}
//				if(www.isError) {
//					Debug.Log(www.error);
//				}
//				else {
//					//Show results as text
//					Debug.Log(www.responseCode.ToString());
//					//process downloadHandler.text
//				}
//			}
//
//			ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
//
//			ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;
//			var httpWebRequest = (HttpWebRequest) WebRequest.Create(URL_GITHUB_API_LATEST_RELEASE);
//			
//			httpWebRequest.UserAgent = Environment.UserName + "liangxiegame/QFramework/CheckForUpdates";
//			httpWebRequest.Timeout = 15000;
//			var webResponse = httpWebRequest.GetResponse();
//
////			ServicePointManager.ServerCertificateValidationCallback -= trustSource;
//			var response = string.Empty;
//			using (var streamReader = new StreamReader(webResponse.GetResponseStream()))
//			{
//				response = streamReader.ReadToEnd();
//			}
//			
//			Log.I(response);

//			return response;
		}
		
		
		public  static bool MyRemoteCertificateValidationCallback(System.Object sender,
			X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			bool isOk = true;
			// If there are errors in the certificate chain,
			// look at each error to determine the cause.
			if (sslPolicyErrors != SslPolicyErrors.None) {
				for (int i=0; i<chain.ChainStatus.Length; i++) {
					if (chain.ChainStatus[i].Status == X509ChainStatusFlags.RevocationStatusUnknown) {
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

		private void OnShow()
		{
			ObservableWWW.Get(URL_GITHUB_API_LATEST_RELEASE).Subscribe(response =>
			{
				var latestedVersionInfo = FrameworkVersionInfo.ParseLatest(response);
			});
		}

		public QFrameworkConfigData CurConfigData;
	
		void OnGUI() 
		{
			CurConfigData.Namespace = EditorGUIUtils.GUILabelAndTextField ("Namespace", CurConfigData.Namespace);
		
			CurConfigData.UIScriptDir = EditorGUIUtils.GUILabelAndTextField ("UI Script Generate Dir", CurConfigData.UIScriptDir);
			CurConfigData.UIPrefabDir = EditorGUIUtils.GUILabelAndTextField ("UI Prefab Dir", CurConfigData.UIPrefabDir);

			if (GUILayout.Button ("Apply")) 
			{
				CurConfigData.Save ();
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