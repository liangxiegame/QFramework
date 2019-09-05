/****************************************************************************
 * Copyright (c) 2017 liangxie
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

#if UNITY_EDITOR
using UnityEditor;


namespace QF.Res
{
    public static class EditorRuntimeAssetDataCollector
    {
        public static ResDatas BuildDataTable()
        {
            Log.I("Start BuildAssetDataTable!");
            var resDatas = ResDatas.Create();
            ProcessAssetBundleRes(resDatas);
            return resDatas;
        }


        #region 构建 AssetDataTable

        private static string AssetPath2Name(string assetPath)
        {
            var startIndex = assetPath.LastIndexOf("/") + 1;
            var endIndex = assetPath.LastIndexOf(".");

            if (endIndex > 0)
            {
                var length = endIndex - startIndex;
                return assetPath.Substring(startIndex, length).ToLower();
            }

            return assetPath.Substring(startIndex).ToLower();
        }

        private static void ProcessAssetBundleRes(ResDatas table)
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();

            var abNames = AssetDatabase.GetAllAssetBundleNames();
            if (abNames != null && abNames.Length > 0)
            {
                foreach (var abName in abNames)
                {
                    var depends = AssetDatabase.GetAssetBundleDependencies(abName, false);
                    AssetDataGroup group;
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
#endif