/****************************************************************************
 * Copyright (c) 2016 - 2023 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    internal class PackageMaker : Architecture<PackageMaker>
    {
        protected override void Init()
        {
            
        }

        [MenuItem("Assets/@QPM/Make Folder Package To Folder Same Path")]
        public static void MakeFolderPackageToFolderSamePath()
        {
            var activeObject = Selection.activeObject;
            var assetPath = AssetDatabase.GetAssetPath(activeObject);
            AssetDatabase.ExportPackage(assetPath,
                assetPath.GetFolderPath() + "/" + activeObject.name + ".unitypackage", ExportPackageOptions.Recurse);
            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/@QPM/Make Folder Package To Folder Same Path",true)]
        public static bool MakeFolderPackageToFolderSamePathCheck()
        {
            var activeObject = Selection.activeObject;
            if (!activeObject) return false;
            var assetPath = AssetDatabase.GetAssetPath(activeObject);
            return AssetDatabase.IsValidFolder(assetPath);
        }

        [MenuItem("Assets/@QPM/Import Package")]
        public static void ImportPackage()
        {
            var activeObject = Selection.activeObject;
            var assetPath = AssetDatabase.GetAssetPath(activeObject);
            Debug.Log(assetPath);
            AssetDatabase.ImportPackage(assetPath, true);

        }

        [MenuItem("Assets/@QPM/Import Package", true)]
        public static bool ImportPackageCheck()
        {
            var activeObject = Selection.activeObject;
            if (!activeObject) return false;
            var assetPath = AssetDatabase.GetAssetPath(activeObject);
            if (!assetPath.EndsWith(".unitypackage")) return false;
            return true;
        }
    }
}
#endif