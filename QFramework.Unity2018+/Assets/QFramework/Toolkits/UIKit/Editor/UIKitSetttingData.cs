/****************************************************************************
 * Copyright (c) 2015 ~ 2025 liangxiegame UNDER MIT LICENSE
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

namespace QFramework
{
	using UnityEditor;

	[Serializable]
	public class UIKitSettingData
	{
		static string mConfigSavedDir => (Application.dataPath + "/QFrameworkData/").CreateDirIfNotExists() + "ProjectConfig/";

		private const string mConfigSavedFileName = "ProjectConfig.json";

		public string Namespace;

		public string UIScriptDir = "/Scripts/UI";

		public string UIPrefabDir = "/Art/UIPrefab";

		public const string DefaultAssemblyNameToSearch = "Assembly-CSharp";
		public List<string> AssemblyNamesToSearch = new List<string>()
		{
			DefaultAssemblyNameToSearch,
		};

		public bool IsDefaultNamespace => Namespace == "QFramework.Example";


		public static UIKitSettingData Load()
		{
			mConfigSavedDir.CreateDirIfNotExists();

			if (!File.Exists(mConfigSavedDir + mConfigSavedFileName))
			{
				using (var fileStream = File.Create(mConfigSavedDir + mConfigSavedFileName))
				{
					fileStream.Close();
				}
			}

			var frameworkConfigData =
				JsonUtility.FromJson<UIKitSettingData>(File.ReadAllText(mConfigSavedDir + mConfigSavedFileName));

			if (frameworkConfigData == null || string.IsNullOrEmpty(frameworkConfigData.Namespace))
			{
				frameworkConfigData = new UIKitSettingData {Namespace = "QFramework.Example"};
			}

			return frameworkConfigData;
		}

		public void Save()
		{
			File.WriteAllText(mConfigSavedDir + mConfigSavedFileName,JsonUtility.ToJson(this));
			AssetDatabase.Refresh();
		}
	}
}