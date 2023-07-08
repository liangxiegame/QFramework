/****************************************************************************
 * Copyright (c) 2021.1 ~ 3 liangxie
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

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    [DisplayName("PackageKit 插件管理")]
    [DisplayNameEN("Plugin Manager(OnlyCN)")]
    [PackageKitGroup("QFramework")]
    [PackageKitRenderOrder(1)]
    internal class PackageManagerView : IPackageKitView, IController, IUnRegisterList
    {
        private IPopup mCategoriesSelectorView = null;


        private MDViewer mMarkdownViewer;

        private PackageKitWindow mPackageKitWindow;

        private IMGUILayout mLeftLayout = null;
        private Rect mLeftRect;

        private IMGUILayout mRightLayout = null;
        private Rect mRightRect;

        private bool mIsOfficial = true;

        public EditorWindow EditorWindow { get; set; }

        public Type Type { get; } = typeof(PackageManagerView);

        public void Init()
        {
            var localPackageVersionModel = this.GetModel<ILocalPackageVersionModel>();

            // 左侧
            mLeftLayout = EasyIMGUI.Vertical()
                .AddChild(EasyIMGUI.Area().WithRectGetter(() => mLeftRect)
                    // 间距 20
                    .AddChild(EasyIMGUI.Vertical()
                        .AddChild(EasyIMGUI.Space().Pixel(20)))
                    // 搜索
                    .AddChild(EasyIMGUI.Horizontal()
                        .AddChild(EasyIMGUI.Label().Text("搜索:")
                            .FontBold()
                            .FontSize(12)
                            .Width(40)
                        ).AddChild(EasyIMGUI.TextField()
                            .Height(20)
                            .Self(search =>
                            {
                                search.Content
                                    .Register(key => { this.SendCommand(new SearchCommand(key)); })
                                    .AddToUnregisterList(this);
                            })
                        )
                    )

                    // 权限
                    .AddChild(EasyIMGUI.Toolbar()
                        .Menus(new List<string>()
                            { "All", PackageAccessRight.Public.ToString(), PackageAccessRight.Private.ToString() })
                        .Self(self =>
                        {
                            self.IndexProperty.Register(value =>
                            {
                                PackageManagerState.AccessRightIndex.Value = value;
                                this.SendCommand(new SearchCommand(PackageManagerState.SearchKey.Value));
                            }).AddToUnregisterList(this);
                        }))
                    // 分类
                    .AddChild(
                        EasyIMGUI.Horizontal()
                            .AddChild(PopupView.Create()
                                .ToolbarStyle()
                                .Self(self =>
                                {
                                    self.IndexProperty.Register(value =>
                                    {
                                        PackageManagerState.CategoryIndex.Value = value;
                                        this.SendCommand(new SearchCommand(PackageManagerState.SearchKey.Value));
                                    }).AddToUnregisterList(this);

                                    mCategoriesSelectorView = self;
                                }))
                    )
                    // 是否是官方
                    .AddChild(
                        EasyIMGUI.Horizontal()
                            .AddChild(EasyIMGUI.Toggle().IsOn(mIsOfficial)
                                .Self(t => t.ValueProperty.Register(v => mIsOfficial = v)))
                            .AddChild(EasyIMGUI.Label().Text("官方"))
                            .AddChild(EasyIMGUI.FlexibleSpace())
                    )
                    .AddChild(EasyIMGUI.Scroll()
                        .AddChild(EasyIMGUI.Custom().OnGUI(() =>
                        {
                            foreach (var p in PackageManagerState.PackageRepositories.Value
                                         .Where(p => p.isOfficial == mIsOfficial)
                                         .OrderByDescending(p =>
                                         {
                                             var installedVersion = localPackageVersionModel.GetByName(p.name);

                                             if (installedVersion == null)
                                             {
                                                 return -1;
                                             }

                                             if (installedVersion.VersionNumber < p.VersionNumber)
                                             {
                                                 return 2;
                                             }

                                             if (installedVersion.VersionNumber == p.VersionNumber)
                                             {
                                                 return 1;
                                             }

                                             return 0;
                                         })
                                         .ThenBy(p => p.name))
                            {
                                GUILayout.BeginVertical("box");

                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.Label(p.name);
                                    GUILayout.FlexibleSpace();
                                }
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal();
                                {
                                    var installedVersion = localPackageVersionModel.GetByName(p.name);

                                    if (installedVersion != null)
                                    {
                                        EasyIMGUI.Box().Text(installedVersion.Version)
                                            .Self(self => self.BackgroundColor = Color.yellow)
                                            .DrawGUI();

                                    }
                                    EasyIMGUI.Box().Text(p.latestVersion)
                                        .Self(self => self.BackgroundColor = Color.green)
                                        .DrawGUI();

                        

                                    GUILayout.FlexibleSpace();


                                    if (installedVersion == null)
                                    {
                                        if (GUILayout.Button(LocaleText.Import))
                                        {
                                            RenderEndCommandExecutor.PushCommand(() =>
                                            {
                                                this.SendCommand(new ImportPackageCommand(p));
                                            });
                                        }
                                    }
                                    else if (installedVersion.VersionNumber < p.VersionNumber)
                                    {
                                        if (GUILayout.Button(LocaleText.Update))
                                        {
                                            RenderEndCommandExecutor.PushCommand(() =>
                                            {
                                                this.SendCommand(new UpdatePackageCommand(p, installedVersion));
                                            });
                                        }
                                    }
                                    else if (installedVersion.VersionNumber == p.VersionNumber)
                                    {
                                        if (GUILayout.Button(LocaleText.Reimport))
                                        {
                                            RenderEndCommandExecutor.PushCommand(() =>
                                            {
                                                this.SendCommand(new UpdatePackageCommand(p, installedVersion));
                                            });
                                        }
                                    }
                                }
                                GUILayout.EndHorizontal();

                                GUILayout.EndVertical();

                                var rect = GUILayoutUtility.GetLastRect();

                                if (mSelectedPackageRepository == p)
                                {
                                    GUI.Box(rect, "", mSelectionRect);
                                }

                                if (rect.Contains(Event.current.mousePosition) &&
                                    Event.current.type == EventType.MouseUp)
                                {
                                    mSelectedPackageRepository = p;
                                    Event.current.Use();
                                }
                            }
                        }))
                    )
                );

            // var skin = AssetDatabase.LoadAssetAtPath<GUISkin>(
            var skin = Resources.Load<GUISkin>("Skin/MarkdownSkinQS");


            mMarkdownViewer = new MDViewer(skin, string.Empty, "");
            // 右侧
            mRightLayout = EasyIMGUI.Vertical()
                .AddChild(EasyIMGUI.Area().WithRectGetter(() => mRightRect)
                    // 间距 20
                    .AddChild(EasyIMGUI.Vertical()
                        .AddChild(EasyIMGUI.Space().Pixel(20))
                    )
                    // 详细信息
                    .AddChild(EasyIMGUI.Vertical()
                        .WithVisibleCondition(() => mSelectedPackageRepository != null)
                        // 名字
                        .AddChild(EasyIMGUI.Label()
                            .Text(() => mSelectedPackageRepository.name)
                            .FontSize(30)
                            .FontBold())
                        .AddChild(EasyIMGUI.Space())
                        // 服务器版本
                        .AddChild(EasyIMGUI.Label()
                            .Text(() => "服务器版本: " + mSelectedPackageRepository.latestVersion)
                            .FontSize(15))
                        // 本地版本
                        .AddChild(EasyIMGUI.Label()
                            .WithVisibleCondition(() =>
                                localPackageVersionModel.GetByName(mSelectedPackageRepository.name) != null)
                            .Text(() =>
                                "本地版本:" + localPackageVersionModel.GetByName(mSelectedPackageRepository.name).Version)
                            .FontSize(15))
                        // 作者
                        .AddChild(EasyIMGUI.Label()
                            .Text(() => "作者:" + mSelectedPackageRepository.author)
                            .FontSize(15))
                        // 权限
                        .AddChild(EasyIMGUI.Label()
                            .Text(() => "权限:" + mSelectedPackageRepository.accessRight)
                            .FontSize(15))
                        // 主页
                        .AddChild(
                            EasyIMGUI.Horizontal()
                                .AddChild(EasyIMGUI.Label()
                                    .FontSize(15)
                                    .Text("插件主页:"))
                                .AddChild(EasyIMGUI.Button()
                                    .Text(() => UrlHelper.PackageUrl(mSelectedPackageRepository))
                                    .FontSize(15)
                                    .OnClick(() =>
                                    {
                                        this.SendCommand(new OpenDetailCommand(mSelectedPackageRepository));
                                    })
                                )
                                .AddChild(EasyIMGUI.FlexibleSpace())
                        )
                        // 描述
                        .AddChild(EasyIMGUI.Label()
                            .Text(() => "描述:")
                            .FontSize(15)
                        )
                        .AddChild(EasyIMGUI.Space())
                        // 描述内容
                        .AddChild(EasyIMGUI.Custom().OnGUI(() =>
                        {
                            mMarkdownViewer.UpdateText(mSelectedPackageRepository.description);
                            var lastRect = GUILayoutUtility.GetLastRect();
                            mMarkdownViewer.DrawWithRect(new Rect(lastRect.x, lastRect.y + lastRect.height,
                                mRightRect.width - 210, mRightRect.height - lastRect.y - lastRect.height));
                            // mMarkdownViewer.Draw();
                        }))
                    )
                );

            mPackageKitWindow = EditorWindow.GetWindow<PackageKitWindow>();

            PackageManagerState.Categories.Register(value =>
            {
                mCategoriesSelectorView.Menus(value);
                mPackageKitWindow.Repaint();
            }).AddToUnregisterList(this);


            // 创建双屏
            mSplitView = mSplitView = new VerticalSplitView(240)
            {
                FirstPan = rect =>
                {
                    mLeftRect = rect;
                    mLeftLayout.DrawGUI();
                },
                SecondPan = rect =>
                {
                    mRightRect = rect;
                    mRightLayout.DrawGUI();
                }
            };
        }

        private static GUIStyle mSelectionRect = "SelectionRect";


        private PackageRepository mSelectedPackageRepository;


        private VerticalSplitView mSplitView;


        public void OnUpdate()
        {
            if (mMarkdownViewer != null && mMarkdownViewer.Update())
            {
                mPackageKitWindow.Repaint();
            }
        }

        public void OnGUI()
        {
            var r = GUILayoutUtility.GetLastRect();
            mSplitView.OnGUI(new Rect(new Vector2(0, r.yMax),
                new Vector2(mPackageKitWindow.position.width, mPackageKitWindow.position.height - r.height)));
        }

        public void OnWindowGUIEnd()
        {
        }

        public void OnDispose()
        {
            mPackageKitWindow = null;
            mSplitView = null;

            this.UnRegisterAll();

            mCategoriesSelectorView = null;

            mLeftLayout.Dispose();
            mLeftLayout = null;

            mRightLayout.Dispose();
            mRightLayout = null;

            mMarkdownViewer = null;
        }

        public void OnShow()
        {
            this.SendCommand<PackageManagerInitCommand>();
        }

        public void OnHide()
        {
        }


        class LocaleText
        {
            public static bool IsCN => LocaleKitEditor.IsCN.Value;
            public static string FrameworkPackages => IsCN ? "框架模块" : "Framework Packages";

            public static string VersionCheck => IsCN ? "版本检测" : "Version Check";

            public static string Import => IsCN ? "导入" : "Import";

            public static string Update => IsCN ? "更新" : "Update";

            public static string Reimport => IsCN ? "再次导入" : "Reimport";
        }

        public IArchitecture GetArchitecture()
        {
            return PackageKit.Interface;
        }

        public List<IUnRegister> UnregisterList { get; } = new List<IUnRegister>();
    }
}
#endif