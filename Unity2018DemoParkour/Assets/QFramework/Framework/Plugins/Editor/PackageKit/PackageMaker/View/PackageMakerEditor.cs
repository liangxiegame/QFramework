/****************************************************************************
 * Copyright (c) 2018 ~ 2020.10 liangxie
 * 
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
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
using QFramework.TreeView;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    public class PackageMakerEditor : EasyEditorWindow,IController
    {
        private PackageVersion mPackageVersion;

        private AssetTree _assetTree;
        private AssetTreeIMGUI _assetTreeGUI;
        private Vector2 _scrollPosition;
        
        static string MakeInstallPath()
        {
            var path = MouseSelector.GetSelectedPathOrFallback();

            if (path.EndsWith("/"))
            {
                return path;
            }

            return path + "/";
        }

        private static void MakePackage()
        {
            var path = MouseSelector.GetSelectedPathOrFallback();

            if (!string.IsNullOrEmpty(path))
            {
                if (Directory.Exists(path))
                {
                    var installPath = MakeInstallPath();

                    new PackageVersion
                    {
                        InstallPath = installPath,
                        Version = "v0.0.0",
                        IncludeFileOrFolders = new List<string>()
                        {
                            // 去掉最后一个元素
                            installPath.Remove(installPath.Length - 1)
                        }
                    }.Save();

                    AssetDatabase.Refresh();
                }
            }
        }

        [MenuItem("Assets/@QPM - Publish Package", true)]
        static bool ValiedateExportPackage()
        {
            return User.Logined;
        }

        [MenuItem("Assets/@QPM - Publish Package", priority = 2)]
        public static void publishPackage()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                EditorUtility.DisplayDialog("Package Manager", "请连接网络", "确定");
                return;
            }

            var selectObject = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);

            if (selectObject == null || selectObject.Length > 1)
            {
                return;
            }

            if (!EditorUtility.IsPersistent(selectObject[0]))
            {
                return;
            }

            var path = AssetDatabase.GetAssetPath(selectObject[0]);

            if (!Directory.Exists(path))
            {
                return;
            }

            var window = (PackageMakerEditor) GetWindow(typeof(PackageMakerEditor), true);

            window.titleContent = new GUIContent(selectObject[0].name);

            window.position = new Rect(Screen.width / 2, Screen.height / 2, 258, 500);

            window.Show();
        }


        private VerticalLayout RootLayout = null;

        DisposableList mDisposableList = new DisposableList();

        private string mPublishVersion = null;

        protected override void Init()
        {
            PackageMakerModel.InitState();

            var hashSet = new HashSet<string>();

            if (mPackageVersion.IncludeFileOrFolders.Count == 0 && mPackageVersion.InstallPath.EndsWith("/"))
            {
                hashSet.Add(mPackageVersion.InstallPath.Remove(mPackageVersion.InstallPath.Length - 1));
            }

            foreach (var packageIncludeFileOrFolder in mPackageVersion.IncludeFileOrFolders)
            {
                hashSet.Add(packageIncludeFileOrFolder);
            }

            _assetTree = new AssetTree();
            _assetTreeGUI = new AssetTreeIMGUI(_assetTree.Root);

            var guids = AssetDatabase.FindAssets(string.Empty);
            int i = 0, l = guids.Length;
            for (; i < l; ++i)
            {
                _assetTree.AddAsset(guids[i], hashSet);
            }

            RootLayout = new VerticalLayout("box");

            var editorView = EasyIMGUI.Vertical().Parent(RootLayout);
            var uploadingView = new VerticalLayout().Parent(RootLayout);

            // 当前版本号
            var versionLine = EasyIMGUI.Horizontal().Parent(editorView);
            EasyIMGUI.Label().Text("当前版本号").Width(100).Parent(versionLine);
            EasyIMGUI.Label().Text(mPackageVersion.Version).Width(100).Parent(versionLine);

            // 发布版本号 
            var publishedVersionLine = new HorizontalLayout().Parent(editorView);

            EasyIMGUI.Label().Text("发布版本号")
                .Width(100)
                .Parent(publishedVersionLine);

            EasyIMGUI.TextField()
                .Text(mPublishVersion)
                .Width(100)
                .Parent(publishedVersionLine)
                .Content.Bind(v => mPublishVersion = v);

            // 类型
            var typeLine = EasyIMGUI.Horizontal().Parent(editorView);
            EasyIMGUI.Label().Text("类型").Width(100).Parent(typeLine);

            var packageType = new EnumPopupView(mPackageVersion.Type).Parent(typeLine);

            var accessRightLine = EasyIMGUI.Horizontal().Parent(editorView);
            EasyIMGUI.Label().Text("权限").Width(100).Parent(accessRightLine);
            var accessRight = new EnumPopupView(mPackageVersion.AccessRight).Parent(accessRightLine);

            EasyIMGUI.Label().Text("发布说明:").Width(150).Parent(editorView);

            var releaseNote = EasyIMGUI.TextArea().Width(245)
                .Parent(editorView);

            // 文件选择部分
            EasyIMGUI.Label().Text("插件目录: " + mPackageVersion.InstallPath)
                .Parent(editorView);

            EasyIMGUI.Custom().OnGUI(() =>
            {
                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

                _assetTreeGUI.DrawTreeLayout();

                EditorGUILayout.EndScrollView();
            }).Parent(editorView);


            PackageMakerModel.InEditorView.BindWithInitialValue(value => { editorView.Visible = value; })
                .AddToDisposeList(mDisposableList);

            if (User.Logined)
            {
                EasyIMGUI.Button()
                    .Text("发布")
                    .OnClick(() =>
                    {
                        var includedPaths = new List<string>();
                        _assetTree.Root.Traverse(data =>
                        {
                            if (data != null && data.isSelected)
                            {
                                includedPaths.Add(data.fullPath);
                                return false;
                            }

                            return true;
                        });

                        mPackageVersion.IncludeFileOrFolders = includedPaths;
                        mPackageVersion.Readme.content = releaseNote.Content.Value;
                        mPackageVersion.AccessRight = (PackageAccessRight) accessRight.ValueProperty.Value;
                        mPackageVersion.Type = (PackageType) packageType.ValueProperty.Value;
                        mPackageVersion.Version = mPublishVersion;
                        this.SendCommand(new PublishPackageCommand(mPackageVersion));
                    }).Parent(editorView);
            }

            var notice = new LabelViewWithRect("", 100, 200, 200, 200).Parent(uploadingView);

            PackageMakerModel.NoticeMessage
                .BindWithInitialValue(value => { notice.Content.Value = value; }).AddToDisposeList(mDisposableList);

            PackageMakerModel.InUploadingView.BindWithInitialValue(value => { uploadingView.Visible = value; })
                .AddToDisposeList(mDisposableList);
        }


        private void OnEnable()
        {
            var selectObject = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);

            if (selectObject == null || selectObject.Length > 1)
            {
                return;
            }

            var packageFolder = AssetDatabase.GetAssetPath(selectObject[0]);

            var files = Directory.GetFiles(packageFolder, "PackageVersion.json", SearchOption.TopDirectoryOnly);

            if (files.Length <= 0)
            {
                MakePackage();
            }

            mPackageVersion = PackageVersion.Load(packageFolder);
            mPackageVersion.InstallPath = MakeInstallPath();

            mPublishVersion = mPackageVersion.Version;

            var versionNumbers = mPublishVersion.Split('.');
            var lastVersionNumber = int.Parse(versionNumbers.Last());
            lastVersionNumber++;
            versionNumbers[versionNumbers.Length - 1] = lastVersionNumber.ToString();
            mPublishVersion = string.Join(".", versionNumbers);
        }

        public override void OnUpdate()
        {
        }


        public override void OnClose()
        {
            mDisposableList.Dispose();
            mDisposableList = null;
        }


        public override void OnGUI()
        {
            base.OnGUI();

            RootLayout.DrawGUI();

            RenderEndCommandExecutor.ExecuteCommand();
        }

        public IArchitecture GetArchitecture()
        {
            return PackageMaker.Interface;
        }
    }
}