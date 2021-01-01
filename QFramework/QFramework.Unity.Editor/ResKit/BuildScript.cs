/****************************************************************************
 * Copyright (c) 2017 ~ 2019.1 liangxie
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


using QFramework;

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

			GenerateVersionConfig();

			var finalDir = Application.streamingAssetsPath + "/AssetBundles/" + GetPlatformName();

			finalDir.DeleteDirIfExists();
			finalDir.CreateDirIfNotExists();

			FileUtil.ReplaceDirectory(outputPath, finalDir);

			AssetBundleExporter.BuildDataTable();
			AssetDatabase.Refresh();
		}

		private static void GenerateVersionConfig()
		{
			if (ResKitEditorWindow.EnableGenerateClass)
			{
				WriteClass();
			}
		}

		public static void WriteClass()
		{
			"Assets/QFrameworkData".CreateDirIfNotExists();

			var path = Path.GetFullPath(
				Application.dataPath + Path.DirectorySeparatorChar + "QFrameworkData/QAssets.cs");
			var writer = new StreamWriter(File.Open(path, FileMode.Create));
			ResDataCodeGenerator.WriteClass(writer, "QAssetBundle");
			writer.Close();
			AssetDatabase.Refresh();
		}


		private static string GetPlatformName()
		{
			return GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
		}
		
		public static string GetPlatformForAssetBundles(BuildTarget target)
		{
			switch (target)
			{
				case BuildTarget.Android:
					return "Android";
				case BuildTarget.iOS:
					return "iOS";
				case BuildTarget.WebGL:
					return "WebGL";
				case BuildTarget.StandaloneWindows:
				case BuildTarget.StandaloneWindows64:
					return "Windows";
				case BuildTarget.StandaloneLinux:
				case BuildTarget.StandaloneLinux64:
				case BuildTarget.StandaloneLinuxUniversal:
					return "Linux";
#if !UNITY_2017_3_OR_NEWER
				case BuildTarget.StandaloneOSXIntel:
				case BuildTarget.StandaloneOSXIntel64:
#elif UNITY_5
			case BuildTarget.StandaloneOSXUniversal:
#else
                case BuildTarget.StandaloneOSX:
#endif
					return "OSX";
				// Add more build targets for your own.
				// If you add more targets, don't forget to add the same platforms to GetPlatformForAssetBundles(RuntimePlatform) function.
				default:
					return null;
			}
		}
	}
}