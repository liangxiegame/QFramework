/****************************************************************************
 * Copyright (c) 2018.7 ~ 8 liangxie
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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using EditorCoroutines;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public class FrameworkPMView
    {
        private List<PackageData> mPackageDatas = new List<PackageData>();

        public FrameworkLocalVersion FrameworkLocalVersion;

        [MenuItem("Assets/@QPM - Make Package",priority = 0)]
        static void MakePackage()
        {
            var path = MouseSelector.GetSelectedPathOrFallback();

            if (path.IsNotNullAndEmpty())
            {
                if (Directory.Exists(path))
                {
                    var installPath = string.Empty;

                    if (path.EndsWith("/"))
                    {
                        installPath = path;
                    }
                    else
                    {
                        installPath = path + "/";
                    }

                    new PackageVersion
                    {
                        InstallPath = installPath,
                        Version = "v0.0.0" 
                    }.Save();
                    
                    AssetDatabase.Refresh();
                }
            }
        }
        
        private static readonly string EXPORT_ROOT_DIR = Application.dataPath.CombinePath("../");

        public static string ExportPaths(string exportPackageName,params string[] paths)
        {
            if (Directory.Exists(paths[0]))
            {
                if (paths[0].EndsWith("/"))
                {
                    paths[0] = paths[0].Remove(paths[0].Length - 1);
                }
                
                var filePath = EXPORT_ROOT_DIR.CombinePath(exportPackageName);
                AssetDatabase.ExportPackage(paths,
                    filePath, ExportPackageOptions.Recurse);
                AssetDatabase.Refresh();

                return filePath;
            }

            return string.Empty;
        }
        
        public void Init(EditorWindow window)
        {   
            FrameworkLocalVersion = FrameworkLocalVersion.Get();

            mPackageDatas = PackageInfosRequestCache.Get().PackageDatas;

            InstalledPackageVersions.Reload();

            EditorActionKit.ExecuteNode(new GetAllRemotePackageInfo(packageDatas => { mPackageDatas = packageDatas; }));
        }                

        private Vector2 mScrollPos;

        public void OnGUI()
        {
            GUILayout.Label("Framework:");
            GUILayout.BeginVertical("box");
            

            GUILayout.Label(string.Format("Current Framework Version:{0}", FrameworkLocalVersion.Version));

            DrawWithServer();
            
            GUILayout.EndVertical();


            GUILayout.Space(50);
        }
        
        private void DrawWithServer()
        {
            // 这里开始具体的内容
            GUILayout.BeginHorizontal("box");
            GUILayout.Label("Package", GUILayout.Width(150));
            GUILayout.Label("Server", GUILayout.Width(100));
            GUILayout.Label("Local", GUILayout.Width(100));
            GUILayout.Label("Action", GUILayout.Width(100));
            GUILayout.Label("Readme", GUILayout.Width(100));
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical("box");
            
            mScrollPos = GUILayout.BeginScrollView(mScrollPos, false, true, GUILayout.Height(240));

            foreach (var packageData in mPackageDatas)
            {
                GUILayout.Space(2);
                GUILayout.BeginHorizontal();
                GUILayout.Label(packageData.Name, GUILayout.Width(150));
                GUILayout.Label(packageData.Version, GUILayout.Width(100));
                var insalledPackage = InstalledPackageVersions.FindVersionByName(packageData.Name);
                GUILayout.Label(insalledPackage != null ? insalledPackage.Version : " ", GUILayout.Width(90));

                if (insalledPackage != null && packageData.VersionNumber > insalledPackage.VersionNumber)
                {
                    if (GUILayout.Button("Update", GUILayout.Width(90)))
                    {
                        var path = Application.dataPath.Replace("Assets", packageData.InstallPath);

                        if (Directory.Exists(path))
                        {
                            if (EditorUtility.DisplayDialog("UpdatePackage", "是否移除本地旧版本?", "是", "否"))
                            {
                                Directory.Delete(path, true);

                                AssetDatabase.Refresh();
                            }
                        }

                        EditorActionKit.ExecuteNode(new InstallPackage(packageData));
                    }
                }
                else
                {
                    if (GUILayout.Button("Import", GUILayout.Width(90)))
                    {
                        EditorActionKit.ExecuteNode(new InstallPackage(packageData));
                    }
                }
                
                if (GUILayout.Button("Readme", GUILayout.Width(90)))
                {
                    ShowReadMe(packageData.readme);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();

            GUILayout.Space(2);

            GUILayout.EndVertical();
            
        }

        private void ShowReadMe(Readme readme)
        {
            ReadmeWindow.Init(readme);
        }
    }
}