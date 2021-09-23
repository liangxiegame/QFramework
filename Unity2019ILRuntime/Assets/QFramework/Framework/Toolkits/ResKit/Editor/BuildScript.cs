/****************************************************************************
 * Copyright (c) 2017 ~ 2021.4 liangxie
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
 * The above copyright notice and this permission notice       shall be included in
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


using System.Linq;

namespace QFramework
{
    using UnityEngine;
    using UnityEditor;
    using System.IO;

    public class AssetBundleInfo
    {
        public readonly string Name = "";

        public AssetBundleInfo(string name)
        {
            this.Name = name;
        }

        public string[] assets;
    }


    public static class BuildScript
    {
        public static void BuildAssetBundles(BuildTarget buildTarget)
        {
            // 先清空一下没用的 ab 名字
            AssetDatabase.RemoveUnusedAssetBundleNames();

            var defaultSubProjectData = new SubProjectData();
            var subProjectDatas = SubProjectData.SearchAllInProject();
            SubProjectData.SplitAssetBundles2DefaultAndSubProjectDatas(defaultSubProjectData, subProjectDatas);

            // Choose the output path according to the build target.
            var outputPath = Path.Combine(ResKitAssetsMenu.AssetBundlesOutputPath, GetPlatformName());
            outputPath.CreateDirIfNotExists();

            BuildPipeline.BuildAssetBundles(outputPath, defaultSubProjectData.Builds.ToArray(),
                BuildAssetBundleOptions.ChunkBasedCompression, buildTarget);

            GenerateVersionConfig();

            var finalDir = Application.streamingAssetsPath + "/AssetBundles/" + GetPlatformName();

            finalDir.DeleteDirIfExists();
            finalDir.CreateDirIfNotExists();

            FileUtil.ReplaceDirectory(outputPath, finalDir);

            AssetBundleExporter.BuildDataTable(defaultSubProjectData.Builds.Select(b => b.assetBundleName).ToArray());

            foreach (var subProjectData in subProjectDatas)
            {
                outputPath = Path.Combine(ResKitAssetsMenu.AssetBundlesOutputPath + "/" + subProjectData.Name,
                    GetPlatformName());
                outputPath.CreateDirIfNotExists();

                BuildPipeline.BuildAssetBundles(outputPath, subProjectData.Builds.ToArray(),
                    BuildAssetBundleOptions.ChunkBasedCompression, buildTarget);
                finalDir = Application.streamingAssetsPath + "/" + subProjectData.Name + "/AssetBundles/" +
                           GetPlatformName();

                finalDir.DeleteDirIfExists();
                finalDir.CreateDirIfNotExists();

                FileUtil.ReplaceDirectory(outputPath, finalDir);
                AssetBundleExporter.BuildDataTable(subProjectData.Builds.Select(b => b.assetBundleName).ToArray(),
                    finalDir + "/");
            }

            AssetDatabase.Refresh();
        }

        private static void GenerateVersionConfig()
        {
            if (ResKitEditorWindow.EnableGenerateClass)
            {
                WriteClass();
            }
        }

        public static void WriteClass()
        {
            "Assets/QFrameworkData".CreateDirIfNotExists();

            var path = Path.GetFullPath(
                Application.dataPath + Path.DirectorySeparatorChar + "QFrameworkData/QAssets.cs");
            var writer = new StreamWriter(File.Open(path, FileMode.Create));
            ResDataCodeGenerator.WriteClass(writer, "QAssetBundle");
            writer.Close();
            AssetDatabase.Refresh();
        }


        private static string GetPlatformName()
        {
            return FromUnityToDll.Setting.GetPlatformName();
        }
    }
}