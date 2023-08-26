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

using System;
using UnityEditor;
using System.IO;
using QFramework;

namespace QFramework
{
	[InitializeOnLoad]
	public class ResKitAssetsMenu
	{
		public const   string AssetBundlesOutputPath       = "AssetBundles";
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
			try
			{
				var ai = AssetImporter.GetAtPath(path);
				var dir = new DirectoryInfo(path);
				return string.Equals(ai.assetBundleName, dir.Name.Replace(".", "_").ToLower());
			}
#pragma warning disable CS0168
			catch (Exception _)
#pragma warning restore CS0168
			{
				return false;
			}
		}

		public static void MarkAB(string path)
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
			MarkAB(path);
		}

		public static string GetSelectedPathOrFallback()
		{
			var path = string.Empty;

			foreach (var obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
			{
				path = AssetDatabase.GetAssetPath(obj);

				if (!string.IsNullOrEmpty(path) && File.Exists(path))
				{
					return path;
				}
			}

			return path;
		}
	}
}