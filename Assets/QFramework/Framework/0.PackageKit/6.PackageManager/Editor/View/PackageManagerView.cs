/****************************************************************************
 * Copyright (c) 2018.7 ~ 2019.7 liangxie
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EGO.Framework;
using Invert.Common.UI;
using QF.Action;
using QF.Extensions;
using UnityEditor;
using UnityEditorUI;
using UnityEngine;
using HorizontalLayout = EGO.Framework.HorizontalLayout;
using VerticalLayout = EGO.Framework.VerticalLayout;

namespace QF.Editor
{
    public class PackageManagerView : GUIView, IPackageKitView
    {
        private List<PackageData> mPackageDatas = new List<PackageData>();

        private static readonly string EXPORT_ROOT_DIR = Application.dataPath.CombinePath("../");

        public static string ExportPaths(string exportPackageName, params string[] paths)
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


        public PackageManagerView()
        {
            mPackageDatas = PackageInfosRequestCache.Get().PackageDatas;

            InstalledPackageVersions.Reload();

            EditorActionKit.ExecuteNode(new GetAllRemotePackageInfo(packageDatas => { mPackageDatas = packageDatas; }));
        }

        private Vector2 mScrollPos;


        private System.Action mOnToolbarIndexChanged;

        public int ToolbarIndex
        {
            get { return EditorPrefs.GetInt("PM_TOOLBAR_INDEX", 0); }
            set
            {
                EditorPrefs.SetInt("PM_TOOLBAR_INDEX", value);
                mOnToolbarIndexChanged.InvokeGracefully();
            }
        }

        private string[] mToolbarNamesLogined =
            {"Framework", "Plugin", "UIKitComponent", "Shader", "AppOrTemplate", "Private"};

        private string[] mToolbarNamesUnLogined = {"Framework", "Plugin", "UIKitComponent", "Shader", "AppOrTemplate"};

        public string[] ToolbarNames
        {
            get { return User.Logined ? mToolbarNamesLogined : mToolbarNamesUnLogined; }
        }

        public IEnumerable<PackageData> SelectedPackageType
        {
            get
            {
                switch (ToolbarIndex)
                {
                    case 0:
                        return mPackageDatas.Where(packageData => packageData.Type == PackageType.FrameworkModule);
                    case 1:
                        return mPackageDatas.Where(packageData => packageData.Type == PackageType.Plugin);
                    case 2:
                        return mPackageDatas.Where(packageData => packageData.Type == PackageType.UIKitComponent);
                    case 3:
                        return mPackageDatas.Where(packageData => packageData.Type == PackageType.Shader);
                    case 4:
                        return mPackageDatas.Where(packageData =>
                            packageData.Type == PackageType.AppOrGameDemoOrTemplate);
                    case 5:
                        return mPackageDatas.Where(packageData =>
                            packageData.AccessRight == PackageAccessRight.Private);
                    default:
                        return mPackageDatas.Where(packageData => packageData.Type == PackageType.FrameworkModule);
                }
            }
        }


        public static bool VersionCheck
        {
            get { return EditorPrefs.GetBool("QFRAMEWORK_VERSION_CHECK", true); }
            set { EditorPrefs.SetBool("QFRAMEWORK_VERSION_CHECK", value); }
        }

        public IQFrameworkContainer Container { get; set; }

        public int RenderOrder
        {
            get { return 1; }
        }

        public bool Ignore { get; private set; }

        public bool Enabled
        {
            get { return true; }
        }

        private VerticalLayout mEGORootLayout = null;

        public void Init(IQFrameworkContainer container)
        {
            var frameworkData = mPackageDatas.Find(packageData => packageData.Name == "Framework");
            var frameworkVersion = string.Format("QFramework:{0}", frameworkData.Version);

            mEGORootLayout = new VerticalLayout();


            var frameworkInfoLayout = new HorizontalLayout("box")
                .AddTo(mEGORootLayout);

            new EGO.Framework.LabelView(frameworkVersion)
                .Width(150)
                .FontBold()
                .FontSize(12)
                .AddTo(frameworkInfoLayout);

            new ToggleView(LocaleText.VersionCheck, VersionCheck)
//                .FontSize(12)
//                .FontBold()
                .AddTo(frameworkInfoLayout)
                .Toggle.Bind(newValue => VersionCheck = newValue);

            new ToolbarView(ToolbarIndex)
                .Menus(ToolbarNames.ToList())
                .AddTo(mEGORootLayout)
                .Index.Bind(newIndex => ToolbarIndex = newIndex);


            new HeaderView()
                .AddTo(mEGORootLayout);

            var packageList = new VerticalLayout("box")
                .AddTo(mEGORootLayout);

            var scroll = new ScrollLayout()
                .Height(240)
                .AddTo(packageList);

            new EGO.Framework.SpaceView(2).AddTo(scroll);

            mOnToolbarIndexChanged = () =>
            {
                scroll.Clear();

                foreach (var packageData in SelectedPackageType)
                {
                    new EGO.Framework.SpaceView(2).AddTo(scroll);
                    new PackageView(packageData).AddTo(scroll);
                }
            };

            foreach (var packageData in SelectedPackageType)
            {
                new EGO.Framework.SpaceView(2).AddTo(scroll);
                new PackageView(packageData).AddTo(scroll);
            }
        }


        public override void OnGUI()
        {
            base.OnGUI();

            if (GUIHelpers.DoToolbarEx(LocaleText.FrameworkPackages))
            {
                GUILayout.BeginVertical("box");

                mEGORootLayout.DrawGUI();

                GUILayout.EndVertical();
            }
        }

        class LocaleText
        {
            public static string FrameworkPackages
            {
                get { return Language.IsChinese ? "框架模块" : "Framework Packages"; }
            }

            public static string VersionCheck
            {
                get { return Language.IsChinese ? "版本检测" : "Version Check"; }
            }
        }
    }
}