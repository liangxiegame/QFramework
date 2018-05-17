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

namespace QFramework
{
	using UnityEngine;
	using UnityEditor;
	using System.Collections.Generic;
	using System.IO;
	
	public class AssetBundleInfo
	{
		public readonly string Name = "";

		public AssetBundleInfo(string name)
		{
			this.Name = name;
		}

		public string[] assets;
	}

	public static class BuildScript
	{
		public static void BuildAssetBundles(BuildTarget buildTarget)
		{
			// Choose the output path according to the build target.
			var outputPath = Path.Combine(ResKitAssetsMenu.AssetBundlesOutputPath, GetPlatformName());
			outputPath.CreateDirIfNotExists();

			BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.ChunkBasedCompression, buildTarget);

			GenerateVersionConfig(outputPath);

			var finalDir = Application.streamingAssetsPath + "/AssetBundles/" + GetPlatformName();

			finalDir.DeleteDirIfExists();
			finalDir.CreateDirIfNotExists();

			FileUtil.ReplaceDirectory(outputPath, finalDir);
			
			AssetBundleExporter.BuildDataTable();
			AssetDatabase.Refresh();
		}

		private static void GenerateVersionConfig(string outputPath)
		{
			var abManifestFile = Path.Combine(outputPath, GetPlatformName());
			var ab = AssetBundle.LoadFromFile(abManifestFile);

			var abMainfest = (AssetBundleManifest) ab.LoadAsset("AssetBundleManifest");
			var allABNames = abMainfest.GetAllAssetBundles();
			AssetBundleInfos.Clear();
			
			foreach (var abName in allABNames)
			{
				var assetBundle = AssetBundle.LoadFromFile(Path.Combine(outputPath, abName));
				var abInfo = new AssetBundleInfo(abName) {assets = assetBundle.GetAllAssetNames()};
				AssetBundleInfos.Add(abInfo);
				assetBundle.Unload(true);
			}

			ab.Unload(true);

			AssetDatabase.Refresh();

			if (ResKitEditorWindow.isEnableGenerateClass)
			{
				"Assets/QFrameworkData".CreateDirIfNotExists();

                var path = Path.GetFullPath(Application.dataPath + Path.DirectorySeparatorChar + "QFrameworkData/QAssets.cs");
				var writer = new StreamWriter(File.Open(path, FileMode.Create));
				ResKitInfoGenerator.WriteClass(writer, "QAssetBundle", AssetBundleInfos);
				writer.Close();
				AssetDatabase.Refresh();
			}
		}

		private static readonly List<AssetBundleInfo> AssetBundleInfos = new List<AssetBundleInfo>();

		private static string GetPlatformName()
		{
			return PlatformUtil.GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
		}
	}
}
