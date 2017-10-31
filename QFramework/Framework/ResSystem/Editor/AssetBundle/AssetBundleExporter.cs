

using UnityEditor;

using System.Collections.Generic;
using System.IO;
using QFramework;
using QFramework;

namespace QFramework
{
    public class AssetBundleExporter
    {
		public static void BuildDataTable()
		{
			Log.I("Start BuildAssetDataTable!");
			AssetDataTable table = AssetDataTable.Create();

			ProcessAssetBundleRes(table);

		    string filePath =
		        IOUtils.CreateDirIfNotExists(FilePath.StreamingAssetsPath + QFrameworkConfigData.RELATIVE_AB_ROOT_FOLDER) +
		        QFrameworkConfigData.EXPORT_ASSETBUNDLE_CONFIG_FILENAME;
			table.Save(filePath);
			AssetDatabase.Refresh ();
		}

#region 处理AssetBundle Name
        //自动设置选中目录下的AssetBundle Name
        public static void GenAssetNameAsFolderName()
        {
            string selectPath = EditorUtils.GetSelectedDirAssetsPath();
            if (selectPath == null)
            {
                Log.W("Not Select Any Folder!");
                return;
            }

            AutoGenAssetNameInFolder(selectPath, true);
            Log.I("Finish GenAssetNameAsFolderName.");
        }

        //自动设置选中目录下的AssetBundle Name
        public static void GenAssetNameAsFileName()
        {
            string selectPath = EditorUtils.GetSelectedDirAssetsPath();
            if (selectPath == null)
            {
                Log.W("Not Select Any Folder!");
                return;
            }

            AutoGenAssetNameInFolder(selectPath, false);

            AssetDatabase.SaveAssets();
            Log.I("Finish GenAssetNameAsFileName.");
        }

        /// <summary>
        // 递归处理文件夹下所有Asset 文件
        /// </summary>
        /// <param name="folderPath">Asset目录下文件夹</param>
        private static void AutoGenAssetNameInFolder(string folderPath, bool useFolderName)
        {
            if (folderPath == null)
            {
                Log.W("Folder Path Is Null!");
                return;
            }

            Log.I("Start Set Asset Name. Folder:" + folderPath);
            string workPath = EditorUtils.AssetsPath2ABSPath(folderPath); //EditUtils.GetFullPath4AssetsPath(folderPath);
            string assetBundleName = EditorUtils.AssetPath2ReltivePath(folderPath).ToLower(); //EditUtils.GetReltivePath4AssetPath(folderPath).ToLower();
            assetBundleName = assetBundleName.Replace("resources/", "");
            //处理文件
            var filePaths = Directory.GetFiles(workPath);
            for (int i = 0; i < filePaths.Length; ++i)
            {
                if (!AssetFileFilter.IsAsset(filePaths[i]))
                {
                    continue;
                }

                string fileName = Path.GetFileName(filePaths[i]);

                string fullFileName = string.Format("{0}/{1}", folderPath, fileName);

                AssetImporter ai = AssetImporter.GetAtPath(fullFileName);
                if (ai == null)
                {
                    Log.E("Not Find Asset:" + fullFileName);
                    return;
                }
                else
                {
                    if (useFolderName)
                    {
                        ai.assetBundleName = assetBundleName;
                    }
                    else
                    {
                        ai.assetBundleName = string.Format("{0}/{1}", assetBundleName, PathHelper.FileNameWithoutSuffix(fileName));
                    }
                }
                
                //ai.SaveAndReimport();
                //Log.I("Success Process Asset:" + fileName);
            }

            //递归处理文件夹
            var dirs = Directory.GetDirectories(workPath);
            for (int i = 0; i < dirs.Length; ++i)
            {
                string fileName = Path.GetFileName(dirs[i]);

                fileName = string.Format("{0}/{1}", folderPath, fileName);
                AutoGenAssetNameInFolder(fileName, useFolderName);
            }
        }
#endregion

#region 指定具体文件构建
        public static void BuildAssetBundlesInSelectFolder()
        {
            string selectPath = EditorUtils.GetSelectedDirAssetsPath();//.CurrentSelectFolder;
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
            string fullFolderPath = EditorUtils.AssetsPath2ABSPath(folderPath);//EditUtils.GetFullPath4AssetsPath(folderPath);
            string assetBundleName = EditorUtils.AssetPath2ReltivePath(folderPath);// EditUtils.GetReltivePath4AssetPath(folderPath);
            var filePaths = Directory.GetFiles(fullFolderPath);

            AssetBundleBuild abb = new AssetBundleBuild();
            abb.assetBundleName = assetBundleName;

            List<string> fileNameList = new List<string>();

            for (int i = 0; i < filePaths.Length; ++i)
            {
                if (!AssetFileFilter.IsAsset(filePaths[i]))
                {
                    continue;
                }

                string fileName = Path.GetFileName(filePaths[i]);
                fileName = string.Format("{0}/{1}", folderPath, fileName);
                fileNameList.Add(fileName);
            }

            if (fileNameList.Count <= 0)
            {
                Log.W("Not Find Asset In Folder:" + folderPath);
                return;
            }

            abb.assetNames = fileNameList.ToArray();
			BuildPipeline.BuildAssetBundles(QFrameworkConfigData.EDITOR_AB_EXPORT_ROOT_FOLDER,
                new AssetBundleBuild[1] { abb },
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

        private static void ProcessAssetBundleRes(AssetDataTable table)
        {
            AssetDataGroup group = null;

			int abIndex = table.AddAssetBundleName(QFrameworkConfigData.ABMANIFEST_AB_NAME, null, out group);

            if (abIndex > 0)
            {
				group.AddAssetData(new AssetData(QFrameworkConfigData.ABMANIFEST_ASSET_NAME, ResType.ABAsset, abIndex,null));
            }

            AssetDatabase.RemoveUnusedAssetBundleNames();

            string[] abNames = AssetDatabase.GetAllAssetBundleNames();
            if (abNames != null && abNames.Length > 0)
            {
                for (int i = 0; i < abNames.Length; ++i)
                {
                    string[] depends = AssetDatabase.GetAssetBundleDependencies(abNames[i], false);
                    abIndex = table.AddAssetBundleName(abNames[i], depends, out group);
                    if (abIndex < 0)
                    {
                        continue;
                    }

                    string[] assets = AssetDatabase.GetAssetPathsFromAssetBundle(abNames[i]);
                    foreach (var cell in assets)
                    {
                        if (cell.EndsWith(".unity"))
                        {
                            group.AddAssetData(new AssetData(AssetPath2Name(cell), ResType.ABScene, abIndex,abNames[i]));
                        }
                        else
                        {
                            group.AddAssetData(new AssetData(AssetPath2Name(cell), ResType.ABAsset, abIndex,abNames[i]));
                        }
                    }
                }
            }

            table.Dump();
        }
#endregion

    }
}
