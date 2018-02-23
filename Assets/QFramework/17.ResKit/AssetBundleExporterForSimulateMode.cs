/****************************************************************************
 * Copyright (c) 2017 liangxie
 * 
 * http://liangxiegame.com
 * https://github.com/liangxiegame/QFramework
 * https://github.com/liangxiegame/QSingleton
 * https://github.com/liangxiegame/QChain
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

#if UNITY_EDITOR
using UnityEditor;


namespace QFramework
{
    public class AssetBundleExporterForSimulateMode
    {
		public static void BuildDataTable()
		{
			Log.I("Start BuildAssetDataTable!");
			AssetDataTable table = AssetDataTable.Create();

			ProcessAssetBundleRes(table);

		    string filePath =
		        IOExtension.CreateDirIfNotExists(FilePath.StreamingAssetsPath + QFrameworkConfigData.RELATIVE_AB_ROOT_FOLDER) +
		        QFrameworkConfigData.EXPORT_ASSETBUNDLE_CONFIG_FILENAME;
			table.Save(filePath);
			AssetDatabase.Refresh ();
		}


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
#endif