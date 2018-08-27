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
using System.Net;
using System;
using UnityEditor;
using System.IO;

namespace QFramework
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

			var tempFile = mRequestPackageData.Name + ".unitypackage";

			Debug.Log(mRequestPackageData.DownloadUrl + ">>>>>>:");

			EditorUtility.DisplayProgressBar("插件更新", "插件下载中 ...", 0.5f);

			var client = new WebClient();

			client.DownloadProgressChanged += OnProgressChanged;

			client.DownloadFile(new Uri(mRequestPackageData.DownloadUrl), tempFile);

			client.DownloadProgressChanged -= OnProgressChanged;

			EditorUtility.ClearProgressBar();

			AssetDatabase.ImportPackage(tempFile, true);

			File.Delete(tempFile);
			
			mRequestPackageData.SaveVersionFile();

			AssetDatabase.Refresh();

			InstalledPackageVersions.Reload();
		}

		void OnProgressChanged(object obj, DownloadProgressChangedEventArgs args)
		{
			EditorUtility.DisplayProgressBar("插件更新",
				"插件下载中 {0} m/{1} m....".FillFormat(args.BytesReceived / 1024 / 1024,
					args.TotalBytesToReceive / 1024 / 1024), args.BytesReceived / args.TotalBytesToReceive);

		}
	}
}