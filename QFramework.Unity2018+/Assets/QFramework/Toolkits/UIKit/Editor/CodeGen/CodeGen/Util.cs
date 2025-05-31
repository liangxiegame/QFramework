using System;

namespace QFramework
{
    public static class CodeGenUtil
    {
        
        public static string GetLastDirName(string absOrAssetsPath)
        {
            var name = absOrAssetsPath.Replace("\\", "/");
            var dirs = name.Split('/');

            return dirs[dirs.Length - 2];
        }

        public static string GenSourceFilePathFromPrefabPath(string uiPrefabPath,string prefabName)
        {
            var strFilePath = String.Empty;
            
            var prefabDirPattern = UIKitSettingData.Load().UIPrefabDir;

            if (uiPrefabPath.Contains(prefabDirPattern))
            {
                strFilePath = uiPrefabPath.Replace(prefabDirPattern, UIKitSettingData.Load().UIScriptDir);

            }
            else if (uiPrefabPath.Contains("/Resources"))
            {
                strFilePath = uiPrefabPath.Replace("/Resources", UIKitSettingData.Load().UIScriptDir);
            }
            else
            {
                strFilePath = uiPrefabPath.Replace("/" + CodeGenUtil.GetLastDirName(uiPrefabPath), UIKitSettingData.Load().UIScriptDir);
            }

            strFilePath.Replace(prefabName + ".prefab", string.Empty).CreateDirIfNotExists();

            strFilePath = strFilePath.Replace(".prefab", ".cs");

            return strFilePath;
        }
    }
}