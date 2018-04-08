using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using System;
using System.Security.Cryptography;
using ICSharpCode.SharpZipLib.Zip;
using System.Linq;

namespace QFramework
{
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
			var outputPath = Path.Combine(QResSystemMark.AssetBundlesOutputPath, GetPlatformName());
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

			if (QResSystemBuilder.isEnableGenerateClass)
			{
				"Assets/QFrameworkData".CreateDirIfNotExists();

                var path = Path.GetFullPath(Application.dataPath + Path.DirectorySeparatorChar + "QFrameworkData/QAssets.cs");
				var writer = new StreamWriter(File.Open(path, FileMode.Create));
				QBundleInfoGenerator.WriteClass(writer, "QAssetBundle", AssetBundleInfos);
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
