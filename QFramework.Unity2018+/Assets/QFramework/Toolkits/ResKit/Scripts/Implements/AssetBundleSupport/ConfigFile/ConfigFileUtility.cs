using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

namespace QFramework
{
    public class ConfigFileUtility
    {
        public static ResDatas BuildEditorDataTable()
        {
            Debug.Log("Start BuildAssetDataTable!");
            var resDatas = new ResDatas();
            AddABInfo2ResDatas(resDatas);
            return resDatas;
        }

        public static void AddABInfo2ResDatas(IResDatas assetBundleConfigFile, string[] abNames = null)
        {
#if UNITY_EDITOR
            AssetDatabase.RemoveUnusedAssetBundleNames();

            var assetBundleNames = abNames ?? AssetDatabase.GetAllAssetBundleNames();
            foreach (var abName in assetBundleNames)
            {
                var depends = AssetDatabase.GetAssetBundleDependencies(abName, false);

                AssetDataGroup group;
                var abIndex = assetBundleConfigFile.AddAssetBundleName(abName, depends, out @group);
                if (abIndex < 0)
                {
                    continue;
                }

                var assets = AssetDatabase.GetAssetPathsFromAssetBundle(abName);
                foreach (var cell in assets)
                {
                    var type = AssetDatabase.GetMainAssetTypeAtPath(cell);

                    var code = type.ToCode();

                    @group.AddAssetData((cell.EndsWith(".unity") || cell.EndsWith(".scene"))
                        ? new AssetData(AssetPath2Name(cell), ResLoadType.ABScene, abIndex, abName, code)
                        : new AssetData(AssetPath2Name(cell), ResLoadType.ABAsset, abIndex, abName, code));
                }
            }
#endif
        }

        public static void AddABInfo2ResDatasAppendHash(IResDatas assetBundleConfigFile, string[] abNames)
        {
#if UNITY_EDITOR
            AssetDatabase.RemoveUnusedAssetBundleNames();

            var abRootPath = Application.streamingAssetsPath + AssetBundleSettings.RELATIVE_AB_ROOT_FOLDER;
            var abFileNames = Directory.GetFiles(abRootPath)
                .Where(p => !p.EndsWith(".meta") && !p.EndsWith(".manifest") && !p.EndsWith(".bin") && p.GetFileName() != AssetBundlePathHelper.GetPlatformName());
            var abNameAndHash = new Dictionary<string, string>();

            foreach (var abFileName in abFileNames)
            {
                var abFileNamePart = abFileName.Split('_');
                if (abFileNamePart.Length > 1)
                {
                    var hash = abFileNamePart.Last();
                    var abName = abFileName.GetFileName().RemoveString("_" + hash);
                    abNameAndHash.Add(abName,hash);
                }
            }


            var assetBundleNames = abNames ?? AssetDatabase.GetAllAssetBundleNames();
            foreach (var abName in assetBundleNames)
            {
                var depends = AssetDatabase.GetAssetBundleDependencies(abName, false);

                AssetDataGroup group;
                var abIndex = assetBundleConfigFile.AddAssetBundleName(abName, depends, out @group);
                if (abIndex < 0)
                {
                    continue;
                }
                

                var assets = AssetDatabase.GetAssetPathsFromAssetBundle(abName);
                foreach (var cell in assets)
                {
                    var type = AssetDatabase.GetMainAssetTypeAtPath(cell);

                    var code = type.ToCode();

                    @group.AddAssetData((cell.EndsWith(".unity") || cell.EndsWith(".scene"))
                        ? new AssetData(AssetPath2Name(cell), ResLoadType.ABScene, abIndex, abName, code)
                        : new AssetData(AssetPath2Name(cell), ResLoadType.ABAsset, abIndex, abName, code));
                }
                
                foreach (var groupAssetBundleData in group.AssetBundleDatas)
                {
                    groupAssetBundleData.Hash = abNameAndHash[groupAssetBundleData.abName];
                }
            }
            
#endif
        }

        public static string AssetPath2Name(string assetPath)
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
    }
}