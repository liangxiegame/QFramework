#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    internal class MyPackageListView
    {
        private IPopup mCategoriesSelectorView = null;

        private MDViewer mMarkdownViewer;

        private PackageKitWindow mPackageKitWindow;

        private IMGUILayout mLeftLayout = null;
        private Rect mLeftRect;
        private IMGUILayout mRightLayout = null;
        private Rect mRightRect;
        private static readonly GUIStyle mSelectionRect = "SelectionRect";
        private PackageRepository mSelectedPackageRepository;
        private VerticalSplitView mSplitView;
        internal List<PackageRepository> PackageRepositories { get; set; } = new List<PackageRepository>();

        internal void Init(IArchitecture architecture, IUnRegisterList unRegisterList,
            LocalPackageVersionModel localPackageVersionModel, Action<string> onSearch)
        {
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
                                    .Register(key => { onSearch(key); })
                                    .AddToUnregisterList(unRegisterList);
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
                                PackageKit.AccessRightIndex.Value = value;
                                onSearch(PackageKit.SearchKey.Value);
                            }).AddToUnregisterList(unRegisterList);
                        }))
                    .AddChild(EasyIMGUI.Scroll()
                        .AddChild(EasyIMGUI.Custom().OnGUI(() =>
                        {
                            foreach (var p in PackageRepositories
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
                                    if (p.status == PackageVersionStatus.Default)
                                    {
                                        GUILayout.Label(nameof(PackageVersionStatus.Release));
                                    }
                                    else
                                    {
                                        GUILayout.Label(p.status.ToString());
                                    }
                                }
                                GUILayout.EndHorizontal();

                                GUILayout.BeginHorizontal();
                                {
                                    var installedVersion = localPackageVersionModel.GetByName(p.name);

                                    if (installedVersion != null)
                                    {
                                        EasyIMGUI.Box().Text(installedVersion.Version)
                                            .Color(Color.yellow)
                                            .DrawGUI();
                                    }

                                    EasyIMGUI.Box().Text(p.latestVersion)
                                        .Color(Color.green)
                                        .DrawGUI();

                              

                                    GUILayout.FlexibleSpace();

                                    if (installedVersion == null)
                                    {
                                        if (GUILayout.Button(LocaleText.Import))
                                        {
                                            RenderEndCommandExecutor.PushCommand(() =>
                                            {
                                                architecture.SendCommand(new ImportPackageCommand(p));
                                            });
                                        }
                                    }
                                    else if (installedVersion.VersionNumber < p.VersionNumber)
                                    {
                                        if (GUILayout.Button(LocaleText.Update))
                                        {
                                            RenderEndCommandExecutor.PushCommand(() =>
                                            {
                                                architecture.SendCommand(
                                                    new UpdatePackageCommand(p, installedVersion));
                                            });
                                        }
                                    }
                                    else if (installedVersion.VersionNumber == p.VersionNumber)
                                    {
                                        if (GUILayout.Button(LocaleText.Reimport))
                                        {
                                            RenderEndCommandExecutor.PushCommand(() =>
                                            {
                                                architecture.SendCommand(
                                                    new UpdatePackageCommand(p, installedVersion));
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
                                        architecture.SendCommand(new OpenDetailCommand(mSelectedPackageRepository));
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
                        }))
                    )
                );

            mPackageKitWindow = EditorWindow.GetWindow<PackageKitWindow>();


            // 创建双屏
            mSplitView = new VerticalSplitView(240)
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

        internal void UpdateCategories(List<string> categories)
        {
            mCategoriesSelectorView.Menus(categories);
            mPackageKitWindow.Repaint();
        }

        internal void Update()
        {
            if (mMarkdownViewer != null && mMarkdownViewer.Update())
            {
                mPackageKitWindow.Repaint();
            }
        }

        internal void DrawGUI()
        {
            var r = GUILayoutUtility.GetLastRect();
            mSplitView.OnGUI(new Rect(new Vector2(0, r.yMax),
                new Vector2(mPackageKitWindow.position.width, mPackageKitWindow.position.height - r.height)));
        }

        internal void Dispose()
        {
            mSplitView = null;

            mCategoriesSelectorView = null;

            mLeftLayout.Dispose();
            mLeftLayout = null;

            mRightLayout.Dispose();
            mRightLayout = null;

            mMarkdownViewer = null;
            mPackageKitWindow = null;
        }

        class LocaleText
        {
            private static bool mIsCn => LocaleKitEditor.IsCN.Value;

            internal static string Import => mIsCn ? "导入" : "Import";

            internal static string Update => mIsCn ? "更新" : "Update";

            internal static string Reimport => mIsCn ? "再次导入" : "Reimport";
        }
    }
}
#endif