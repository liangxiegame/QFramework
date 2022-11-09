/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using UnityEditor;

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
    }
}
#endif