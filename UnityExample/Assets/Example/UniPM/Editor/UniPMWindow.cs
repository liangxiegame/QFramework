/****************************************************************************
 * Copyright (c) 2017 liangxieq
 * 
 * https://github.com/UniPM/UniPM
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

using System.Diagnostics;

namespace UniPM
{
	using UnityEditor;
	using UnityEngine;
	using System.IO;
	using QFramework;

	/// <summary>
	///  TODO: 交互优化:
	/// 	1.提供右键就可以支持上传。
	/// 	2.要有个显示列表支持。
	///  TODO:功能:
	/// 	1.要提供一个全局的配置。
	/// </summary>
	public class UniPMWindow : QEditorWindow
	{
		[MenuItem("UniPM/Open")]
		static void Open()
		{
			UniPMWindow frameworkConfigEditorWindow = (UniPMWindow) GetWindow(typeof(UniPMWindow), true);
			frameworkConfigEditorWindow.titleContent = new GUIContent("UniPM");
			frameworkConfigEditorWindow.position = new Rect(Screen.width / 2, Screen.height / 2, 800, 600f);
			frameworkConfigEditorWindow.LocalConfig = PackageListConfig.GetInstalledPackageList();
			frameworkConfigEditorWindow.Init();
			frameworkConfigEditorWindow.Show();
		}

				/// <summary>
		/// 现在成功的PackageListPage
		/// </summary>
		private UIInstalledPackageListPage mInstalledPackageListPage;
		
		void Init()
		{
			mInstalledPackageListPage = new UIInstalledPackageListPage(LocalConfig);
			AddChild(mInstalledPackageListPage);
		}

		[MenuItem("Assets/UniPM/MakePackage")]
		static void MakePackage()
		{
			string packagePath = MouseSelector.GetSelectedPathOrFallback();
			string packageConfigPath = Path.Combine(packagePath, "Package.assets");

			PackageConfig packageConfig = null;
			if (File.Exists(packageConfigPath))
			{
				packageConfig = PackageConfig.LoadFromPath(packageConfigPath);
				packageConfig.PackagePath = packagePath;
			}
			else
			{
				packageConfig = new PackageConfig(packagePath);
			}
			packageConfig.SaveLocal();
		}

		[MenuItem("Assets/UniPM/UploadPackage")]
		static void UploadPackage()
		{
			
			string packagePath = MouseSelector.GetSelectedPathOrFallback();
			string packageConfigPath = packagePath.EndsWith(".asset") ? packagePath : packagePath.Append(".asset").ToString();
			if (File.Exists(packageConfigPath))
			{
				string err = string.Empty;

				PackageConfig config = PackageConfig.LoadFromPath(packageConfigPath);
				string serverUploaderPath = Application.dataPath.CombinePath(PackageListConfig.GitUrl.GetLastWord());

				if (!Directory.Exists(serverUploaderPath))
				{
					RunCommand("","git clone ".Append(PackageListConfig.GitUrl).ToString());
				}

				ZipUtil.ZipDirectory(config.PackagePath,
					IOUtils.CreateDirIfNotExists(serverUploaderPath.CombinePath(config.Name)).CombinePath(config.Name + ".zip"));
				
				PackageListConfig.GetInstalledPackageList().SaveExport();
				AssetDatabase.Refresh();
				
				RunCommand(PackageListConfig.GitUrl.GetLastWord(),
					"git add . && git commit -m \"test update\" && git push");
			}
			else
			{
				Log.W("no package.json file in folder:{0}",packagePath);
			}
		}

		[MenuItem("Assets/UniPM/Version/Update (x.0.0")]
		static void UpdateMajorVersion()
		{
			string packagePath = MouseSelector.GetSelectedPathOrFallback();
			string packageConfigPath = Path.Combine(packagePath, "Package.asset");
			if (File.Exists(packageConfigPath))
			{
				PackageConfig config = PackageConfig.LoadFromPath(packageConfigPath);
				config.UpdateVersion(0);
				config.SaveLocal();
			}
		}
		
		[MenuItem("Assets/UniPM/Version/Update (0.x.0")]
		static void UpdateMiddleVersion()
		{
			string packagePath = MouseSelector.GetSelectedPathOrFallback();
			string packageConfigPath = Path.Combine(packagePath, "Package.asset");
			if (File.Exists(packageConfigPath))
			{
				PackageConfig config = PackageConfig.LoadFromPath(packageConfigPath);
				config.UpdateVersion(1);
				config.SaveLocal();
			}
		}
		
		[MenuItem("Assets/UniPM/Version/Update (0.0.x")]
		static void UpdateSubVersion()
		{
			string packagePath = MouseSelector.GetSelectedPathOrFallback();
			string packageConfigPath = Path.Combine(packagePath, "Package.asset");
			if (File.Exists(packageConfigPath))
			{
				PackageConfig config = PackageConfig.LoadFromPath(packageConfigPath);
				config.UpdateVersion(2);
				config.SaveLocal();
			}
			else
			{
				Log.W("no package.json file in folder:{0}",packagePath);
			}
		}


		public static void RunCommand(string workingDirectory,string command)
		{
			ProcessStartInfo startInfo = new ProcessStartInfo("/bin/bash");
			startInfo.WorkingDirectory = Application.dataPath.CombinePath(workingDirectory);
			startInfo.UseShellExecute = false;
			startInfo.RedirectStandardInput = true;
			startInfo.RedirectStandardOutput = true;
			startInfo.CreateNoWindow = true;

			Process process = new Process();
			process.StartInfo = startInfo;
			process.Start();
			string[] splitCmds = command.Split("&&".ToCharArray());
			foreach (var cmd in splitCmds)
			{
				process.StandardInput.WriteLine(cmd);
			}
			process.StandardInput.WriteLine("exit"); // if no exit then WaitForExit will lockup your program
			process.StandardInput.Flush();

			string line = process.StandardOutput.ReadToEnd();

			process.WaitForExit();
			Debug.Log(line);
		}


		public PackageListConfig LocalConfig;

		public static void DownloadZip(PackageConfig config)
		{
			ObservableWWW
				.GetAndGetBytes(config.DownloadURL).Subscribe(
					bytes =>
					{
						string tempZipFile = Application.persistentDataPath + "/temp.zip";
						File.WriteAllBytes(tempZipFile, bytes);
						string err = string.Empty;
						IOUtils.DeleteDirIfExists(config.PackagePath);
						ZipUtil.UnZipFile(tempZipFile, config.PackagePath);
						File.Delete(tempZipFile);

						AssetDatabase.Refresh();
						config.SaveLocal();
					});
		}

		[MenuItem("UniPM/ExtractServer")]
		public static void ExtractGameServer()
		{
			PackageListConfig.GetInstalledPackageList().SaveExport();
		}

	}
}