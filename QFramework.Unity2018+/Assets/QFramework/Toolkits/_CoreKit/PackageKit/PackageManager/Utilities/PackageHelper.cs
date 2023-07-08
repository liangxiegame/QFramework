/****************************************************************************
 * Copyright (c) 2015 ~ 2023 liangxiegame UNDER MIT License
 *
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System.IO;
using UnityEngine;

namespace QFramework
{
    internal class PackageHelper
    {
        public static void DeletePackageFiles(PackageVersion packageVersion)
        {
            foreach (var installedVersionIncludeFileOrFolder in packageVersion.IncludeFileOrFolders)
            {
                var path = Application.dataPath.Replace("Assets", installedVersionIncludeFileOrFolder);
                
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
                else if (File.Exists(path))
                {
                    File.Delete(path);
                }
                else if (Directory.Exists(path + "/"))
                {
                    Directory.Delete(path + "/");
                }
            }
        }
    }
}
#endif