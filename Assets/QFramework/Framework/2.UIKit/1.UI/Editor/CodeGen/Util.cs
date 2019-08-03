using System;
using System.Text;
using QF.Extensions;
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
        
        
        public static string GetBindBelongs2(Bind bind)
        {
            var trans = bind.Transform;
            
            while (trans.parent != null)
            {
                if (trans.parent.GetComponent<ViewController>())
                {
                    return trans.parent.name + "(" +  trans.parent.GetComponent<ViewController>().ScriptName  + ")";
                }
                
                if (trans.parent.GetComponent<UIPanel>())
                {
                    return "UIPanel" + "(" +trans.parent.GetComponent<UIPanel>().name + ")";
                }


                trans = trans.parent;
            }

            return trans.name;
        }

        public static GameObject GetBindBelongs2GameObject(Bind bind)
        {
            var trans = bind.Transform;
            
            while (trans.parent != null)
            {
                if (trans.parent.GetComponent<ViewController>())
                {
                    return trans.parent.gameObject;
                }
                
                if (trans.parent.GetComponent<UIPanel>())
                {
                    return trans.parent.gameObject;
                }


                trans = trans.parent;
            }

            return bind.gameObject;
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