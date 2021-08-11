/****************************************************************************
 * Copyright (c) 2017 magicbell
 * Copyright (c) 2018.3 ~ 2021.1 liangxie
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
using System.IO;

using UnityEngine;

namespace QFramework
{
	using UnityEditor;

	[Serializable]
	public class UIKitSettingData
	{
		static string mConfigSavedDir
		{
			get { return (Application.dataPath + "/QFrameworkData/").CreateDirIfNotExists() + "ProjectConfig/"; }
		}

		private const string mConfigSavedFileName = "ProjectConfig.json";

		public string Namespace;

		public string UIScriptDir = "/Scripts/UI";

		public string UIPrefabDir = "/Art/UIPrefab";

		public string DefaultViewControllerScriptDir = "/Scripts/Game";
		
		public string DefaultViewControllerPrefabDir = "/Art/Prefab";
		
		public bool IsDefaultNamespace
		{
			get { return Namespace == "QFramework.Example"; }
		}
		
		public static string GetScriptsPath()
		{
			return Load().UIScriptDir;
		}
		
		public static string GetProjectNamespace()
		{
			return Load().Namespace;
		}
		
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