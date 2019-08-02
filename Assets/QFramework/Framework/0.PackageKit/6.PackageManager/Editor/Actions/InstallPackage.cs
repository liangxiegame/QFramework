/****************************************************************************
 * Copyright (c) 2018.7 liangxie
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

using UnityEngine;
using UnityEditor;
using System.IO;
using QF.Action;
using QF.Extensions;
using UniRx;

namespace QF
{
	public class InstallPackage : NodeAction
	{
		private PackageData mRequestPackageData;

		public InstallPackage(PackageData requestPackageData)
		{
			mRequestPackageData = requestPackageData;
		}

		protected override void OnBegin()
		{
			base.OnBegin();

			var tempFile = "Assets/" + mRequestPackageData.Name + ".unitypackage";

			Debug.Log(mRequestPackageData.DownloadUrl + ">>>>>>:");

			EditorUtility.DisplayProgressBar("插件更新", "插件下载中 ...", 0.1f);

			var progressListener = new ScheduledNotifier<float>();

			ObservableWWW.GetAndGetBytes(mRequestPackageData.DownloadUrl, null, progressListener)
				.Subscribe(bytes =>
				{
					File.WriteAllBytes(tempFile, bytes);

					EditorUtility.ClearProgressBar();

					AssetDatabase.ImportPackage(tempFile, false);

					File.Delete(tempFile);

					mRequestPackageData.SaveVersionFile();

					AssetDatabase.Refresh();

					Log.I("PackageManager:插件下载成功");

					InstalledPackageVersions.Reload();

				}, e =>
				{
					EditorUtility.ClearProgressBar();

					EditorUtility.DisplayDialog(mRequestPackageData.Name, "插件安装失败,请联系 liangxiegame@163.com 或者加入 QQ 群:623597263" + e.ToString() + ";", "OK");
				});

			progressListener.Subscribe(OnProgressChanged);
		}

		private void OnProgressChanged(float progress)
		{
			EditorUtility.DisplayProgressBar("插件更新",
				"插件下载中 {0:P2}".FillFormat(progress), progress);
		}
	}
}