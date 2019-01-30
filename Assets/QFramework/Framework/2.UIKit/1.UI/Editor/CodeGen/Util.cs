using System;
using System.Text;
using UnityEngine;

namespace QFramework
{
    public class CodeGenUtil
    {
        public static string PathToParent(Transform trans, string parentName)
        {
            var retValue = new StringBuilder(trans.name);

            while (trans.parent != null)
            {
                if (trans.parent.name.Equals(parentName))
                {
                    break;
                }

                retValue = trans.parent.name.Append("/").Append(retValue);

                trans = trans.parent;
            }

            return retValue.ToString();
        }
        
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
                strFilePath = uiPrefabPath.Replace(prefabDirPattern, UIKitSettingData.GetScriptsPath());

            }
            else if (uiPrefabPath.Contains("/Resources"))
            {
                strFilePath = uiPrefabPath.Replace("/Resources", UIKitSettingData.GetScriptsPath());
            }
            else
            {
                strFilePath = uiPrefabPath.Replace("/" + CodeGenUtil.GetLastDirName(uiPrefabPath), UIKitSettingData.GetScriptsPath());
            }

            strFilePath.Replace(prefabName + ".prefab", string.Empty).CreateDirIfNotExists();

            strFilePath = strFilePath.Replace(".prefab", ".cs");

            return strFilePath;
        }
    }
}