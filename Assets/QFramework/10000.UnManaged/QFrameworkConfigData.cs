/****************************************************************************
 * Copyright (c) 2017 maoling@putao.com
 * Copyright (c) 2017 liangxie
****************************************************************************/

namespace QFramework
{
	using UnityEngine;
	using System.IO;
#if UNITY_EDITOR
	using UnityEditor;
#endif
	[System.Serializable]
	public class QFrameworkConfigData
	{
		static string mConfigSavedDir = Application.dataPath + "/QFramework/ProjectConfig/";
		static string mConfigSavedFileName = "ProjectConfig.json";

		public string Namespace = null;
		public int ResLoaderSupportIndex = 0;
		public int LuaSupportIndex = 0;
		public int CocosSupportIndex = 0;

		public string UIScriptDir = "/Scripts/UI";

		public string UIScriptDirFullPath
		{
			get { return Application.dataPath + UIScriptDir; }
		}

		public string UIPrefabDir = "/ArtRes/AssetBundles/UIPrefab";

		public string UIPrefabDirFullPath
		{
			get { return Application.dataPath + UIPrefabDir; }
		}

		public string UIFactoryFileDir = "/QFramework/Framework/Script";

		public string UIFactoryFileFullPath
		{
			get { return string.Format("{0}/{1}.cs", Application.dataPath + UIFactoryFileDir, "QUIFactory"); }
		}

		public static QFrameworkConfigData Load()
		{
			IOUtils.CreateDirIfNotExists(mConfigSavedDir);

			if (!File.Exists(mConfigSavedDir + mConfigSavedFileName))
			{
				using (var fileStream = File.Create(mConfigSavedDir + mConfigSavedFileName))
				{
					fileStream.Close();
				}
			}

			var frameworkConfigData = SerializeHelper.LoadJson<QFrameworkConfigData>(mConfigSavedDir + mConfigSavedFileName);

			if (frameworkConfigData == null || string.IsNullOrEmpty(frameworkConfigData.Namespace))
			{
				frameworkConfigData = new QFrameworkConfigData();
				frameworkConfigData.Namespace = "Putao.ProjectName";
			}

			return frameworkConfigData;
		}

		public void Save()
		{
			this.SaveJson(mConfigSavedDir + mConfigSavedFileName);
#if UNITY_EDITOR
			AssetDatabase.Refresh();
#endif
		}

		#region AssetBundle 相关

		public const string ABMANIFEST_AB_NAME = "putaogame";
		public const string ABMANIFEST_ASSET_NAME = "assetbundlemanifest";

		public static string AB_RELATIVE_PATH
		{
			get { return "AssetBundles/" + PlatformUtil.GetPlatformName() + "/putaogame/"; }
		}

		public static string AssetBundleUrl2Name(string url)
		{
			string retName = null;
			string parren = FilePath.StreamingAssetsPath + "AssetBundles/" + PlatformUtil.GetPlatformName() + "/" +
			                ABMANIFEST_AB_NAME + "/";
			retName = url.Replace(parren, "");

			parren = FilePath.PersistentDataPath + "AssetBundles/" + PlatformUtil.GetPlatformName() + "/" +
			         ABMANIFEST_AB_NAME + "/";
			retName = retName.Replace(parren, "");
			return retName;
		}

		public static string AssetBundleName2Url(string name)
		{
			string retUrl = FilePath.PersistentDataPath + "AssetBundles/" + PlatformUtil.GetPlatformName() + "/" +
			                ABMANIFEST_AB_NAME + "/" + name;
			
			if (File.Exists(retUrl))
			{
				return retUrl;
			}
			
			return FilePath.StreamingAssetsPath + "AssetBundles/" + PlatformUtil.GetPlatformName() + "/" +
			       ABMANIFEST_AB_NAME + "/" + name;
		}

		//导出目录
		public static string EDITOR_AB_EXPORT_ROOT_FOLDER
		{
			get { return "StreamingAssets/AssetBundles/" + RELATIVE_AB_ROOT_FOLDER; }
		}

		/// <summary>
		/// AssetBundle存放路径
		/// </summary>
		public static string RELATIVE_AB_ROOT_FOLDER
		{
			get { return "/AssetBundles/" + PlatformUtil.GetPlatformName() + "/putaogame/"; }
		}

		/// <summary>
		/// AssetBundle 配置路径
		/// </summary>
		public static string EXPORT_ASSETBUNDLE_CONFIG_FILENAME
		{
			get { return "asset_bindle_config.bin"; }
		}

		#endregion
	}
}