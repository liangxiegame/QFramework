/****************************************************************************
 * Copyright (c) 2017 ~ 2024 liangxiegame UNDER MIT LINCESE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/


using System.Linq;

namespace QFramework
{
    using UnityEngine;
    using UnityEditor;
    using System.IO;

    public class AssetBundleInfo
    {
        public readonly string Name;

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

            if (ResKitView.AppendHash)
            {
                BuildPipeline.BuildAssetBundles(outputPath, defaultSubProjectData.Builds.ToArray(),
                    BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.AppendHashToAssetBundleName,
                    buildTarget);
            }
            else
            {
                BuildPipeline.BuildAssetBundles(outputPath, defaultSubProjectData.Builds.ToArray(),
                    BuildAssetBundleOptions.ChunkBasedCompression,
                    buildTarget);
            }

            GenerateVersionConfig();

            var finalDir = Application.streamingAssetsPath + "/AssetBundles/" + GetPlatformName();

            finalDir.DeleteDirIfExists();
            finalDir.CreateDirIfNotExists();

            FileUtil.ReplaceDirectory(outputPath, finalDir);

            AssetBundleExporter.BuildDataTable(defaultSubProjectData.Builds.Select(b => b.assetBundleName).ToArray(),appendHash:ResKitView.AppendHash);

            // foreach (var subProjectData in subProjectDatas)
            // {
            //     outputPath = Path.Combine(ResKitAssetsMenu.AssetBundlesOutputPath + "/" + subProjectData.Name,
            //         GetPlatformName());
            //     outputPath.CreateDirIfNotExists();
            //
            //     BuildPipeline.BuildAssetBundles(outputPath, subProjectData.Builds.ToArray(),
            //         BuildAssetBundleOptions.ChunkBasedCompression, buildTarget);
            //     finalDir = Application.streamingAssetsPath + "/" + subProjectData.Name + "/AssetBundles/" +
            //                GetPlatformName();
            //
            //     finalDir.DeleteDirIfExists();
            //     finalDir.CreateDirIfNotExists();
            //
            //     FileUtil.ReplaceDirectory(outputPath, finalDir);
            //     AssetBundleExporter.BuildDataTable(subProjectData.Builds.Select(b => b.assetBundleName).ToArray(),
            //         finalDir + "/");
            // }

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
            return AssetBundlePathHelper.GetPlatformName();
        }
    }
}