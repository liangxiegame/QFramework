/****************************************************************************
 * Copyright (c) 2017 ~ 2018.5 liangxie
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

using UnityEditor;
using System.IO;

namespace QF.Res
{
	[InitializeOnLoad]
	public class ResKitAssetsMenu
	{
		public const   string AssetBundlesOutputPath       = "AssetBundles";
		private static int    mSimulateAssetBundleInEditor = -1;
		private const  string kSimulateAssetBundles        = "SimulateAssetBundles";

		// Flag to indicate if we want to simulate assetBundles in Editor without building them actually.
		public static bool SimulateAssetBundleInEditor
		{
			get
			{
				if (mSimulateAssetBundleInEditor == -1)
					mSimulateAssetBundleInEditor = EditorPrefs.GetBool(kSimulateAssetBundles, true) ? 1 : 0;

				return mSimulateAssetBundleInEditor != 0;
			}
			set
			{
				var newValue = value ? 1 : 0;
				if (newValue != mSimulateAssetBundleInEditor)
				{
					mSimulateAssetBundleInEditor = newValue;
					EditorPrefs.SetBool(kSimulateAssetBundles, value);
				}
			}
		}

		private const string Mark_AssetBundle   = "Assets/@ResKit - AssetBundle Mark";

		static ResKitAssetsMenu()
		{
			Selection.selectionChanged = OnSelectionChanged;
		}

		public static void OnSelectionChanged()
		{
			var path = GetSelectedPathOrFallback();
			if (!string.IsNullOrEmpty(path))
			{
				Menu.SetChecked(Mark_AssetBundle, Marked(path));
			}
		}

		public static bool Marked(string path)
		{
			var ai = AssetImporter.GetAtPath(path);
			var dir = new DirectoryInfo(path);
			return string.Equals(ai.assetBundleName, dir.Name.Replace(".", "_").ToLower());
		}

		public static void MarkPTAB(string path)
		{
			if (!string.IsNullOrEmpty(path))
			{
				var ai = AssetImporter.GetAtPath(path);
				var dir = new DirectoryInfo(path);

				if (Marked(path))
				{
					Menu.SetChecked(Mark_AssetBundle, false);
					ai.assetBundleName = null;
				}
				else
				{
					Menu.SetChecked(Mark_AssetBundle, true);
					ai.assetBundleName = dir.Name.Replace(".", "_");
				}

				AssetDatabase.RemoveUnusedAssetBundleNames();
			}
		}


		[MenuItem(Mark_AssetBundle)]
		public static void MarkPTABDir()
		{
			var path = GetSelectedPathOrFallback();
			MarkPTAB(path);
		}

		public static string GetSelectedPathOrFallback()
		{
			var path = string.Empty;

			foreach (var obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
			{
				path = AssetDatabase.GetAssetPath(obj);
				if (!string.IsNullOrEmpty(path) && File.Exists(path))
				{
				}
			}

			//Debug.Log ("path ***** :"+path);
			return path;
		}
	}
}