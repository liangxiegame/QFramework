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
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public class FrameworkPMView
    {
        private List<PackageData> mPackageDatas = new List<PackageData>();

        public FrameworkLocalVersion FrameworkLocalVersion;

        public void Init()
        {
            FrameworkLocalVersion = FrameworkLocalVersion.Get();

            mPackageDatas = PackageInfosRequestCache.Get().PackageDatas;

            EditorActionKit.ExecuteNode(new GetAllRemotePackageInfo(packageDatas => { mPackageDatas = packageDatas; }));
        }

        private Vector2 mScrollPos;

        public void OnGUI()
        {
            mScrollPos = GUILayout.BeginScrollView(mScrollPos, true, true, GUILayout.Width(600), GUILayout.Height(480));

            GUILayout.Label("Framework:");
            GUILayout.BeginVertical("box");
            GUILayout.Label(string.Format("Current Framework Version:{0}", FrameworkLocalVersion.Version));

            DrawWithServer();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Manage Package"))
            {
                Application.OpenURL("http://liangxiegame.com/xadmin/demo/packagefile/");
            }

            if (GUILayout.Button("Upload Package"))
            {
                Application.OpenURL("http://liangxiegame.com/xadmin/demo/packagefile/add/");
            }

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.EndScrollView();

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

            foreach (var packageData in mPackageDatas)
            {
                GUILayout.Space(2);
                GUILayout.BeginHorizontal();
                GUILayout.Label(packageData.Name, GUILayout.Width(150));
                GUILayout.Label(packageData.Version, GUILayout.Width(100));
                GUILayout.Label(" ", GUILayout.Width(100));

                if (GUILayout.Button("Import", GUILayout.Width(90)))
                {
					string path = Application.dataPath.Replace ("Assets", packageData.InstallPath);

                    if (Directory.Exists(path))
                    {
                        if (EditorUtility.DisplayDialog("UpdatePackage", "是否移除本地旧版本?", "是", "否"))
                        {
                            Directory.Delete(path, true);

                            AssetDatabase.Refresh();
                        }
                    }

                    EditorActionKit.ExecuteNode(new UpdatePackage(packageData.DownloadUrl, packageData.Name));
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.Space(2);

            GUILayout.EndVertical();
        }

        private void ShowReadMe(string readmeContet)
        {
            ReadmeWindow.Init(readmeContet);
        }
    }
}