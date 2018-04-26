/****************************************************************************
 * Copyright (c) 2017 liangxie
 * 
 * http://qframework.io
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

namespace QFramework
{
	using UnityEngine;
	using UnityEditor;
	
	public class ResKitEditorWindow : EditorWindow
	{
		private int mBuildTargetIndex = 0;
		private readonly string[] mPlatformLabels = {"Windows32", "iOS", "Android"};
		private Vector2 scrollPos;
		private const string KEY_QAssetBundleBuilder_RESVERSION = "KEY_QAssetBundleBuilder_RESVERSION";
		private const string KEY_AUTOGENERATE_CLASS = "KEY_AUTOGENERATE_CLASS";

		private const string KEY_ProjectTag = "KEY_ProjectTag";

		public static string resVersion = "100";
		private static string projectTag = "";

		//public static bool isUseFramework = true;
		public static bool isEnableGenerateClass = false;

		public static void ForceClear()
		{

			QResSystemMark.AssetBundlesOutputPath.DeleteDirIfExists();
			(Application.streamingAssetsPath + "/AssetBundles").DeleteDirIfExists();

			AssetDatabase.Refresh();
		}

		[MenuItem("QFramework/Res Kit %#r")]
		public static void ExecuteAssetBundle()
		{
			ResKitEditorWindow window = (ResKitEditorWindow) GetWindow(typeof(ResKitEditorWindow), true);
			Debug.Log(Screen.width + " screen width*****");
			window.position = new Rect(100, 100, 500, 400);
			window.Show();
		}

		private void OnEnable()
		{
			resVersion = EditorPrefs.GetString(KEY_QAssetBundleBuilder_RESVERSION, "100");
			isEnableGenerateClass = EditorPrefs.GetBool(KEY_AUTOGENERATE_CLASS, true);

			projectTag = EditorPrefs.GetString(KEY_ProjectTag, "");

			switch (EditorUserBuildSettings.activeBuildTarget)
			{
				case BuildTarget.Android:
					mBuildTargetIndex = 2;
					break;
				case BuildTarget.iOS:
					mBuildTargetIndex = 1;
					break;
				default:
					mBuildTargetIndex = 0;
					break;

			}
		}

		private void DrawMenu()
		{
			GUILayout.Toolbar(mBuildTargetIndex, mPlatformLabels);
		}

		public void OnDisable()
		{
			EditorPrefs.SetBool(KEY_AUTOGENERATE_CLASS, isEnableGenerateClass);
			EditorPrefs.SetString(KEY_QAssetBundleBuilder_RESVERSION, resVersion);
			EditorPrefs.SetString(KEY_ProjectTag, projectTag);
		}

		private void OnGUI()
		{
			scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(500), GUILayout.Height(400));
			GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();
			GUILayout.Label("PersistanePath:");
			GUILayout.TextField(Application.persistentDataPath);
			GUILayout.EndHorizontal();
			if (GUILayout.Button("Go To Persistance"))
			{
				EditorUtility.RevealInFinder(Application.persistentDataPath);
			}

			DrawMenu();

			isEnableGenerateClass = GUILayout.Toggle(isEnableGenerateClass, "auto generate class");

			GUILayout.BeginHorizontal();
			GUILayout.Label("ResVersion:");
			resVersion = GUILayout.TextField(resVersion);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Project Tag:");
			projectTag = GUILayout.TextField(projectTag);

			GUILayout.EndHorizontal();

			if (GUILayout.Button("1.Gen Res Tree File"))
			{
				AssetBundleExporter.BuildDataTable();
			}

			if (GUILayout.Button("2.Build"))
			{
				AssetBundleExporter.BuildDataTable();
				BuildWithTarget(EditorUserBuildSettings.activeBuildTarget);
			}

			if (GUILayout.Button("ForceClear"))
			{
				ForceClear();
			}

			GUILayout.EndVertical();
			GUILayout.Space(50);

			EditorGUILayout.EndScrollView();

		}

		private void BuildWithTarget(BuildTarget buildTarget)
		{
			AssetDatabase.RemoveUnusedAssetBundleNames();
			AssetDatabase.Refresh();
			BuildScript.BuildAssetBundles(buildTarget);
		}
	}
}