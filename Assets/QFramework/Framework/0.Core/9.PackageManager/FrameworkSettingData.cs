/****************************************************************************
 * Copyright (c) 2017 magicbell
 * Copyright (c) 2018.3 ~ 7 liangxie
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
#if UNITY_EDITOR
	using UnityEditor;
#endif
	[Serializable]
	public class FrameworkSettingData
	{
#if UNITY_EDITOR
		static string mConfigSavedDir
		{
			get { return (Application.dataPath + "/QFrameworkData/").CreateDirIfNotExists() + "ProjectConfig/"; }
		}

		private const string mConfigSavedFileName = "ProjectConfig.json";

		public string Namespace;

		public string UIScriptDir = "/Scripts/UI";

		public string UIPrefabDir = "/Art/UIPrefab";

		public static FrameworkSettingData Load()
		{
			mConfigSavedDir.CreateDirIfNotExists();

			if (!File.Exists(mConfigSavedDir + mConfigSavedFileName))
			{
				using (var fileStream = File.Create(mConfigSavedDir + mConfigSavedFileName))
				{
					fileStream.Close();
				}
			}

			var frameworkConfigData = SerializeHelper.LoadJson<FrameworkSettingData>(mConfigSavedDir + mConfigSavedFileName);

			if (frameworkConfigData == null || string.IsNullOrEmpty(frameworkConfigData.Namespace))
			{
				frameworkConfigData = new FrameworkSettingData {Namespace = "QFramework.Example"};
			}

			return frameworkConfigData;
		}

		public void Save()
		{
			this.SaveJson(mConfigSavedDir + mConfigSavedFileName);
			AssetDatabase.Refresh();

		}
#endif
	}
}