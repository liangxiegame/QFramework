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
	public class UpdatePackage : NodeAction
	{
		private readonly string mUrl;
		private readonly string mPackageName;

		public UpdatePackage(string url, string packageName)
		{
			mUrl = url;
			mPackageName = packageName;
		}

		protected override void OnBegin()
		{
			base.OnBegin();

			string tempFile = mPackageName + ".unitypackage";

			Debug.Log(mUrl + ">>>>>>:");

			EditorUtility.DisplayProgressBar("插件更新", "插件下载中....", 0.5f);

			WebClient client = new WebClient();

			client.DownloadFile(new Uri(mUrl), tempFile);

			EditorUtility.ClearProgressBar();

			AssetDatabase.ImportPackage(tempFile, true);

			File.Delete(tempFile);
		}
	}
}
