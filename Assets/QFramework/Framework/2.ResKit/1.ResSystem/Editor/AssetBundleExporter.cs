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

using System.Collections.Generic;
using System.IO;
using QF.Extensions;
using QFramework;

namespace QF.Res
{
    public static class AssetBundleExporter
    {
		public static void BuildDataTable()
		{
			Log.I("Start BuildAssetDataTable!");
			ResDatas table = ResDatas.Create();

			ProcessAssetBundleRes(table);

		    var filePath =
		        (FilePath.StreamingAssetsPath + ResKitUtil.RELATIVE_AB_ROOT_FOLDER).CreateDirIfNotExists() +
		        ResKitUtil.EXPORT_ASSETBUNDLE_CONFIG_FILENAME;
			table.Save(filePath);
			AssetDatabase.Refresh ();
		}

#region 指定具体文件构建
        public static void BuildAssetBundlesInSelectFolder()
        {
            var selectPath = EditorUtils.GetSelectedDirAssetsPath();//.CurrentSelectFolder;
            if (selectPath == null)
            {
                Log.W("Not Select Any Folder!");
                return;
            }

            BuildAssetBundlesInFolder(selectPath);
        }

        private static void BuildAssetBundlesInFolder(string folderPath)
        {
            if (folderPath == null)
            {
                Log.W("Folder Path Is Null.");
                return;
            }

            Log.I("Start Build AssetBundle:" + folderPath);
            var fullFolderPath = EditorUtils.AssetsPath2ABSPath(folderPath);//EditUtils.GetFullPath4AssetsPath(folderPath);
            var assetBundleName = EditorUtils.AssetPath2ReltivePath(folderPath);// EditUtils.GetReltivePath4AssetPath(folderPath);
            var filePaths = Directory.GetFiles(fullFolderPath);

            AssetBundleBuild abb = new AssetBundleBuild();
            abb.assetBundleName = assetBundleName;

            List<string> fileNameList = new List<string>();

            foreach (var filePath in filePaths)
            {
                if (!filePath.EndsWith(".meta"))
                {
                    continue;
                }

                var fileName = Path.GetFileName(filePath);
                fileName = string.Format("{0}/{1}", folderPath, fileName);
                fileNameList.Add(fileName);
            }

            if (fileNameList.Count <= 0)
            {
                Log.W("Not Find Asset In Folder:" + folderPath);
                return;
            }

            abb.assetNames = fileNameList.ToArray();
			BuildPipeline.BuildAssetBundles(ResKitUtil.EDITOR_AB_EXPORT_ROOT_FOLDER,
                new[] { abb },
                BuildAssetBundleOptions.ChunkBasedCompression,
                BuildTarget.StandaloneWindows);
        }

#endregion

#region 构建 AssetDataTable

        private static string AssetPath2Name(string assetPath)
        {
            int startIndex = assetPath.LastIndexOf("/") + 1;
            int endIndex = assetPath.LastIndexOf(".");

            if (endIndex > 0)
            {
                int length = endIndex - startIndex;
                return assetPath.Substring(startIndex, length).ToLower();
            }

            return assetPath.Substring(startIndex).ToLower();
        }

        private static void ProcessAssetBundleRes(ResDatas table)
        {
            AssetDataGroup group = null;

            AssetDatabase.RemoveUnusedAssetBundleNames();

            string[] abNames = AssetDatabase.GetAllAssetBundleNames();
            if (abNames != null && abNames.Length > 0)
            {
                foreach (var abName in abNames)
                {
                    var depends = AssetDatabase.GetAssetBundleDependencies(abName, false);
                    var abIndex = table.AddAssetBundleName(abName, depends, out @group);
                    if (abIndex < 0)
                    {
                        continue;
                    }

                    var assets = AssetDatabase.GetAssetPathsFromAssetBundle(abName);
                    foreach (var cell in assets)
                    {
                        @group.AddAssetData(cell.EndsWith(".unity")
                            ? new AssetData(AssetPath2Name(cell), ResType.ABScene, abIndex, abName)
                            : new AssetData(AssetPath2Name(cell), ResType.ABAsset, abIndex, abName));
                    }
                }
            }

        }
#endregion

    }
}
