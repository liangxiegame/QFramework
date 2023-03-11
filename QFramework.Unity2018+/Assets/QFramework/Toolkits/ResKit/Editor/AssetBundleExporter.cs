/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 ~ 2018.5 liangxie
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

using UnityEditor;
using UnityEngine;

namespace QFramework
{
	public static class AssetBundleExporter
	{
		public static void BuildDataTable(string[] abNames = null,string outputPath = null)
		{
			Debug.Log("Start Default BuildAssetDataTable!");
			var table = new ResDatas();
			ConfigFileUtility.AddABInfo2ResDatas(table, abNames);

			var filePath =
				(outputPath ?? (AssetBundlePathHelper.StreamingAssetsPath + AssetBundleSettings.RELATIVE_AB_ROOT_FOLDER)).CreateDirIfNotExists() +
				ResDatas.FileName;
			
			table.Save(filePath);
			AssetDatabase.Refresh();
		}
	}
}
