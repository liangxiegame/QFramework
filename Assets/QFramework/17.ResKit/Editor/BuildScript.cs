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

		public string name = "";

		public AssetBundleInfo(string name)
		{
			this.name = name;
		}

		public string[] assets;

	}

	public class BuildScript
	{
		public static string overloadedDevelopmentServerURL = "";

		private static string projectTag = "putaogame";

		public static void SetProjectTag()
		{

			string projectMark = "_project_";
			var allAbPaths = AssetDatabase.GetAllAssetPaths();
			foreach (string path in allAbPaths)
			{
				if (string.IsNullOrEmpty(path))
				{
					continue;
				}
				AssetImporter ai = AssetImporter.GetAtPath(path);
				if (!string.IsNullOrEmpty(ai.assetBundleName))
				{
					string abName = ai.assetBundleName;
					int lastIndex = abName.LastIndexOf(projectMark);
					if (lastIndex >= 0)
					{
						abName = abName.Remove(lastIndex, abName.Length - lastIndex);
					}
					Debug.Log(abName + ":abname:" + projectTag + ":bbb:" + string.IsNullOrEmpty(projectTag));
					if (string.IsNullOrEmpty(projectTag))
					{
						ai.assetBundleName = abName;
					}
					else
					{
						ai.assetBundleName = abName + projectMark + projectTag;
					}

				}
				AssetDatabase.SaveAssets();

			}
		}

		public static void BuildAssetBundles(BuildTarget buildTarget, string inputProjectTag)
		{
			if (string.IsNullOrEmpty(inputProjectTag))
			{
				projectTag = "putaogame";
			}
			else
			{
				projectTag = inputProjectTag;
			}
			SetProjectTag();

			// Choose the output path according to the build target.
			string outputPath = Path.Combine(QResSystemMark.AssetBundlesOutputPath, GetPlatformName());

			outputPath = outputPath + "/" + projectTag;

			IOExtension.CreateDirIfNotExists(outputPath);

			BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.ChunkBasedCompression, buildTarget);

			List<string> finalzips = PackZips(outputPath);
			List<string> finalFiles = PackQFiles(outputPath);

			GenerateVersionConfig(outputPath, finalzips, finalFiles);

			string finalDir = Application.streamingAssetsPath + "/AssetBundles/" + GetPlatformName() + "/" + projectTag;

			IOExtension.DeleteDirIfExists(finalDir);

			IOExtension.CreateDirIfNotExists(finalDir);

			FileUtil.ReplaceDirectory(outputPath, finalDir);
			
			AssetBundleExporter.BuildDataTable();

			AssetDatabase.Refresh();
		}

		/// <summary>
		/// 遍历压缩所有zip文件
		/// </summary>
		/// <returns>The zips.</returns>
		/// <param name="outpath">Outpath.</param>
		public static List<string> PackZips(string outpath)
		{
			List<string> finalZipFiles = new List<string>();
			var allDirs = AssetDatabase.GetAllAssetPaths().Where(path => (Directory.Exists(path)));
			foreach (var path in allDirs)
			{
				UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
				if (AssetLabelHelper.HasAssetLabel(obj, QResSystemMark.LABEL_ZIP))
				{
					PackFiles(outpath + "/" + Path.GetFileName(path) + ".zip", path);
					finalZipFiles.Add(outpath + "/" + Path.GetFileName(path) + ".zip");
				}
			}
			return finalZipFiles;
		}

		/// <summary>
		/// 遍历复制所有标记了file的文件
		/// </summary>
		/// <returns>The point files.</returns>
		/// <param name="outpath">Outpath.</param>
		public static List<string> PackQFiles(string outpath)
		{

			List<string> finalMarkedFiles = new List<string>();
			var allFiles = AssetDatabase.GetAllAssetPaths().Where(path => (File.Exists(path)));
			List<string> files = new List<string>();
			foreach (var k in allFiles)
			{
				UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(k, typeof(UnityEngine.Object));
				if (AssetLabelHelper.HasAssetLabel(obj, QResSystemMark.LABEL_FILE))
				{
					files.Add(k);
					FileUtil.ReplaceFile(k, outpath + "/" + Path.GetFileName(k));
					finalMarkedFiles.Add(outpath + "/" + Path.GetFileName(k));
				}
			}

			return finalMarkedFiles;
		}

		/// <summary>
		/// MAC上再文件夹内容一致的情况下用fastzip压缩出来的zip文件 md5不一致，所以单独用shell脚本处理。
		/// </summary>
		/// <param name="filename">Filename.</param>
		/// <param name="directory">Directory.</param>
		public static void PackFiles(string filename, string directory)
		{
			try
			{
				if (Application.platform == RuntimePlatform.OSXEditor)
				{
					Debug.Log("pack files in osx editor ***********");
					string shell = Application.dataPath + "/QUGame/QFramework/ResSystem/Editor/shell.sh";
					Debug.Log(File.Exists(shell));
					string arg1 = Path.GetFullPath(filename);
					string arg2 = Path.GetFullPath(directory);
					Debug.Log(arg1 + ":");
					Debug.Log(arg2 + ":");
					string argss = shell + " " + arg1 + " " + arg2;
					if (File.Exists(arg1))
					{
						File.Delete(arg1);
					}
					System.Diagnostics.Process sampleProcess = new System.Diagnostics.Process();
					sampleProcess.StartInfo.FileName = "/bin/bash"; //IE浏览器，可以更换
					sampleProcess.StartInfo.Arguments = argss;
					sampleProcess.Start();
					sampleProcess.WaitForExit();

				}
				else
				{
					Debug.Log("pack files in windows editor ***********");
					FastZip fz = new FastZip();
					fz.CreateEmptyDirectories = true;
					fz.CreateZip(filename, directory, true, "");
					fz = null;
				}

			}
			catch (Exception e)
			{
				Debug.Log(e);
				throw;
			}
		}

		private static void GenerateVersionConfig(string outputPath, List<string> finalZips, List<string> finalFiles)
		{
			string abManifestFile;
			if (projectTag != "")
				abManifestFile = Path.Combine(outputPath, projectTag);
			else
				abManifestFile = Path.Combine(outputPath, GetPlatformName());

			AssetBundle ab = AssetBundle.LoadFromFile(abManifestFile);

			AssetBundleManifest abMainfest = (AssetBundleManifest) ab.LoadAsset("AssetBundleManifest");
			string[] allABNames = abMainfest.GetAllAssetBundles();
			XmlDocument xmlDoc = new XmlDocument();
			XmlElement xmlRoot = xmlDoc.CreateElement("config");
			xmlRoot.SetAttribute("res_version", QResSystemBuilder.resVersion);
			xmlDoc.AppendChild(xmlRoot);
			assetBundleInfos.Clear();
			for (int i = 0; i < allABNames.Length; i++)
			{

				XmlElement xmlItem =
					CreateConfigItem(xmlDoc, Path.Combine(outputPath, allABNames[i]), allABNames[i], allABNames[i]);
				xmlRoot.AppendChild(xmlItem);

				AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(outputPath, allABNames[i]));
				AssetBundleInfo abInfo = new AssetBundleInfo(allABNames[i]);
				abInfo.assets = assetBundle.GetAllAssetNames();
				assetBundleInfos.Add(abInfo);
				assetBundle.Unload(true);

			}
			// 这里要加上平台相关的xml
			string platformBundleName = GetPlatformName();
			XmlElement platformItem;
			if (projectTag == "")
			{
				platformItem = CreateConfigItem(xmlDoc, abManifestFile, platformBundleName, platformBundleName);
			}
			else
			{
				platformItem = CreateConfigItem(xmlDoc, abManifestFile, projectTag, projectTag);
			}
			xmlRoot.AppendChild(platformItem);

			foreach (var zipPath in finalZips)
			{
				XmlElement zipItem = CreateConfigItem(xmlDoc, zipPath, Path.GetFileName(zipPath), Path.GetFileName(zipPath));
				xmlRoot.AppendChild(zipItem);
			}
			foreach (var filePath in finalFiles)
			{
				XmlElement fileItem = CreateConfigItem(xmlDoc, filePath, Path.GetFileName(filePath), Path.GetFileName(filePath));
				xmlRoot.AppendChild(fileItem);
			}
						
			XmlElement filTiem = CreateConfigItem(xmlDoc,
				Application.streamingAssetsPath + QFrameworkConfigData.RELATIVE_AB_ROOT_FOLDER +
				QFrameworkConfigData.EXPORT_ASSETBUNDLE_CONFIG_FILENAME,
				QFrameworkConfigData.EXPORT_ASSETBUNDLE_CONFIG_FILENAME,
				QFrameworkConfigData.EXPORT_ASSETBUNDLE_CONFIG_FILENAME);

			xmlRoot.AppendChild(filTiem);
			
			ab.Unload(true);

			xmlDoc.Save(outputPath + "/resconfig.xml");
			AssetDatabase.Refresh();

			if (QResSystemBuilder.isEnableGenerateClass)
			{
				if (!Directory.Exists("QFrameworkData"))
				{
                    Directory.CreateDirectory("QFrameworkData");
				}

                var path = Path.GetFullPath(Application.dataPath + Path.DirectorySeparatorChar + "QFrameworkData/QAssets.cs");
				StreamWriter writer = new StreamWriter(File.Open(path, FileMode.Create));
				QBundleInfoGenerator.WriteClass(writer, "QAssetBundle", assetBundleInfos, projectTag);
				writer.Close();
				AssetDatabase.Refresh();
			}
		}

		private static XmlElement CreateConfigItem(XmlDocument xmlDoc, string filePath, string fileName, string finalPath)
		{
			XmlElement platformItem = xmlDoc.CreateElement("item");

			platformItem.SetAttribute("name", fileName);
			platformItem.SetAttribute("path", finalPath);

			byte[] platformFileBytes = getFileBytes(filePath);
			platformItem.SetAttribute("hash", getMD5(platformFileBytes));
			platformItem.SetAttribute("size", getSize(platformFileBytes));
			return platformItem;
		}

		private static List<AssetBundleInfo> assetBundleInfos = new List<AssetBundleInfo>();

		public static string GetPlatformName()
		{
			return PlatformUtil.GetPlatformForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
		}

		static void CopyAssetBundlesTo(string outputPath)
		{
			// Clear streaming assets folder.
			FileUtil.DeleteFileOrDirectory(Application.streamingAssetsPath);
			Directory.CreateDirectory(outputPath);

			string outputFolder = GetPlatformName();

			// Setup the source folder for assetbundles.
			var source =
				Path.Combine(Path.Combine(System.Environment.CurrentDirectory, QResSystemMark.AssetBundlesOutputPath),
					outputFolder);
			if (!System.IO.Directory.Exists(source))
				Debug.Log("No assetBundle output folder, try to build the assetBundles first.");

			// Setup the destination folder for assetbundles.
			var destination = System.IO.Path.Combine(outputPath, outputFolder);
			if (System.IO.Directory.Exists(destination))
				FileUtil.DeleteFileOrDirectory(destination);

			FileUtil.CopyFileOrDirectory(source, destination);
		}

		static string[] GetLevelsFromBuildSettings()
		{
			List<string> levels = new List<string>();
			for (int i = 0; i < EditorBuildSettings.scenes.Length; ++i)
			{
				if (EditorBuildSettings.scenes[i].enabled)
					levels.Add(EditorBuildSettings.scenes[i].path);
			}

			return levels.ToArray();
		}


		public static byte[] getFileBytes(string filePath)
		{
			FileStream fs = new FileStream(filePath, FileMode.Open);
			int len = (int) fs.Length;
			byte[] data = new byte[len];
			fs.Read(data, 0, len);
			fs.Close();
			return data;
		}

		public static string getMD5(byte[] data)
		{
			MD5 md5 = new MD5CryptoServiceProvider();
			byte[] result = md5.ComputeHash(data);
			string fileMD5 = "";
			foreach (byte b in result)
			{
				fileMD5 += Convert.ToString(b, 16);
			}
			return fileMD5;
		}

		public static string getSize(byte[] data)
		{
			return data.Length.ToString();
		}
	}
}